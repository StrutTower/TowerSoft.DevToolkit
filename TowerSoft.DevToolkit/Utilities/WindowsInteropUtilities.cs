using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TowerSoft.DevToolkit.Utilities {
    public class WindowsInteropUtilities {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        public static void SetDarkModeAndTitlebarColor(Window window) {
            nint handle = new WindowInteropHelper(window).Handle;

            if (DwmSetWindowAttribute(handle, (int)DWMWA.USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, [1], 4) != 0)
                _ = DwmSetWindowAttribute(handle, (int)DWMWA.USE_IMMERSIVE_DARK_MODE, [1], 4);

            //Color titleBackground = Color.FromArgb(42, 42, 46);
            Color titleBackground = Color.FromArgb(0, 88, 184);

            int[] caption = [int.Parse(ToBgr(titleBackground), System.Globalization.NumberStyles.HexNumber)];
            _ = DwmSetWindowAttribute(handle, (int)DWMWA.CAPTION_COLOR, caption, 4);


            //Color borderBackground = Color.FromArgb(42, 42, 180);

            //int[] border = [int.Parse(ToBgr(borderBackground), System.Globalization.NumberStyles.HexNumber)];
            //DwmSetWindowAttribute(handle, DWWMA_BORDER_COLOR, border, 4);

            _ = DwmSetWindowAttribute(handle, (int)DWMWA.WINDOW_CORNER_PREFERENCE, [(int)DWM_WINDOW_CORNER_PREFERENCE.DONOTROUND], 4);
        }

        private static string ToBgr(Color c) => $"{c.B:X2}{c.G:X2}{c.R:X2}";
    }

    public enum DWM_WINDOW_CORNER_PREFERENCE {
        DEFAULT = 0,
        DONOTROUND = 1,
        ROUND = 2,
        ROUNDSMALL = 3
    }

    public enum DWMWA {
        USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19,
        USE_IMMERSIVE_DARK_MODE = 20,
        WINDOW_CORNER_PREFERENCE = 33,
        BORDER_COLOR = 34,
        CAPTION_COLOR = 35,
        TEXT_COLOR = 36
    }
}
