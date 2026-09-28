using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.ThemeVS2026;

public static class DwmWindowHelper
{
    private const int DWMWA_BORDER_COLOR = 34;

    // Valores especiales admitidos por DWM
    private const int DWMWA_COLOR_DEFAULT = unchecked((int)0xFFFFFFFF);
    private const int DWMWA_COLOR_NONE = unchecked((int)0xFFFFFFFE);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute);

    public static bool SetBorderColor(Form form, System.Drawing.Color color)
    {
        if (form == null)
            throw new ArgumentNullException(nameof(form));

        if (!form.IsHandleCreated)
            throw new InvalidOperationException(
                "El Form debe tener el handle creado.");

        // DWM usa COLORREF: 0x00BBGGRR
        int colorRef =
            color.R |
            (color.G << 8) |
            (color.B << 16);

        int result = DwmSetWindowAttribute(
            form.Handle,
            DWMWA_BORDER_COLOR,
            ref colorRef,
            sizeof(int));

        return result == 0;
    }

    public static bool ResetBorderColor(Form form)
    {
        int value = DWMWA_COLOR_DEFAULT;

        return DwmSetWindowAttribute(
            form.Handle,
            DWMWA_BORDER_COLOR,
            ref value,
            sizeof(int)) == 0;
    }

    public static bool HideBorder(Form form)
    {
        int value = DWMWA_COLOR_NONE;

        return DwmSetWindowAttribute(
            form.Handle,
            DWMWA_BORDER_COLOR,
            ref value,
            sizeof(int)) == 0;
    }
}