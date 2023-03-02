using System.Runtime.InteropServices;

namespace CommonUITools.Utils;

internal static partial class PInvokeUtils {
    /// <summary>
    /// Performs an operation on a specified file.
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="lpOperation"></param>
    /// <param name="lpFile"></param>
    /// <param name="lpParameters"></param>
    /// <param name="lpDirectory"></param>
    /// <param name="nShowCmd"></param>
    /// <returns></returns>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shellexecutew"/>
#if NET7_0_OR_GREATER
    [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr ShellExecuteW(
        IntPtr hwnd,
        string lpOperation,
        string lpFile,
        string lpParameters,
        string lpDirectory,
        ShowCommands nShowCmd
    );
#elif NET5_0_OR_GREATER
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr ShellExecuteW(
        IntPtr hwnd,
        string lpOperation,
        string lpFile,
        string lpParameters,
        string lpDirectory,
        ShowCommands nShowCmd
    );
#endif

    /// <summary>
    /// Retrieves the current double-click time for the mouse. A double-click is a series of two clicks of the mouse button, the second occurring within a specified time after the first. The double-click time is the maximum number of milliseconds that may occur between the first and second click of a double-click. The maximum double-click time is 5000 milliseconds.
    /// </summary>
    /// <returns></returns>
    /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdoubleclicktime"/>
#if NET7_0_OR_GREATER
    [LibraryImport("user32.dll")]
    public static partial ushort GetDoubleClickTime();
#elif NET5_0_OR_GREATER
    [DllImport("user32.dll")]
    public static extern ushort GetDoubleClickTime();
#endif
}
