using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public interface IImageService
    {
        Bitmap Dockindicator_PaneDiamond { get; }
        Bitmap Dockindicator_PaneDiamond_Fill { get; }
        Bitmap Dockindicator_PaneDiamond_Hotspot { get; }
        Bitmap DockIndicator_PaneDiamond_HotspotIndex { get; }
        Image DockIndicator_PanelBottom { get; }
        Image DockIndicator_PanelFill { get; }
        Image DockIndicator_PanelLeft { get; }
        Image DockIndicator_PanelRight { get; }
        Image DockIndicator_PanelTop { get; }
        Bitmap DockPane_Close { get; }
        Bitmap DockPane_List { get; }
        Bitmap DockPane_Dock { get; }
        Bitmap DockPaneActive_AutoHide { get; }
        Bitmap DockPane_Option { get; }
        Bitmap DockPane_OptionOverflow { get; }
        Bitmap DockPaneActive_Close { get; }
        Bitmap DockPaneActive_Dock { get; }
        Bitmap DockPaneActive_Option { get; }
        Bitmap DockPaneHover_Close { get; }
        Bitmap DockPaneHover_List { get; }
        Bitmap DockPaneHover_Dock { get; }
        Bitmap DockPaneActiveHover_AutoHide { get; }
        Bitmap DockPaneHover_Option { get; }
        Bitmap DockPaneHover_OptionOverflow { get; }
        Bitmap DockPanePress_Close { get; }
        Bitmap DockPanePress_List { get; }
        Bitmap DockPanePress_Dock { get; }
        Bitmap DockPanePress_AutoHide { get; }
        Bitmap DockPanePress_Option { get; }
        Bitmap DockPanePress_OptionOverflow { get; }
        Bitmap DockPaneActiveHover_Close { get; }
        Bitmap DockPaneActiveHover_Dock { get; }
        Bitmap DockPaneActiveHover_Option { get; }
        Image TabActive_Close { get; }
        Image TabInactive_Close { get; }
        Image TabLostFocus_Close { get; }
        Image TabHoverActive_Close { get; }
        Image TabHoverInactive_Close { get; }
        Image TabHoverLostFocus_Close { get; }
        Image TabPressActive_Close { get; }
        Image TabPressInactive_Close { get; }
        Image TabPressLostFocus_Close { get; }
    }

    public static class ImageServiceHelper
    {
        private static bool  DpiComputed = false;
        private static float DpiScale = 1.0f;

        /// <summary>
        /// Returns the current DPI scaling factor.
        /// </summary>
        /// <returns>The DPI scaling factor, or 1.0 if DPI awareness is disabled.</returns>
        /// <remarks>The DPI value is acquired once and stored in DpiScale</remarks>
        private static float GetDpiScale()
        {
            if (DpiComputed)
                return DpiScale;

            if (PatchController.EnableHighDpi != true)
            {
                DpiScale = 1.0f;
                DpiComputed = true;
                return DpiScale;
            }

            // Use a temporary control to get the current DPI
            using var control = new Control();
            using var graphics = control.CreateGraphics();
            DpiScale = graphics.DpiX / 96.0f;
            DpiComputed = true;
            return DpiScale;
        }

        /// <summary>
        /// Resizes a bitmap
        /// </summary>
        /// <param name="map">Original bitmap</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <returns>The resized bitmap</returns>
        public static Bitmap ResizeBitmap(Bitmap map, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using var g = Graphics.FromImage(result);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(map, 0, 0, width, height);
            return result;
        }

        /// <summary>
        /// Scales a value according to current DPI settings.
        /// </summary>
        /// <param name="value">The value to scale.</param>
        /// <returns>The scaled value.</returns>
        private static int ScaleValue(int value)
        {
            if (PatchController.EnableHighDpi == true)
            {
                float scale = GetDpiScale();
                return (int)(value * scale);
            }
            return value;
        }

        /// <summary>
        /// Gets images for tabs and captions.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="glyph"></param>
        /// <param name="background"></param>
        /// <param name="border"></param>
        /// <returns></returns>
        public static Bitmap GetImage(Bitmap mask, Color glyph, Color background, Color? border = null)
        {
            var width = mask.Width;
            var height = mask.Height;
            Bitmap input = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(input))
            {
                SolidBrush brush = new SolidBrush(glyph);
                gfx.FillRectangle(brush, 0, 0, width, height);
            }

            Bitmap output = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, input.Width, input.Height);
            var bitsMask = mask.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bitsInput = input.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bitsOutput = output.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < input.Height; y++)
                {
                    byte* ptrMask = (byte*)bitsMask.Scan0 + y * bitsMask.Stride;
                    byte* ptrInput = (byte*)bitsInput.Scan0 + y * bitsInput.Stride;
                    byte* ptrOutput = (byte*)bitsOutput.Scan0 + y * bitsOutput.Stride;
                    for (int x = 0; x < input.Width; x++)
                    {
                        ptrOutput[4 * x] = ptrInput[4 * x];           // blue
                        ptrOutput[4 * x + 1] = ptrInput[4 * x + 1];   // green
                        ptrOutput[4 * x + 2] = ptrInput[4 * x + 2];   // red
                        ptrOutput[4 * x + 3] = ptrMask[4 * x];        // alpha
                    }
                }
            }

            mask.UnlockBits(bitsMask);
            input.UnlockBits(bitsInput);
            output.UnlockBits(bitsOutput);
            input.Dispose();

            if (border == null)
            {
                border = background;
            }

            // Create the final image with potential DPI scaling
            var finalWidth = ScaleValue(width);
            var finalHeight = ScaleValue(height);
            Bitmap back = new Bitmap(finalWidth, finalHeight);
            using (Graphics gfx = Graphics.FromImage(back))
            {
                SolidBrush brush = new SolidBrush(background);
                SolidBrush brush2 = new SolidBrush(border.Value);
                gfx.FillRectangle(brush2, 0, 0, finalWidth, finalHeight);
                if (background != border.Value)
                {
                    var borderSize = ScaleValue(1);
                    gfx.FillRectangle(brush, borderSize, borderSize, finalWidth - 2 * borderSize, finalHeight - 2 * borderSize);
                }

                // Scale the output image when drawing if DPI scaling is enabled
                if (PatchController.EnableHighDpi == true && GetDpiScale() != 1.0f)
                {
                    gfx.DrawImage(output, new Rectangle(0, 0, finalWidth, finalHeight));
                }
                else
                {
                    gfx.DrawImageUnscaled(output, 0, 0);
                }
            }

            output.Dispose();
            return back;
        }

        public static Bitmap GetBackground(Color innerBorder, Color outerBorder, int width, IPaintingService painting)
        {
            Bitmap back = new Bitmap(width, width);
            using (Graphics gfx = Graphics.FromImage(back))
            {
                SolidBrush brush = painting.GetBrush(innerBorder);
                SolidBrush brush2 = painting.GetBrush(outerBorder);
                gfx.FillRectangle(brush2, 0, 0, width, width);
                gfx.FillRectangle(brush, 1, 1, width - 2, width - 2);
            }

            return back;
        }

        public static Bitmap GetLayerImage(Color color, int width, IPaintingService painting)
        {
            Bitmap back = new Bitmap(width, width);
            using (Graphics gfx = Graphics.FromImage(back))
            {
                SolidBrush brush = painting.GetBrush(color);
                gfx.FillRectangle(brush, 0, 0, width, width);
            }

            return back;
        }

        /// <summary>
        /// Gets images for docking indicators.
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetDockIcon(Bitmap maskArrow, Bitmap layerArrow, Bitmap maskWindow, Bitmap layerWindow, Bitmap maskBack, Color background, IPaintingService painting, Bitmap maskCore = null, Bitmap layerCore = null, Color? separator = null)
        {
            var width = ScaleValue(maskWindow.Width);
            var height = ScaleValue(maskWindow.Height);
            bool rescale = (width != maskWindow.Width || height != maskWindow.Height);

            // Window Contour bitmap
            Bitmap windowOut = null;
            if (rescale)
            {
                using var inputLayerWindow = ResizeBitmap(layerWindow, width, height);
                using var inputMaskWindow = ResizeBitmap(maskWindow, width, height);
                windowOut = MaskImages(inputLayerWindow, inputMaskWindow);
            }
            else
            {
                windowOut = MaskImages(layerWindow, maskWindow);
            }

            // Window Fill bitmap
            Bitmap coreOut = null;
            if (layerCore != null)
            {
                if (rescale)
                {
                    using var inputLayerCore = ResizeBitmap(layerCore, width, height);
                    using var inputMaskCore = ResizeBitmap(maskCore, width, height);
                    coreOut = MaskImages(inputLayerCore, inputMaskCore);
                }
                else
                {
                    coreOut = MaskImages(layerCore, maskCore);
                }
            }

            // Complete background bitmap: Background + window contour + window fill + separator
            Bitmap backOut = null;
            using (var inputBack = new Bitmap(width, height))
            {
                using (Graphics gfx = Graphics.FromImage(inputBack))
                {
                    SolidBrush brush = painting.GetBrush(background);
                    gfx.FillRectangle(brush, 0, 0, width, height);
                    gfx.DrawImageUnscaled(windowOut, 0, 0);
                    windowOut.Dispose();
                    if (layerCore != null)
                    {
                        gfx.DrawImageUnscaled(coreOut, 0, 0);
                        coreOut.Dispose();
                    }

                    if (separator != null)
                    {
                        Pen sep = painting.GetPen(separator.Value);
                        gfx.DrawRectangle(sep, 0, 0, width - 1, height - 1);
                    }
                }
                if (rescale)
                {
                    using var inputMaskBack = ResizeBitmap(maskBack, width, height);
                    backOut = MaskImages(inputBack, inputMaskBack);
                }
                else
                {
                    backOut = MaskImages(inputBack, maskBack);
                }
            }

            // Arrow bitmap
            Bitmap arrowOut = null;
            if (maskArrow != null)
            {
                if (rescale)
                {
                    using var inputLayerArrow = ResizeBitmap(layerArrow, width, height);
                    using var inputMaskArrow = ResizeBitmap(maskArrow, width, height);
                    arrowOut = MaskImages(inputLayerArrow, inputMaskArrow);
                }
                else
                {
                    arrowOut = MaskImages(layerArrow, maskArrow);
                }
            }

            // Output bitmap: background + arrow
            using (Graphics gfx = Graphics.FromImage(backOut))
            {
                if (arrowOut != null)
                {
                    gfx.DrawImageUnscaled(arrowOut, 0, 0);
                    arrowOut.Dispose();
                }
            }
            return backOut;
        }

        public static Bitmap MaskImages(Bitmap input, Bitmap maskArrow)
        {
            var width = input.Width;
            var height = input.Height;
            var rect = new Rectangle(0, 0, width, height);
            var arrowOut = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bitsMask = maskArrow.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bitsInput = input.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bitsOutput = arrowOut.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    byte* ptrMask = (byte*)bitsMask.Scan0 + y * bitsMask.Stride;
                    byte* ptrInput = (byte*)bitsInput.Scan0 + y* bitsInput.Stride;
                    byte* ptrOutput = (byte*)bitsOutput.Scan0 + y * bitsOutput.Stride;
                    for (int x = 0; x < width; x++)
                    {
                        ptrOutput[4 * x] = ptrInput[4 * x];           // blue
                        ptrOutput[4 * x + 1] = ptrInput[4 * x + 1];   // green
                        ptrOutput[4 * x + 2] = ptrInput[4 * x + 2];   // red
                        ptrOutput[4 * x + 3] = ptrMask[4 * x];        // alpha
                    }
                }
            }

            maskArrow.UnlockBits(bitsMask);
            input.UnlockBits(bitsInput);
            arrowOut.UnlockBits(bitsOutput);
            return arrowOut;
        }

        public static Bitmap GetDockImage(Bitmap icon, Bitmap background)
        {
            var result = new Bitmap(background);
            var offset = (background.Width - icon.Width) / 2;
            using (var gfx = Graphics.FromImage(result))
            {
                gfx.DrawImage(icon, offset, offset);
            }
            return result;
        }

        public static Bitmap CombineFive(Bitmap five, Bitmap bottom, Bitmap center, Bitmap left, Bitmap right, Bitmap top)
        {
            var result = new Bitmap(five);
            var cell = (result.Width - bottom.Width) / 2;
            var offset = (cell - bottom.Width) / 2;
            using (var gfx = Graphics.FromImage(result))
            {
                gfx.DrawImageUnscaled(top, cell, offset);
                gfx.DrawImageUnscaled(center, cell, cell);
                gfx.DrawImageUnscaled(bottom, cell, 2 * cell - offset);
                gfx.DrawImageUnscaled(left, offset, cell);
                gfx.DrawImageUnscaled(right, 2 * cell - offset, cell);
            }

            return result;
        }

        public static Point ScaledPoint(int x, int y)
        {
            float scale = GetDpiScale();
            
            if (scale == 1.0f)
                return new Point(x, y);
            else
                return new Point((int) (x * scale), (int) (y * scale));
        }

        public static Bitmap GetFiveBackground(Bitmap mask, Color innerBorder, Color outerBorder, IPaintingService painting)
        {
            int scaledWidth = ScaleValue(mask.Width);
            bool rescale = (scaledWidth != mask.Width);
            using (var input = GetLayerImage(innerBorder, scaledWidth, painting))
            {
                using (var gfx = Graphics.FromImage(input))
                {
                    var pen = painting.GetPen(outerBorder);
                    gfx.DrawLines(pen, new[]
                    {
                        ScaledPoint(36, 25), ScaledPoint(36, 0),
                        ScaledPoint(75, 0), ScaledPoint(75, 25)
                    });
                    gfx.DrawLines(pen, new[]
                    {
                        ScaledPoint(86, 36), ScaledPoint(111, 36),
                        ScaledPoint(11, 75), ScaledPoint(86, 75)
                    });
                    gfx.DrawLines(pen, new[]
                    {
                        ScaledPoint(75, 86), ScaledPoint(75, 111),
                        ScaledPoint(36, 111), ScaledPoint(36, 86)
                    });
                    gfx.DrawLines(pen, new[]
                    {
                        ScaledPoint(25, 75), ScaledPoint(0, 75),
                        ScaledPoint(0, 36), ScaledPoint(25, 36)
                    });
                    var pen2 = painting.GetPen(outerBorder, ScaleValue(2));
                    gfx.DrawLine(pen2, ScaledPoint(36, 25), ScaledPoint(25, 36));
                    gfx.DrawLine(pen2, ScaledPoint(75, 25), ScaledPoint(86, 36));
                    gfx.DrawLine(pen2, ScaledPoint(86, 75), ScaledPoint(75, 86));
                    gfx.DrawLine(pen2, ScaledPoint(36, 86), ScaledPoint(25, 75));
                }

                Bitmap output = null;
                if (rescale)
                {
                    using var scaledMask = ResizeBitmap(mask, scaledWidth, scaledWidth);
                    output = MaskImages(input, scaledMask);
                }
                else
                {
                    output = MaskImages(input, mask);
                }
                return output;
            }
        }
    }
}