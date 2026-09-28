using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace WeifenLuo.WinFormsUI.ThemeVS2026
{
    [ToolboxItem(false)]
    public class VS2026DockPane : DockPane
    {
        public VS2026DockPane(IDockContent content, DockState visibleState, bool show)
            : base(content, visibleState, show)
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
        public VS2026DockPane(IDockContent content, FloatWindow floatWindow, bool show)
            : base(content, floatWindow, show)
        {
        }

        public VS2026DockPane(IDockContent content, DockPane previousPane, DockAlignment alignment, double proportion, bool show)
            : base(content, previousPane, alignment, proportion, show)
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
        public VS2026DockPane(IDockContent content, Rectangle floatWindowBounds, bool show)
            : base(content, floatWindowBounds, show)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var backcolor = DockPanel.Theme.ColorPalette.ToolWindowBackground;
            e.Graphics.FillRectangle(DockPanel.Theme.PaintingService.GetBrush(backcolor), e.ClipRectangle);
            DrawOutline(e);
            TabStripControl.Invalidate();
        }

        private void DrawOutline(PaintEventArgs e)
        {
            bool active = IsActive();
            var color = active ? DockPanel.Theme.ColorPalette.ToolWindowBorderFocus : DockPanel.Theme.ColorPalette.ToolWindowBorder; // && globalFocus
            var pen = DockPanel.Theme.PaintingService.GetPen(color);
            int left = this.ContentRectangle.X - 1;
            int top = this.ContentRectangle.Y - 1;
            int right = left + this.ContentRectangle.Width + 1;
            int bottom = top + e.ClipRectangle.Height;
            e.Graphics.DrawLine(pen, left, top, left, bottom);
            e.Graphics.DrawLine(pen, left, bottom, right, bottom);
            e.Graphics.DrawLine(pen, right, bottom, right, top);
            System.Diagnostics.Debug.WriteLine($"Paint DockPane {this.ActiveContent?.DockHandler?.TabText} -> ACTIVE: {active} / Type: {this.Appearance}");
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate();
        }

        protected override bool HasCaption
        {
            get
            {
                if (DockState == DockState.Document ||
                    DockState == DockState.Hidden ||
                    DockState == DockState.Unknown)
                    return false;
                else
                    return true;
            }
        }

        protected internal override Rectangle ContentRectangle
        {
            get
            {
                var rect = base.ContentRectangle;
                if (DockState == DockState.Document || Contents.Count == 1)
                {
                    rect.Height -= 2;
                    //rect.Y++;
                }
                rect.Width -= 2;
                rect.X++;
                return rect;
            }
        }
    }
}
