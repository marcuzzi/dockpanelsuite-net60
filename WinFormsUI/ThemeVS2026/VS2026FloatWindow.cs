using System;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;

namespace WeifenLuo.WinFormsUI.ThemeVS2026;

public class VS2026FloatWindow : FloatWindow
{
    internal protected VS2026FloatWindow(DockPanel dockPanel, DockPane pane): base(dockPanel, pane)
    {
    }

    internal protected VS2026FloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds): base(dockPanel, pane, bounds)
    {
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        DwmWindowHelper.SetBorderColor(this, Color.FromArgb(0, 122, 204));
    }
}