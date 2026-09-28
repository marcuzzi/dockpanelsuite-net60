namespace WeifenLuo.WinFormsUI.Docking
{
    using ThemeVS2026;

    /// <summary>
    /// Visual Studio 2026 Dark theme.
    /// </summary>
    public class VS2026DarkTheme : VS2026ThemeBase
    {
        public VS2026DarkTheme()
            : base(Decompress(Resources.vs2026dark_vstheme))
        {
        }
    }
}
