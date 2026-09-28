using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public static class DrawHelper
    {
        public static Point RtlTransform(Control control, Point point)
        {
            if (control.RightToLeft != RightToLeft.Yes)
                return point;
            else
                return new Point(control.Right - point.X, point.Y);
        }

        public static Rectangle RtlTransform(Control control, Rectangle rectangle)
        {
            if (control.RightToLeft != RightToLeft.Yes)
                return rectangle;
            else
                return new Rectangle(control.ClientRectangle.Right - rectangle.Right, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static GraphicsPath GetRoundedCornerTab(GraphicsPath graphicsPath, Rectangle rect, bool upCorner)
        {
            if (graphicsPath == null)
                graphicsPath = new GraphicsPath();
            else
                graphicsPath.Reset();

            int curveSize = 8;
            int halfCurveSize = curveSize / 2;
            if (upCorner)
            {
                graphicsPath.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + halfCurveSize);
                graphicsPath.AddArc(new Rectangle(rect.Left, rect.Top, curveSize, curveSize), 180, 90);
                graphicsPath.AddLine(rect.Left + halfCurveSize, rect.Top, rect.Right - halfCurveSize, rect.Top);
                graphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Top, curveSize, curveSize), -90, 90);
                graphicsPath.AddLine(rect.Right, rect.Top + halfCurveSize, rect.Right, rect.Bottom);
            }
            else
            {
                graphicsPath.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom - halfCurveSize);
                graphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize), 0, 90);
                graphicsPath.AddLine(rect.Right - halfCurveSize, rect.Bottom, rect.Left + halfCurveSize, rect.Bottom);
                graphicsPath.AddArc(new Rectangle(rect.Left, rect.Bottom - curveSize, curveSize, curveSize), 90, 90);
                graphicsPath.AddLine(rect.Left, rect.Bottom - halfCurveSize, rect.Left, rect.Top);
            }

            return graphicsPath;
        }

        public static GraphicsPath GetRoundedCornerTab2026(GraphicsPath graphicsPath, Rectangle rect, int curveSize, bool upCorner, bool leftCurve = false, bool rightCurve = false)
        {
            if (graphicsPath == null)
                graphicsPath = new GraphicsPath();
            else
                graphicsPath.Reset();

            int halfCurveSize = curveSize / 2;
            if (upCorner)
            {
                if (leftCurve)
                {
                    graphicsPath.AddArc(new Rectangle(rect.Left - curveSize, rect.Bottom - curveSize - 1, curveSize, curveSize), 90, -90);
                }
                else
                {
                    graphicsPath.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + halfCurveSize);
                }
                graphicsPath.AddArc(new Rectangle(rect.Left, rect.Top, curveSize, curveSize), 180, 90);
                graphicsPath.AddLine(rect.Left + halfCurveSize, rect.Top, rect.Right - halfCurveSize, rect.Top);
                graphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Top, curveSize, curveSize), -90, 90);
                if (rightCurve)
                {
                    graphicsPath.AddArc(new Rectangle(rect.Right, rect.Bottom - curveSize - 1, curveSize, curveSize), 180, -90);
                }
                else
                {
                    graphicsPath.AddLine(rect.Right, rect.Top + halfCurveSize, rect.Right, rect.Bottom);
                }
            }
            else
            {
                if (rightCurve)
                {
                    graphicsPath.AddArc(new Rectangle(rect.Right, rect.Top, curveSize, curveSize), 270, -90);
                    //graphicsPath.AddLine(rect.Right, rect.Top - curveSize, rect.Right, rect.Bottom - halfCurveSize);
                }
                else
                {
                    graphicsPath.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom - halfCurveSize);
                }
                graphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize), 0, 90);
                graphicsPath.AddLine(rect.Right - halfCurveSize, rect.Bottom, rect.Left + halfCurveSize, rect.Bottom);
                graphicsPath.AddArc(new Rectangle(rect.Left, rect.Bottom - curveSize, curveSize, curveSize), 90, 90);
                if (leftCurve)
                {
                    //graphicsPath.AddLine(rect.Left, rect.Bottom - halfCurveSize, rect.Left, rect.Top - curveSize);
                    graphicsPath.AddArc(new Rectangle(rect.Left - curveSize, rect.Top, curveSize, curveSize), 0, -90);
                }
                else
                {
                    graphicsPath.AddLine(rect.Left, rect.Bottom - halfCurveSize, rect.Left, rect.Top);
                }
            }

            return graphicsPath;
        }

        public static GraphicsPath CalculateGraphicsPathFromBitmap(Bitmap bitmap)
        {
            return CalculateGraphicsPathFromBitmap(bitmap, Color.Empty);
        }

        // From http://edu.cnzz.cn/show_3281.html
        public static GraphicsPath CalculateGraphicsPathFromBitmap(Bitmap bitmap, Color colorTransparent)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            if (colorTransparent == Color.Empty)
                colorTransparent = bitmap.GetPixel(0, 0);

            for (int row = 0; row < bitmap.Height; row++)
            {
                int colOpaquePixel = 0;
                for (int col = 0; col < bitmap.Width; col++)
                {
                    if (bitmap.GetPixel(col, row) != colorTransparent)
                    {
                        colOpaquePixel = col;
                        int colNext = col;
                        for (colNext = colOpaquePixel; colNext < bitmap.Width; colNext++)
                            if (bitmap.GetPixel(colNext, row) == colorTransparent)
                                break;

                        graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
                        col = colNext;
                    }
                }
            }
            return graphicsPath;
        }

        public static int Balance(int length, int margin, int input, int lower, int upper)
        {
            return Max(Min(input, upper - length - margin), lower + margin);
        }

        private static int Min(int one, int other)
        {
            return one > other ? other : one;
        }

        private static int Max(int one, int other)
        {
            return one < other ? other : one;
        }
    }
}
