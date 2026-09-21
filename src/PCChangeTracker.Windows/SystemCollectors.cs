using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using PCChangeTracker.Core;

namespace PCChangeTracker.Windows;

internal static class NativeCom
{
    public static dynamic Create(string progId) => Activator.CreateInstance(Type.GetTypeFromProgID(progId, true)!)!;
    public static void Release(object? value)
    {
        if (value is not null && Marshal.IsComObject(value)) Marshal.FinalReleaseComObject(value);
    }
}

public sealed class ProtectionCollector() : CollectorBase(Category.Protection)
{
    protected override void Read(CancellationToken cancellationToken)
    {
        dynamic policy = NativeCom.Create("HNetCfg.FwPolicy2");
        try
        {
            foreach (var profile in new[] { (Id: 1, Name: "Domain firewall"), (Id: 2, Name: "Private firewall"), (Id: 4, Name: "Public firewall") })
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard(() => Items.Add(new(profile.Id.ToString(CultureInfo.InvariantCulture), profile.Name,
                    new() { ["Enabled"] = (bool)policy.FirewallEnabled[profile.Id] ? "Yes" : "No", ["Source"] = "Windows Firewall profile configuration" })));
            }
        }
        finally { NativeCom.Release(policy); }
    }
}

public sealed class TasksCollector(string dataDirectory, byte[]? comparisonKey = null) : CollectorBase(Category.ScheduledTasks)
{
    protected override void Read(CancellationToken cancellationToken)
    {
        using var key = new ComparisonKey(dataDirectory, comparisonKey);
        KeyId = key.Id;
        dynamic scheduler = NativeCom.Create("Schedule.Service");
        object? root = null;
        try
        {
            scheduler.Connect();
            root = scheduler.GetFolder("\\");
            ReadFolder(root, key, cancellationToken, 0);
        }
        finally { NativeCom.Release(root); NativeCom.Release(scheduler); }
    }

    private void ReadFolder(dynamic folder, ComparisonKey key, CancellationToken cancellationToken, int depth)
    {
        if (depth > 32 || Items.Count > 5000) { MarkPartial(); return; }
        Guard(() =>
        {
            dynamic tasks = folder.GetTasks(1);
            try
            {
                var count = (int)tasks.Count;
                for (var index = 1; index <= count; index++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    dynamic task = tasks[index];
                    try { Guard(() => ReadTask(task, key)); }
                    finally { NativeCom.Release(task); }
                }
            }
            finally { NativeCom.Release(tasks); }
        });
        Guard(() =>
        {
            dynamic folders = folder.GetFolders(0);
            try
            {
                for (var index = 1; index <= (int)folders.Count; index++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    dynamic child = folders[index];
                    try { ReadFolder(child, key, cancellationToken, depth + 1); }
                    finally { NativeCom.Release(child); }
                }
            }
            finally { NativeCom.Release(folders); }
        });
    }

    private void ReadTask(dynamic task, ComparisonKey key)
    {
        dynamic definition = task.Definition;
        dynamic actions = definition.Actions;
        dynamic triggers = definition.Triggers;
        try
        {
            var launchValues = new List<string>();
            for (var index = 1; index <= (int)actions.Count; index++)
            {
                dynamic action = actions[index];
                try
                {
                    var type = (int)action.Type;
                    launchValues.Add(type == 0
                        ? JsonSerializer.Serialize(new[] { (string)action.Path, (string)action.Arguments, (string)action.WorkingDirectory })
                        : $"Action type {type}");
                }
                finally { NativeCom.Release(action); }
            }
            var triggerTypes = new List<int>();
            for (var index = 1; index <= (int)triggers.Count; index++)
            {
                dynamic trigger = triggers[index];
                try { triggerTypes.Add((int)trigger.Type); }
                finally { NativeCom.Release(trigger); }
            }
            var enabled = (bool)task.Enabled;
            using var xmlReader = XmlReader.Create(new StringReader((string)definition.XmlText), new XmlReaderSettings
            { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 1_000_000 });
            var xml = XDocument.Load(xmlReader);
            var configuration = xml.Root!.Elements()
                .Where(element => element.Name.LocalName is "Actions" or "Triggers" or "Settings" or "Principals")
                .Select(element => element.ToString(SaveOptions.DisableFormatting)).ToArray();
            Items.Add(new((string)task.Path, (string)task.Name, new()
            {
                ["Task path"] = (string)task.Path, ["Enabled"] = enabled ? "Yes" : "No",
                ["Automatic"] = enabled && triggerTypes.Any(type => type is 8 or 9) ? "Yes" : "No",
                ["Triggers"] = string.Join(", ", triggerTypes.Select(type => type switch
                {
                    0 => "Event", 1 => "Time", 2 => "Daily", 3 => "Weekly", 4 or 5 => "Monthly", 6 => "Idle",
                    7 => "Registration", 8 => "Startup", 9 => "Sign-in", 11 => "Session change", _ => $"Type {type}"
                })),
                ["Actions"] = $"{launchValues.Count} action(s); launch values privately compared"
            }) { Fingerprint = key.Fingerprint(JsonSerializer.Serialize(configuration)) });
        }
        finally { NativeCom.Release(triggers); NativeCom.Release(actions); NativeCom.Release(definition); }
    }
}

public sealed class UpdatesCollector() : CollectorBase(Category.WindowsUpdates)
{
    protected override void Read(CancellationToken cancellationToken)
    {
        dynamic session = NativeCom.Create("Microsoft.Update.Session");
        dynamic searcher = session.CreateUpdateSearcher();
        try
        {
            var seen = new Dictionary<string, ConfigurationItem>(StringComparer.OrdinalIgnoreCase);
            var total = (int)searcher.GetTotalHistoryCount();
            if (total > 5000) { MarkPartial(); total = 5000; }
            for (var offset = 0; offset < total; offset += 100)
            {
                cancellationToken.ThrowIfCancellationRequested();
                dynamic page = searcher.QueryHistory(offset, Math.Min(100, total - offset));
                try
                {
                    for (var index = 0; index < (int)page.Count; index++)
                    {
                        dynamic entry = page[index];
                        try
                        {
                            if ((int)entry.ResultCode != 2) continue;
                            dynamic identity = entry.UpdateIdentity;
                            try
                            {
                                var date = DateTime.SpecifyKind((DateTime)entry.Date, DateTimeKind.Utc);
                                var operation = (int)entry.Operation == 1 ? "Installed" : "Uninstalled";
                                var id = $"{identity.UpdateID}/{identity.RevisionNumber}/{date:O}/{operation}";
                                var item = new ConfigurationItem(id, (string)entry.Title, new()
                                {
                                    ["Operation"] = operation, ["Windows event time (UTC)"] = date.ToString("O", CultureInfo.InvariantCulture),
                                    ["Result"] = "Succeeded"
                                });
                                if (seen.TryGetValue(id, out var previous))
                                {
                                    if (previous.Name != item.Name) MarkPartial();
                                }
                                else { seen.Add(id, item); Items.Add(item); }
                            }
                            finally { NativeCom.Release(identity); }
                        }
                        finally { NativeCom.Release(entry); }
                    }
                }
                finally { NativeCom.Release(page); }
            }
        }
        finally { NativeCom.Release(searcher); NativeCom.Release(session); }
    }
}