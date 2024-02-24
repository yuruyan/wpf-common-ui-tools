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

    /// <summary>
    /// Retrieves the current double-click time for the mouse. A double-click is a series of two clicks of the mouse button, the second occurring within a specified time after the first. The double-click time is the maximum number of milliseconds that may occur between the first and second click of a double-click. The maximum double-click time is 5000 milliseconds.
    /// </summary>
    /// <returns></returns>
    /// <remarks>see https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdoubleclicktime</remarks>
    [LibraryImport("user32.dll")]
    public static partial ushort GetDoubleClickTime();
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
    [DllImport("user32.dll")]
    public static extern ushort GetDoubleClickTime();
#endif
}

/// <summary>
/// Window Backdrop
/// </summary>
internal static partial class PInvokeUtils {
    [Flags]
    public enum DwmWindowAttribute {
        UseImmersiveDarkMode = 20,
        SystembackdropType = 38
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowBorder {
        public int LeftWidth;      // width of left border that retains its size
        public int RightWidth;     // width of right border that retains its size
        public int TopHeight;      // height of top border that retains its size
        public int BottomHeight;   // height of bottom border that retains its size

        public WindowBorder() : this(default) { }

        public WindowBorder(int value) {
            LeftWidth = value;
            RightWidth = value;
            TopHeight = value;
            BottomHeight = value;
        }
    }

#if NET7_0_OR_GREATER
    [LibraryImport("DwmApi.dll")]
    public static partial int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref WindowBorder pMarInset);

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

    [LibraryImport("User32.dll", EntryPoint = "SetWindowLongPtrA")]
    public static partial int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport("User32.dll", EntryPoint = "GetWindowLongPtrA")]
    public static partial int GetWindowLong(nint hWnd, int nIndex);
#elif NET5_0_OR_GREATER
    [DllImport("DwmApi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref WindowBorder pMarInset);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("User32.dll", EntryPoint = "SetWindowLongPtrA")]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [DllImport("User32.dll", EntryPoint = "GetWindowLongPtrA")]
    public static extern int GetWindowLong(nint hWnd, int nIndex);
#endif

    public static int ExtendFrame(IntPtr hwnd, WindowBorder margins) => DwmExtendFrameIntoClientArea(hwnd, ref margins);

    public static int SetWindowAttribute(IntPtr hwnd, DwmWindowAttribute attribute, int parameter) {
        return DwmSetWindowAttribute(
            hwnd,
            attribute,
            ref parameter,
            Marshal.SizeOf<int>()
        );
    }

    public static void HideAllWindowButtons(nint hwnd) {
        _ = SetWindowLong(hwnd, -16, GetWindowLong(hwnd, -16) & (~0x00080000));
    }
}