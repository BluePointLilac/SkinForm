using MyCommon.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyCommon.MyControls
{
    /// <summary>支持透明背景的分层窗体</summary>
    /* https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-blendfunction */
    public class MyLayerForm : Form
    {
        public MyLayerForm()
        {
            this.DoubleBuffered = true;
            this.ShowInTaskbar = this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint, true);
            this.UpdateStyles();
        }

        /// <summary>0~1间的透明度</summary>
        public new double Opacity { get; set; } = 1;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WSEX.WS_EX_LAYERED;//分层窗口
                cp.ExStyle &= ~(int)WSEX.WS_EX_APPWINDOW;//不在任务栏显示
                cp.ExStyle |= (int)WSEX.WS_EX_TOOLWINDOW;//工具窗口，不在alt+tab、win+tab中显示
                return cp;
            }
        }

        /// <summary>更新窗体图像</summary>
        public void UpdateImage(Bitmap bitmap)
        {
            if(bitmap == null || !Image.IsCanonicalPixelFormat(bitmap.PixelFormat)
                || !Image.IsAlphaPixelFormat(bitmap.PixelFormat))
                throw new Exception("The bitmap must be 32 bit with alhpa channel");
            if(this.IsDisposed) return;
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;
            IntPtr screenDC = Apis.GetDC(IntPtr.Zero);
            IntPtr memDc = Apis.CreateCompatibleDC(screenDC);
            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBitmap = Apis.SelectObject(memDc, hBitmap);
                var srcPos = new POINT(this.Left, this.Top);
                var size = new SIZE(this.Width, this.Height);
                var topPos = new POINT(0, 0);
                var blend = new BLENDFUNCTION
                {
                    SourceConstantAlpha = (byte)(255 * this.Opacity),
                    AlphaFormat = 1,
                    BlendFlags = 0,
                    BlendOp = 0,
                };
                Apis.UpdateLayeredWindow(this.Handle, screenDC, ref srcPos, 
                    ref size, memDc, ref topPos, 0, ref blend, 2);
            }
            finally
            {
                Apis.ReleaseDC(IntPtr.Zero, screenDC);
                if(hBitmap != IntPtr.Zero)
                {
                    Apis.SelectObject(memDc, oldBitmap);
                    Apis.DeleteObject(hBitmap);
                }
                Apis.DeleteDC(memDc);
            }
        }
    }
}