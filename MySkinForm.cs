using MyCommon.MyMethods;
using MyCommon.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyCommon.MyControls
{
    /// <summary>圆角阴影窗体</summary>
    /// <remarks>注：由于皮肤窗体阴影层级问题隐藏了此类的原生Owner，
    /// 在调用此类或其继承类的Owner时不可使用Form强制类型转换得到Owner，
    /// 也不应在此类或其继承类中使用Show(owner)或ShowDialog(owner)来设置Owner，
    /// 如需指定Owner，请在Show或ShowDialog前单独设置好Owner属性</remarks>
    public class MySkinForm : Form
    {
        //控件层窗体
        public MySkinForm()
        {
            this.SuspendLayout();
            this.ControlBox = false;
            this.ResizeRedraw = true;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.White;
            this.MinimumSize = new Size(48, 48);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ForeColor = Color.FromArgb(102, 102, 102);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Microsoft YaHei UI", 12F);
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.ContainerControl
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            this.ResumeLayout(false);
        }

        //阴影层窗体
        private readonly MyLayerForm ShadowForm = new MyLayerForm();

        //隐藏边框属性
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FormBorderStyle FormBorderStyle
        {
            get => base.FormBorderStyle;
            set => base.FormBorderStyle = FormBorderStyle.None;
        }

        public new Form Owner
        {
            get => ShadowForm.Owner;
            set
            {
                ShadowForm.Owner = value;
                base.Owner = ShadowForm;
            }
        }

        public new bool TopMost
        {
            get => base.TopMost;
            set => ShadowForm.TopMost = base.TopMost = value;
        }

        public new double Opacity
        {
            get => ShadowForm.Opacity;
            set
            {
                ShadowForm.Opacity = value;
                this.SetOpacity(value);
                this.DrawShadow();
            }
        }

        /// <summary>是否显示阴影</summary>
        [Description("是否显示阴影")]
        public bool ShowShadow { get; set; } = true;

        private Font titleBarFont = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold);
        /// <summary>标题栏字体</summary>
        [Description("标题栏字体")]
        public Font TitleBarFont
        {
            get => titleBarFont;
            set
            {
                titleBarFont = value;
                this.Invalidate(false);
            }
        }

        private Color titleBarForeColor = Color.FromArgb(102, 102, 102);
        /// <summary>标题栏前景色</summary>
        [Description("标题栏前景色")]
        public Color TitleBarForeColor
        {
            get => titleBarForeColor;
            set
            {
                titleBarForeColor = value;
                this.Refresh();
            }
        }

        private Color titleBarBackColor = Color.FromArgb(238, 238, 238);

        /// <summary>标题栏背景色</summary>
        [Description("标题栏背景色")]
        public Color TitleBarBackColor
        {
            get => titleBarBackColor;
            set
            {
                titleBarBackColor = value;
                this.Refresh();
            }
        }

        private int titleBarHeight = 48;
        /// <summary>标题栏高度</summary>
        [Description("标题栏高度")]
        public int TitleBarHeight
        {
            get => titleBarHeight;
            set
            {
                titleBarHeight = value;
                this.Refresh();
            }
        }

        private bool showTitleBar = true;
        /// <summary>是否显示标题栏</summary>
        [Description("是否显示标题栏")]
        public bool ShowTitleBar
        {
            get => showTitleBar;
            set
            {
                showTitleBar = value;
                this.Refresh();
            }
        }

        private bool showCloseBox = true;
        /// <summary>是否显示关闭按钮</summary>
        [Description("是否显示关闭按钮")]
        public bool ShowCloseBox
        {
            get => showCloseBox;
            set
            {
                showCloseBox = value;
                this.Invalidate(false);
            }
        }

        private int cornerRadius = 8;
        /// <summary>窗体圆角半径</summary>
        [Description("窗体圆角半径")]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value;
                this.SetRoundBorder();
                this.DrawShadow();
            }
        }

        private int shadowThickness = 5;
        /// <summary>窗体阴影宽度</summary>
        [Description("窗体阴影宽度")]
        public int ShadowThickness
        {
            get => shadowThickness;
            set
            {
                shadowThickness = value;
                this.OnLocationChanged(null);
                this.OnResize(null);
            }
        }

        private Color shadowColor = Color.FromArgb(40, 40, 40, 40);
        /// <summary>窗体阴影颜色</summary>
        [Description("窗体阴影颜色")]
        public Color ShadowColor
        {
            get => shadowColor;
            set
            {
                shadowColor = value;
                this.DrawShadow();
            }
        }

        /// <summary>显示窗体时的动画</summary>
        [Description("显示窗体时的动画")]
        public AW ShowAnimate { get; set; } = AW.AW_ACTIVATE;
        /// <summary>隐藏窗体时的动画</summary>
        [Description("隐藏窗体时的动画")]
        public AW HideAnimate { get; set; } = AW.AW_HIDE;
        /// <summary>显示窗体时的动画时长</summary>
        [Description("显示窗体时的动画时长")]
        public int ShowAnimateTime { get; set; } = 250;
        /// <summary>隐藏窗体时的动画</summary>
        [Description("隐藏窗体时的动画")]
        public int HideAnimateTime { get; set; } = 250;

        /// <summary>水平方向可调整大小</summary>
        [Description("水平方向可调整大小")]
        public bool HorizontalResizable { get; set; } = false;

        /// <summary>竖直方向可调整大小</summary>
        [Description("竖直方向可调整大小")]
        public bool VerticalResizable { get; set; } = false;

        /// <summary>窗体可移动范围</summary>
        [Description("窗体可移动范围")]
        public Rectangle MovableRange { get; set; } = Rectangle.Empty;

        /// <summary>是否可以通过任务栏、任务视图、Alt+Tab视图、Alt+F4系统菜单命令关闭窗体</summary>
        [Description("是否可以通过任务栏、任务视图、Alt+Tab视图、Alt+F4系统菜单命令关闭窗体")]
        public bool SysClosable { get; set; } = false;

        /// <summary>是否可用Esc键关闭窗体</summary>
        [Description("是否可用Esc键关闭窗体")]
        public bool EscToClose { get; set; } = false;

        /// <summary>是否可用Esc键隐藏窗体</summary>
        [Description("是否可用Esc键隐藏窗体")]
        public bool EscToHide { get; set; } = false;

        private bool isMoving = false;//是否正在移动窗体
        private int tempRadius = 0;//临时圆角大小

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ShadowThickness = this.shadowThickness;
            if(this.Owner != null) this.TopMost = Owner.TopMost;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ShadowForm.Width = this.Width + this.ShadowThickness * 2;
            ShadowForm.Height = this.Height + this.ShadowThickness * 2;
            this.CornerRadius = this.cornerRadius;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            ShadowForm.Left = this.Left - this.ShadowThickness;
            ShadowForm.Top = this.Top - this.ShadowThickness;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if(HideAnimate != AW.AW_HIDE) this.Hide();
            base.OnFormClosed(e);
            if(ShadowForm.IsHandleCreated) ShadowForm.Invoke(new Action(ShadowForm.Dispose));
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if(DesignMode) return;
            if(this.Visible)
            {
                this.SetOpacity(this.Opacity);
                if(ShowAnimate != AW.AW_ACTIVATE)
                {
                    Apis.AnimateWindow(this.Handle, this.ShowAnimateTime, AW.AW_ACTIVATE | ShowAnimate);
                    this.Refresh();
                }
                if(this.ShowShadow && !ShadowForm.Visible)
                {
                    ShadowForm.Show();
                    ShadowForm.SetMouseThrough(true);
                }
                base.Owner = ShadowForm;
            }
            else
            {
                ShadowForm.Hide();
                if(HideAnimate != AW.AW_HIDE)
                {
                    Apis.AnimateWindow(this.Handle, this.HideAnimateTime, AW.AW_HIDE | HideAnimate);
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            int a = TextRenderer.MeasureText("A", this.TitleBarFont).Height;
            int b = (this.TitleBarHeight - a) / 2;
            if(this.ShowTitleBar)
            {
                Rectangle rect = new Rectangle(0, 0, this.Width, this.TitleBarHeight);
                using(Brush brush = new SolidBrush(this.TitleBarBackColor))
                {
                    g.FillRectangle(brush, rect);
                }
                using(Brush brush = new SolidBrush(this.TitleBarForeColor))
                {
                    g.DrawString(this.Text, this.TitleBarFont, brush, b, b);
                }
            }
            if(this.ShowCloseBox)
            {
                Rectangle rect = new Rectangle(this.Width - a - b, b, a, a);
                rect.Inflate(-4, -4);
                using(Pen pen = new Pen(this.TitleBarForeColor, 3F))
                {
                    g.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
                    g.DrawLine(pen, rect.X + rect.Width, rect.Y, rect.X, rect.Y + rect.Height);
                }
            }
            base.OnPaint(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && this.ShowTitleBar && e.Y <= this.TitleBarHeight)
            {
                Apis.ReleaseCapture();
                Message m = new Message
                {
                    HWnd = this.Handle,
                    Msg = (int)WM.WM_NCLBUTTONDOWN,
                    WParam = (IntPtr)HT.HT_CAPTION,
                };
                this.WndProc(ref m);
            }
            else base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && this.ShowCloseBox)
            {
                int a = TextRenderer.MeasureText("A", this.Font).Height;
                int b = (this.TitleBarHeight - a) / 2;
                Rectangle rect = new Rectangle(this.Width - a - b, b, a, a);
                if(rect.Contains(e.Location)) this.Close();
            }
            base.OnMouseDown(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch((WM)m.Msg)
            {
                #region 限制窗体可移动范围
                case WM.WM_WINDOWPOSCHANGING:
                    if(this.MovableRange.Equals(Rectangle.Empty)) break;
                    Rectangle rect = this.MovableRange;
                    rect.Inflate(-1, -1);
                    WINDOWPOS pos = (WINDOWPOS)m.GetLParam(typeof(WINDOWPOS));
                    pos.x = Math.Max(pos.x, rect.Left);
                    pos.y = Math.Max(pos.y, rect.Top);
                    pos.x = Math.Min(pos.x, rect.Right - pos.cx);
                    pos.y = Math.Min(pos.y, rect.Bottom - pos.cy);
                    Marshal.StructureToPtr(pos, m.LParam, false);
                    break;
                #endregion

                #region 无边框窗体边缘拖拽放缩大小
                case WM.WM_NCHITTEST:
                    isMoving = false;
                    base.WndProc(ref m);
                    if(this.WindowState != FormWindowState.Normal) return;
                    Point point = PointToClient(Cursor.Position);
                    int w = Math.Max(this.CornerRadius / 2, 3);
                    int x = point.X;
                    int y = point.Y;
                    HT res = HT.HT_CLIENT;
                    if(x <= w)
                    {
                        if(y <= w) res = HT.HT_TOPLEFT;
                        else if(y >= ClientSize.Height - w) res = HT.HT_BOTTOMLEFT;
                        else if(x <= w) res = HT.HT_LEFT;
                    }
                    else if(x >= ClientSize.Width - w)
                    {
                        if(y <= w) res = HT.HT_TOPRIGHT;
                        else if(y >= ClientSize.Height - w) res = HT.HT_BOTTOMRIGHT;
                        else if(x >= ClientSize.Width - w) res = HT.HT_RIGHT;
                    }
                    else if(y <= w) res = HT.HT_TOP;
                    else if(y >= ClientSize.Height - w) res = HT.HT_BOTTOM;
                    switch(res)
                    {
                        case HT.HT_TOP:
                        case HT.HT_BOTTOM:
                            if(!this.VerticalResizable) res = HT.HT_NOWHERE;
                            break;
                        case HT.HT_LEFT:
                        case HT.HT_RIGHT:
                            if(!this.HorizontalResizable) res = HT.HT_NOWHERE;
                            break;
                        case HT.HT_TOPLEFT:
                        case HT.HT_TOPRIGHT:
                        case HT.HT_BOTTOMLEFT:
                        case HT.HT_BOTTOMRIGHT:
                            if(!this.VerticalResizable || !this.HorizontalResizable) res = HT.HT_NOWHERE;
                            break;
                    }
                    m.Result = (IntPtr)res;
                    return;
                #endregion

                #region 标题栏左键按下拖拽移动窗体
                case WM.WM_NCLBUTTONDOWN:
                    switch((HT)m.WParam)
                    {
                        case HT.HT_CAPTION:
                            isMoving = true;
                            break;
                    }
                    break;
                #endregion

                #region 放缩窗体时临时禁用圆角减少闪烁
                case WM.WM_ENTERSIZEMOVE://ResizeBegin
                    if(!isMoving)
                    {
                        tempRadius = this.CornerRadius;
                        this.CornerRadius = 0;
                    }
                    break;
                case WM.WM_EXITSIZEMOVE://ResizeEnd
                    if(!isMoving)
                    {
                        this.CornerRadius = tempRadius;
                    }
                    break;
                #endregion

                case WM.WM_SYSCOMMAND:
                    switch((SC)m.WParam)
                    {
                        //解决控件过多从最小化还原重绘卡顿问题
                        case SC.SC_RESTORE:
                            this.SuspendLayout();
                            base.WndProc(ref m);
                            this.ResumeLayout();
                            return;
                        //通过窗口系统菜单、任务栏、任务视图、Alt+Tab视图等系统命令关闭窗体
                        case SC.SC_CLOSE:
                            if(!this.SysClosable) return;
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= (int)WS.WS_MINIMIZEBOX;//无边框点击任务栏还原窗口
                if(!DesignMode) cp.ExStyle |= (int)WSEX.WS_EX_LAYERED;//分层窗口
                return cp;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch((WM)msg.Msg)
            {
                case WM.WM_KEYDOWN:
                case WM.WM_SYSKEYDOWN:
                    //ESC键关闭窗体
                    if(keyData == Keys.Escape)
                    {
                        if(this.EscToHide) this.Hide();
                        if(this.EscToClose) this.Close();
                    }
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>设置圆角</summary>
        private void SetRoundBorder()
        {
            Rectangle rect = new Rectangle(-1, -1, this.Width + 1, this.Height + 1);
            using(GraphicsPath path = RoundMaker.GetRoundPath(rect, this.CornerRadius))
            {
                this.Region = new Region(path);
            }
        }

        /// <summary>绘制阴影</summary>
        private void DrawShadow()
        {
            if(DesignMode || !this.ShowShadow) return;
            using(Bitmap bitmap = new Bitmap(ShadowForm.Width, ShadowForm.Height))
            using(Graphics g = Graphics.FromImage(bitmap))
            {
                if(this.WindowState == FormWindowState.Normal)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    int alhpa = this.ShadowColor.A;
                    int d = this.CornerRadius * 2;
                    int count = alhpa / this.ShadowThickness;
                    float penWidth = this.ShadowThickness / (float)count;
                    for(int i = 0; i < count; i++)
                    {
                        RectangleF rect = ShadowForm.ClientRectangle;
                        float w = penWidth * i;
                        rect.Inflate(-w, -w);
                        Color color = Color.FromArgb(alhpa * i / (count - 1), this.ShadowColor);
                        using(Pen pen = new Pen(color, penWidth))
                        using(GraphicsPath path = RoundMaker.GetRoundPath(rect, this.CornerRadius))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
                ShadowForm.UpdateImage(bitmap);
            }
        }
    }
}