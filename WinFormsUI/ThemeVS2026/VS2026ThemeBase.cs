namespace WeifenLuo.WinFormsUI.ThemeVS2026
{
    using ThemeVS2012;
    using ThemeVS2013;
    using Docking;

    /// <summary>
    /// Visual Studio 2026 theme base.
    /// </summary>
    public abstract class VS2026ThemeBase : ThemeBase
    {
        public VS2026ThemeBase(byte[] resources)
        {
            ColorPalette = new DockPanelColorPalette(new VS2026PaletteFactory(resources));
            Skin = new DockPanelSkin();
            PaintingService = new PaintingService();
            ImageService = new ImageService(this);
            ToolStripRenderer = new VisualStudioToolStripRenderer(ColorPalette)
            {
                UseGlassOnMenuStrip = false,
            };
            Measures.SplitterSize = 6;
            Measures.AutoHideSplitterSize = 3;
            Measures.DockPadding = 6;
            Measures.AutoHideTabLineWidth = 3;
            ShowAutoHideContentOnHover = false;
            Extender.AutoHideStripFactory = new VS2026AutoHideStripFactory();
            Extender.AutoHideWindowFactory = new VS2012AutoHideWindowFactory();
            Extender.DockPaneFactory = new VS2026DockPaneFactory();
            Extender.DockPaneCaptionFactory = new VS2026DockPaneCaptionFactory();
            Extender.DockPaneStripFactory = new VS2026DockPaneStripFactory();
            Extender.DockPaneSplitterControlFactory = new VS2013DockPaneSplitterControlFactory();
            Extender.WindowSplitterControlFactory = new VS2013WindowSplitterControlFactory();
            Extender.DockWindowFactory = new VS2026DockWindowFactory();
            Extender.PaneIndicatorFactory = new VS2012PaneIndicatorFactory();
            Extender.PanelIndicatorFactory = new VS2012PanelIndicatorFactory();
            Extender.DockOutlineFactory = new VS2012DockOutlineFactory();
            Extender.DockIndicatorFactory = new VS2012DockIndicatorFactory();
        }

        public override void CleanUp(DockPanel dockPanel)
        {
            PaintingService.CleanUp();
            base.CleanUp(dockPanel);
        }
    }
}
