namespace PCChangeTracker.Core;

public static class PathComparison
{
    public static string[] Entries(string value) => value.Split(';').Select(entry => entry.Trim()).ToArray();

    public static bool Equal(string before, string after) =>
        Entries(before).SequenceEqual(Entries(after), StringComparer.OrdinalIgnoreCase);

    public static string Describe(string before, string after)
    {
        var oldEntries = Entries(before);
        var newEntries = Entries(after);
        var remaining = oldEntries.ToList();
        var added = new List<string>();
        foreach (var entry in newEntries)
        {
            var position = remaining.FindIndex(value => string.Equals(value, entry, StringComparison.OrdinalIgnoreCase));
            if (position >= 0) remaining.RemoveAt(position);
            else added.Add(entry);
        }
        if (added.Count == 0 && remaining.Count == 0)
            return "PATH entries were reordered. This can change which command a terminal finds first.";
        return $"PATH has {added.Count} added and {remaining.Count} removed entries. Order and duplicates are preserved; existing terminals may still use an older PATH.";
    }
}