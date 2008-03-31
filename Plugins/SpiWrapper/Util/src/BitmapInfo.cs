using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SpiUtility {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BITMAPINFO {
        public BITMAPINFOHEADER bmiHeader;

        public fixed int bmiColors[1];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct RGBQUAD {
        [FieldOffset(0)]
        public int value;
        [FieldOffset(0)]
        public byte rgbBlue;
        [FieldOffset(1)]
        public byte rgbGreen;
        [FieldOffset(2)]
        public byte rgbRed;
        [FieldOffset(3)]
        public byte rgbReserved;
    }
}
