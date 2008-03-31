using System;

namespace SpiUtility {
  public struct DataLocation {
    uint offset;
    uint length;

    public DataLocation(uint offset, uint length) {
      this.offset = offset;
      this.length = length;
    }

    public uint Offset {
      get { return offset; }
      set { offset = value; }
    }

    public uint Length {
      get { return length; }
      set { length = value; }
    }
  }
}