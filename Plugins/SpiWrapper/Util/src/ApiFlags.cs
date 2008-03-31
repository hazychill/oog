using System;

namespace SpiUtility {
  public enum ApiFlags : uint {
    InputFromFile  = 0x0000,
    InputFromImage = 0x0001,
    OutputToFile   = 0x0000,
    OutputToImage  = 0x0100,
    IgnoreCase     = 0x0080
  }
}