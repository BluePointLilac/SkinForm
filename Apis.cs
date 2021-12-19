using System;
using System.Runtime.InteropServices;

namespace MyCommon.Win32
{
    public static class Apis
    {
        #region USER32
        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll ")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern bool AnimateWindow(IntPtr hWnd, int dwtime, AW dwflag);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst,
            ref POINT pptDst, ref SIZE pSize, IntPtr hdcSrc, ref POINT pptSrc,
            uint crKey, ref BLENDFUNCTION pblend, int dwFlags);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect([In] ref ICONINFO piconinfo);
        #endregion

        #region GDI32
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int DeleteObject(IntPtr hObj);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        #endregion

        #region KERNEL32
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
        #endregion

        #region DWMAPI
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);
        #endregion
    }
}