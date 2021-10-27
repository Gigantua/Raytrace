using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine.Hardware
{
    static class Screen
    {
        public static DEVMODE QueryCurrentSettings()
        {
            DEVMODE mode = new DEVMODE();
            bool ok = EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode);
            return mode;
        }

        public static DEVMODE[] QueryAllSettings()
        {
            List<DEVMODE> modes = new List<DEVMODE>();

            int i = 0;
            DEVMODE mode = new DEVMODE();
            while (EnumDisplaySettings(null, i, ref mode))
            {
                modes.Add(mode);
                i++;
            }
            return modes.ToArray();
        }

        //Highest resolution with highest refresh rate
        public static DEVMODE QueryHighestSetting()
        {
            DEVMODE[] ALL = QueryAllSettings();
            int maxwidth = ALL.Max(x => x.Width);
            return ALL.Where(x => x.Width == maxwidth).OrderByDescending(x => x.RefreshRate).First();
        }

        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(
              string deviceName, int modeNum, ref DEVMODE devMode);
        const int ENUM_CURRENT_SETTINGS = -1;

        const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int Width;
            public int Height;
            public int dmDisplayFlags;
            public int RefreshRate;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;

            public override string ToString()
            {
                return $"{Width}x{Height} @ {RefreshRate}fps";
            }
        }
    }
}
