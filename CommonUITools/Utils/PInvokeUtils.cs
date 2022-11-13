using System.Runtime.InteropServices;

namespace CommonUITools.Utils;

/// <summary>
/// Sets the specified window's show state.
/// </summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"/>
internal enum ShowCommands {
    SW_HIDE = 0,
    SW_SHOWNORMAL = 1,
    SW_NORMAL = 1,
    SW_SHOWMINIMIZED = 2,
    SW_SHOWMAXIMIZED = 3,
    SW_MAXIMIZE = 3,
    SW_SHOWNOACTIVATE = 4,
    SW_SHOW = 5,
    SW_MINIMIZE = 6,
    SW_SHOWMINNOACTIVE = 7,
    SW_SHOWNA = 8,
    SW_RESTORE = 9,
    SW_SHOWDEFAULT = 10,
    SW_FORCEMINIMIZE = 11,
    SW_MAX = 11
}

internal class PInvokeUtils {
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
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr ShellExecuteW(
        IntPtr hwnd,
        string lpOperation,
        string lpFile,
        string lpParameters,
        string lpDirectory,
        ShowCommands nShowCmd
    );
}
