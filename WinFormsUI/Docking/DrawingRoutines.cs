﻿using System.Drawing;
using System.Drawing.Drawing2D;

namespace WeifenLuo.WinFormsUI.Docking
{
    public static class DrawingRoutines
    {
        public static void SafelyDrawLinearGradient(this Rectangle rectangle, Color startColor, Color endColor,
            LinearGradientMode mode, Graphics graphics, Blend blend = null)
        {
            if (rectangle.Width > 0 && rectangle.Height > 0)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rectangle, startColor, endColor, mode))
                {
                    if (blend != null)
                    {
                        brush.Blend = blend;
                    }

                    graphics.FillRectangle(brush, rectangle);
                }
            }
        }

        public static void SafelyDrawLinearGradientF(this RectangleF rectangle, Color startColor, Color endColor,
            LinearGradientMode mode, Graphics graphics)
        {
            if (rectangle.Width > 0 && rectangle.Height > 0)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rectangle, startColor, endColor, mode))
                {
                    graphics.FillRectangle(brush, rectangle);
                }
            }
        }
    }
}
