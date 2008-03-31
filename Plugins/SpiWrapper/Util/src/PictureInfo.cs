using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SpiUtility {
    [StructLayout(LayoutKind.Sequential)]
    public struct PictureInfo {
        public int left;
        public int top;
        public int width;
        public int height;
        public ushort x_density;
        public ushort y_density;
        public short colorDepth;
        public IntPtr hInfo;
    }
}
