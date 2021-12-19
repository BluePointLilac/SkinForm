using System.Drawing;
using System.Drawing.Drawing2D;

namespace MyCommon.MyMethods
{
    public static class RoundMaker
    {
        public static GraphicsPath GetRoundPath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if(radius == 0) path.AddRectangle(rect);
            else
            {
                float d = radius * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, -90, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
            }
            return path;
        }
    }
}