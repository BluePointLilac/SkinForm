using MyCommon.Win32;
using System;
using System.Windows.Forms;

namespace MyCommon.MyMethods
{
    public static class FormExtension
    {
        /// <summary>通过Win32API设置窗体透明度</summary>
        /// <remarks>分层窗口Opacity属性失效，可通过此方法设置透明度</remarks>
        /// <param name="frm">目标窗体</param>
        /// <param name="opacity">0~1之间的透明度</param>
        public static void SetOpacity(this Form frm, double opacity)
        {
            SetOpacity(frm.Handle, opacity);
        }

        /// <summary>通过Win32API设置窗体透明度</summary>
        /// <remarks>分层窗口Opacity属性失效，可通过此方法设置透明度</remarks>
        /// <param name="handle">目标窗体句柄</param>
        /// <param name="opacity">0~1之间的透明度</param>
        public static void SetOpacity(IntPtr handle, double opacity)
        {
            if(opacity < 0 || opacity > 1) return;
            int nRet = Apis.GetWindowLong(handle, (int)GWL.GWL_EXSTYLE);
            nRet |= (int)WSEX.WS_EX_LAYERED;
            Apis.SetWindowLong(handle, (int)GWL.GWL_EXSTYLE, nRet);
            Apis.SetLayeredWindowAttributes(handle, 0, (byte)(255 * opacity), 2);
        }

        /// <summary>设置窗体鼠标穿透</summary>
        /// <param name="frm">目标窗体</param>
        /// <param name="through">是否穿透</param>
        public static void SetMouseThrough(this Form frm, bool through)
        {
            SetMouseThrough(frm.Handle, through);
        }

        /// <summary>设置窗体鼠标穿透</summary>
        /// <param name="handle">目标窗体句柄</param>
        /// <param name="through">是否穿透</param>
        public static void SetMouseThrough(IntPtr handle, bool through)
        {
            int nRet = Apis.GetWindowLong(handle, (int)GWL.GWL_EXSTYLE);
            if(through) nRet |= (int)WSEX.WS_EX_TRANSPARENT;
            else nRet &= ~(int)WSEX.WS_EX_TRANSPARENT;
            Apis.SetWindowLong(handle, (int)GWL.GWL_EXSTYLE, nRet);
        }

        /// <summary>为无边框窗体添加阴影</summary>
        /// <param name="frm">目标窗体</param>
        public static bool AddShadows(this Form frm)
        {
            if(Environment.OSVersion.Version.Major < 6) return false;
            Apis.DwmIsCompositionEnabled(out bool enabled);
            if(!enabled) return false;
            var v = 2;
            Apis.DwmSetWindowAttribute(frm.Handle, 2, ref v, 4);
            MARGINS margins = new MARGINS
            {
                bottomHeight = 1,
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0
            };
            Apis.DwmExtendFrameIntoClientArea(frm.Handle, ref margins);
            return true;
        }
    }
}