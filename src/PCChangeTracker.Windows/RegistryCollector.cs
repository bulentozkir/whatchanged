using System.Globalization;
using Microsoft.Win32;
using PCChangeTracker.Core;

namespace PCChangeTracker.Windows;

public sealed class RegistryCollector(Category category, string dataDirectory,
    CollectionScope scope = CollectionScope.CurrentUser, byte[]? comparisonKey = null) : CollectorBase(category)
{
    private RegistryHive[] Hives => scope switch
    {
        CollectionScope.CurrentUser => [RegistryHive.CurrentUser],
        CollectionScope.Machine => [RegistryHive.LocalMachine],
        _ => throw new ArgumentOutOfRangeException(nameof(scope))
    };

    protected override void Read(CancellationToken cancellationToken)
    {
        if (!CollectorCatalog.Supports(Category, scope)) throw new InvalidOperationException("Source is outside the selected scope.");
        switch (Category)
        {
            case Category.Applications: ReadApplications(cancellationToken); break;
            case Category.Startup: ReadStartup(cancellationToken); break;
            case Category.Services: ReadServices(cancellationToken); break;
            case Category.Environment: ReadEnvironment(); break;
        }
    }

    private void ReadApplications(CancellationToken cancellationToken)
    {
        foreach (var hive in Hives)
        foreach (var view in Views(hive))
            Guard(() =>
            {
                using var root = RegistryKey.OpenBaseKey(hive, view);
                using var uninstall = root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                if (uninstall is null) return;
                foreach (var child in uninstall.GetSubKeyNames())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Guard(() =>
                    {
                        using var entry = uninstall.OpenSubKey(child, false);
                        if (entry is null) { MarkPartial(); return; }
                        var name = Text(entry, "DisplayName");
                        if (string.IsNullOrWhiteSpace(name) || Integer(entry, "SystemComponent") == 1) return;
                        Items.Add(new($"{hive}/{view}/{child}", name, new()
                        {
                            ["Version"] = Text(entry, "DisplayVersion"), ["Publisher"] = Text(entry, "Publisher"),
                            ["Scope"] = hive == RegistryHive.CurrentUser ? "Current user" : "Machine", ["Registry view"] = view.ToString()
                        }));
                    });
                }
            });
    }

    private void ReadStartup(CancellationToken cancellationToken)
    {
        using var key = new ComparisonKey(dataDirectory, comparisonKey);
        KeyId = key.Id;
        foreach (var hive in Hives)
        foreach (var view in Views(hive))
        foreach (var location in new[] { "Run", "RunOnce" })
            Guard(() =>
            {
                using var root = RegistryKey.OpenBaseKey(hive, view);
                var source = @"SOFTWARE\Microsoft\Windows\CurrentVersion\" + location;
                using var entries = root.OpenSubKey(source, false);
                if (entries is null) return;
                foreach (var name in entries.GetValueNames())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Guard(() => Items.Add(new($"{hive}/{view}/{location}/{name}", name, new()
                    {
                        ["Scope"] = hive == RegistryHive.CurrentUser ? "Current user" : "Machine",
                        ["Registration"] = location, ["Source"] = $"{hive}\\{source}",
                        ["Enabled state"] = "Not established by registration", ["Launch value"] = "Privately compared; contents not stored"
                    }) { Fingerprint = key.Fingerprint(Text(entries, name)) }));
                }
            });
    }

    private void ReadServices(CancellationToken cancellationToken)
    {
        using var fingerprint = new ComparisonKey(dataDirectory, comparisonKey);
        KeyId = fingerprint.Id;
        using var root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        using var services = root.OpenSubKey(@"SYSTEM\CurrentControlSet\Services", false)
            ?? throw new InvalidOperationException("Services registry unavailable.");
        foreach (var name in services.GetSubKeyNames())
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard(() =>
            {
                using var service = services.OpenSubKey(name, false);
                if (service is null) { MarkPartial(); return; }
                var type = Integer(service, "Type");
                if ((type & 0x30) == 0) return;
                var start = Integer(service, "Start");
                var displayName = Text(service, "DisplayName");
                if (string.IsNullOrWhiteSpace(displayName) || displayName.StartsWith('@')) displayName = name;
                Items.Add(new(name, displayName, new()
                {
                    ["Service"] = name, ["Startup"] = start switch { 2 => "Automatic", 3 => "Manual", 4 => "Disabled", _ => $"Type {start}" },
                    ["Automatic"] = start == 2 ? "Yes" : "No", ["Delayed start"] = Integer(service, "DelayedAutoStart") == 1 ? "Yes" : "No",
                    ["Launch value"] = "Privately compared; contents not stored"
                }) { Fingerprint = fingerprint.Fingerprint(Text(service, "ImagePath")) });
            });
        }
    }

    private void ReadEnvironment()
    {
        foreach (var hive in Hives)
            Guard(() =>
            {
                using var root = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
                var location = hive == RegistryHive.CurrentUser ? "Environment" : @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                using var environment = root.OpenSubKey(location, false);
                if (environment is null) return;
                var value = environment.GetValue("Path", null, RegistryValueOptions.DoNotExpandEnvironmentNames);
                if (value is null) return;
                var scope = hive == RegistryHive.CurrentUser ? "User" : "Machine";
                Items.Add(new(scope + "/PATH", scope + " PATH", new() { ["Scope"] = scope, ["Entries"] = (string)value }));
            });
    }

    private static RegistryView[] Views(RegistryHive hive) => hive == RegistryHive.CurrentUser
        ? [RegistryView.Registry64] : [RegistryView.Registry64, RegistryView.Registry32];

    internal static string Text(RegistryKey key, string name) => Convert.ToString(key.GetValue(name, "", RegistryValueOptions.DoNotExpandEnvironmentNames), CultureInfo.InvariantCulture) ?? "";
    internal static int Integer(RegistryKey key, string name) => Convert.ToInt32(key.GetValue(name, 0), CultureInfo.InvariantCulture);
}