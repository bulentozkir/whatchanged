using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PCChangeTracker.App.Controls;

/// <summary>Matches the native title bar to the app theme on Windows 10 1809+ and Windows 11; a no-op elsewhere.</summary>
internal static class WindowTheme
{
    private const int UseImmersiveDarkMode = 20;
    private const int UseImmersiveDarkModeBefore20H1 = 19;

    internal static void ApplyTitleBar(Window window, bool dark)
    {
        var handle = new WindowInteropHelper(window).Handle;
        if (handle == 0) return;
        var value = dark ? 1 : 0;
        if (DwmSetWindowAttribute(handle, UseImmersiveDarkMode, ref value, sizeof(int)) != 0)
            DwmSetWindowAttribute(handle, UseImmersiveDarkModeBefore20H1, ref value, sizeof(int));
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(nint window, int attribute, ref int value, int size);
}
