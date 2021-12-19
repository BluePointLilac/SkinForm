using MyCommon.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyCommon.MyMethods
{
    public static class ControlExtension
    {
        /// <summary>使控件能够移动所在窗体</summary>
        /// <param name="ctr">目标控件</param>
        /// <param name="useApi">是否使用Win32 Api，使用Api延迟低但不能超出屏幕顶部</param>
        public static void CanMoveForm(this Control ctr, bool useApi = true)
        {
            if(useApi)
            {
                DateTime time = DateTime.Now;
                ctr.MouseDown += (sender, e) =>
                {
                    if(e.Button == MouseButtons.Left) time = DateTime.Now;
                };
                ctr.MouseMove += (sender, e) =>
                {
                    if(e.Button != MouseButtons.Left) return;
                    if((DateTime.Now - time).TotalMilliseconds < 20) return;
                    Apis.ReleaseCapture();
                    Apis.PostMessage(ctr.FindForm().Handle, (uint)WM.WM_NCLBUTTONDOWN, (IntPtr)HT.HT_CAPTION, IntPtr.Zero);
                };
            }
            else
            {
                bool isMove = false;
                Point point = Point.Empty;
                ctr.MouseUp += (sender, e) => isMove = false;
                ctr.MouseDown += (sender, e) =>
                {
                    isMove = e.Button == MouseButtons.Left;
                    point = e.Location;
                };
                ctr.MouseMove += (sender, e) =>
                {
                    if(!isMove) return;
                    Form frm = ctr.FindForm();
                    frm.Left += e.X - point.X;
                    frm.Top += e.Y - point.Y;
                };
            }
        }

        /// <summary>通过Win32API禁用/启用控件</summary>
        /// <remarks>控件被禁用时仍可更改字体颜色，不需要同时设置ctr.Enabled=false</remarks>
        /// <param name="ctr">目标控件</param>
        /// <param name="enabled">启用为true，禁用为false</param>
        public static void SetEnabled(this Control ctr, bool enabled)
        {
            SetEnabled(ctr.Handle, enabled);
        }

        /// <summary>通过Win32API禁用/启用控件</summary>
        /// <remarks>控件被禁用时仍可更改字体颜色，不需要同时设置ctr.Enabled=false</remarks>
        /// <param name="handle">目标控件句柄</param>
        /// <param name="enabled">启用为true，禁用为false</param>
        public static void SetEnabled(IntPtr handle, bool enabled)
        {
            int value = Apis.GetWindowLong(handle, (int)GWL.GWL_STYLE);
            if(enabled) value &= ~(int)WS.WS_DISABLED;
            else value |= (int)WS.WS_DISABLED;
            Apis.SetWindowLong(handle, (int)GWL.GWL_STYLE, value);
        }
    }
}