using Microsoft.Win32;

namespace PCChangeTracker.Windows;

/// <summary>Registers or removes a per-user, unelevated "start with Windows" entry. Never touches other apps or policy-managed keys.</summary>
public static class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ChangeTracker";

    public static bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        return key?.GetValue(ValueName) is string value && value.Length > 0;
    }

    public static string BuildCommand(string executablePath, string? dataDirectory = null)
    {
        static string QuotePath(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            if (path.Contains('"')) throw new ArgumentException("A startup path cannot contain quotes.", nameof(path));
            var fullPath = Path.GetFullPath(path);
            var trailingSlashes = fullPath.Length - fullPath.TrimEnd('\\').Length;
            return "\"" + fullPath + new string('\\', trailingSlashes) + "\"";
        }
        var command = $"{QuotePath(executablePath)} --start-minimized";
        return dataDirectory is null ? command : command + " --data-dir " + QuotePath(dataDirectory);
    }

    public static bool TrySetEnabled(bool enabled, string executablePath, string? dataDirectory = null)
    {
        try
        {
            if (!enabled)
            {
                using var existing = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
                existing?.DeleteValue(ValueName, throwOnMissingValue: false);
                return true;
            }
            var command = BuildCommand(executablePath, dataDirectory);
            if (command.Length > 260) return false;
            using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);
            if (key is null) return false;
            key.SetValue(ValueName, command, RegistryValueKind.String);
            return true;
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or System.Security.SecurityException or IOException or ArgumentException)
        {
            return false;
        }
    }
}
