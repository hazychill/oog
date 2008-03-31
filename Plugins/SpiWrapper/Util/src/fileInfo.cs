using System;
using System.Runtime.InteropServices;

namespace SpiUtility {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct fileInfo {
        public fixed byte method[8];
        public uint position;
        public uint compsize;
        public uint filesize;
        public int timestamp;
        public fixed byte path[200];
        public fixed byte filename[200];
        public uint crc;
    }
}
