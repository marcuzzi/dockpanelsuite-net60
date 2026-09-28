using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    /// <summary>
    /// Dock window of Visual Studio 2026 dark theme.
    /// </summary>
    [ToolboxItem(false)]
    internal class VS2026DockWindow : DockWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VS2026DockWindow"/> class.
        /// </summary>
        /// <param name="dockPanel">The dock panel.</param>
        /// <param name="dockState">State of the dock.</param>
        public VS2026DockWindow(DockPanel dockPanel, DockState dockState) : base(dockPanel, dockState)
        {
        }

        public override Rectangle DisplayingRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle;
                if (DockState == DockState.DockLeft)
                    rect.Width -= DockPanel.Theme.Measures.SplitterSize;
                else if (DockState == DockState.DockRight)
                {
                    rect.X += DockPanel.Theme.Measures.SplitterSize;
                    rect.Width -= DockPanel.Theme.Measures.SplitterSize;
                }
                else if (DockState == DockState.DockTop)
                    rect.Height -= DockPanel.Theme.Measures.SplitterSize;
                else if (DockState == DockState.DockBottom)
                {
                    rect.Y += DockPanel.Theme.Measures.SplitterSize;
                    rect.Height -= DockPanel.Theme.Measures.SplitterSize;
                }

                return rect;
            }
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    // if DockWindow is document, draw the border
        //    if (DockState == DockState.Document)
        //    {
        //        using var borderPen = new Pen(DockPanel.Theme.ColorPalette.ToolWindowBorderFocus);
        //        e.Graphics.DrawRectangle(borderPen, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
        //    }

        //    base.OnPaint(e);
        //}
    }
}
