using System.Security.Principal;
using System.Text;

namespace PCChangeTracker.App;

public sealed class CaptureAccessException(string message) : Exception(message);

/// <summary>
/// Supporting code for collector workers. Workers are child processes started directly from the unelevated app and return
/// results over standard output; there is no elevation, helper process, or inter-process channel beyond that output.
/// </summary>
internal static class CollectorTransport
{
    /// <summary>Whether this process holds administrator rights; the app and its workers refuse to run in that case.</summary>
    internal static bool IsAdministrator
    {
        get
        {
            using var identity = WindowsIdentity.GetCurrent();
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    internal static async Task<string> ReadTextAsync(StreamReader reader, int limit, CancellationToken cancellationToken)
    {
        var result = new StringBuilder();
        var buffer = new char[8192];
        int count;
        while ((count = await reader.ReadAsync(buffer, cancellationToken)) > 0)
        {
            if (result.Length + count > limit) throw new InvalidDataException("Collector output exceeded its size limit.");
            result.Append(buffer, 0, count);
        }
        return result.ToString();
    }
}
