using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using PCChangeTracker.Core;

namespace PCChangeTracker.Windows;

public sealed class DefaultAppsCollector() : CollectorBase(Category.DefaultApps)
{
    protected override void Read(CancellationToken cancellationToken)
    {
        foreach (var association in new[] { "http", "https", ".pdf", ".jpg", ".png", ".mp3", ".mp4", ".zip", ".csv" })
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard(() =>
            {
                var identifier = Query(association, 20);
                var friendly = identifier is null ? null : Query(association, 4, false);
                Items.Add(new(association, association switch { "http" => "Web links (HTTP)", "https" => "Web links (HTTPS)", ".pdf" => "PDF files", _ => association + " files" },
                    new() { ["Handler"] = identifier ?? "No association", ["Application"] = friendly ?? identifier ?? "Not configured" }));
            });
        }
    }

    private static string? Query(string association, uint kind, bool strict = true)
    {
        var flags = association.StartsWith('.') ? 0u : 0x1000u;
        uint length = 0;
        var result = AssocQueryString(flags, kind, association, null, null, ref length);
        if (result == unchecked((int)0x80070483) || result == unchecked((int)0x80070002)) return null;
        if (length == 0 || length > 32768)
        {
            if (strict) Marshal.ThrowExceptionForHR(result < 0 ? result : unchecked((int)0x80004005));
            return null;
        }
        var output = new StringBuilder((int)length);
        result = AssocQueryString(flags, kind, association, null, output, ref length);
        if (result == 0) return output.ToString();
        if (strict) Marshal.ThrowExceptionForHR(result);
        return null;
    }

    [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, EntryPoint = "AssocQueryStringW")]
    private static extern int AssocQueryString(uint flags, uint kind, string association, string? extra, StringBuilder? output, ref uint length);
}

public sealed class AudioCollector() : CollectorBase(Category.Audio)
{
    protected override void Read(CancellationToken cancellationToken)
    {
        using var enumerator = new MMDeviceEnumerator();
        foreach (var flow in new[] { DataFlow.Render, DataFlow.Capture })
        foreach (var role in new[] { Role.Multimedia, Role.Communications })
        {
            cancellationToken.ThrowIfCancellationRequested();
            var label = (flow == DataFlow.Render ? "Speakers" : "Microphone") + (role == Role.Communications ? " (communications)" : " (default)");
            Guard(() =>
            {
                try
                {
                    using var endpoint = enumerator.GetDefaultAudioEndpoint(flow, role);
                    Items.Add(new($"{flow}/{role}", label, new() { ["Device"] = endpoint.FriendlyName, ["Endpoint"] = endpoint.ID }));
                }
                catch (COMException exception) when (exception.HResult == unchecked((int)0x80070490))
                {
                    Items.Add(new($"{flow}/{role}", label, new() { ["Device"] = "No default endpoint", ["Endpoint"] = "None" }));
                }
            });
        }
    }
}

public sealed class DriverCollector() : CollectorBase(Category.Drivers)
{
    protected override void Read(CancellationToken cancellationToken)
    {
        using var searcher = new ManagementObjectSearcher("SELECT DeviceID,DeviceName,DriverVersion,DriverProviderName,InfName FROM Win32_PnPSignedDriver");
        searcher.Options.Timeout = TimeSpan.FromSeconds(15);
        using var results = searcher.Get();
        foreach (ManagementObject device in results)
        {
            using (device)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard(() =>
                {
                    var id = device["DeviceID"]?.ToString();
                    if (string.IsNullOrWhiteSpace(id)) { MarkPartial(); return; }
                    Items.Add(new(id, device["DeviceName"]?.ToString() ?? "Unnamed device", new()
                    {
                        ["Version"] = device["DriverVersion"]?.ToString() ?? "Unknown",
                        ["Provider"] = device["DriverProviderName"]?.ToString() ?? "Unknown",
                        ["Driver package"] = device["InfName"]?.ToString() ?? "Unknown"
                    }));
                });
            }
        }
    }
}

public sealed class NetworkCollector(string dataDirectory,
    CollectionScope scope = CollectionScope.CurrentUser, byte[]? comparisonKey = null) : CollectorBase(Category.Network)
{
    protected override void Read(CancellationToken cancellationToken)
    {
        if (scope == CollectionScope.Machine) { ReadAdapters(cancellationToken); return; }
        if (scope != CollectionScope.CurrentUser) throw new ArgumentOutOfRangeException(nameof(scope));
        cancellationToken.ThrowIfCancellationRequested();
        using var key = new ComparisonKey(dataDirectory, comparisonKey);
        KeyId = key.Id;
        Guard(() =>
        {
            using var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            using var proxy = root.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", false);
            if (proxy is null) return;
            var server = RegistryCollector.Text(proxy, "ProxyServer");
            var configurationUrl = RegistryCollector.Text(proxy, "AutoConfigURL");
            Items.Add(new("current-user-proxy", "Current-user proxy", new()
            {
                ["Enabled"] = RegistryCollector.Integer(proxy, "ProxyEnable") == 1 ? "Yes" : "No",
                ["Server"] = ReportExporter.Sanitize(server),
                ["Automatic configuration"] = SafeUrl(configurationUrl),
                ["Hidden configuration"] = "Privately compared; URL paths and queries are not stored"
            }) { Fingerprint = key.Fingerprint(JsonSerializer.Serialize(new[] { server, configurationUrl })) });
        });
    }

    private void ReadAdapters(CancellationToken cancellationToken)
    {
        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces().Where(adapter => adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback))
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard(() =>
            {
                var properties = adapter.GetIPProperties();
                Items.Add(new(adapter.Id, adapter.Name, new()
                {
                    ["Adapter"] = adapter.Description, ["Type"] = adapter.NetworkInterfaceType.ToString(),
                    ["DNS servers"] = string.Join(", ", properties.DnsAddresses.Select(address => address.ToString())),
                    ["DHCP"] = !adapter.Supports(NetworkInterfaceComponent.IPv4) ? "Not applicable (no IPv4)"
                        : properties.GetIPv4Properties()?.IsDhcpEnabled == true ? "Yes" : "No"
                }));
            });
        }
    }

    private static string SafeUrl(string value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
        ? $"{uri.Scheme}://{uri.Host}:{uri.Port}/[path omitted]" : string.IsNullOrEmpty(value) ? "Not configured" : "Configured; value omitted";
}