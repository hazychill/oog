using System;

namespace SpiUtility {
  public enum ErrorCode : int {
    Scceeded=0,
    NotImplemented=-1,
    Canceled=1,
    UnknownFormat=2,
    DataCorrupted=3,
    MemoryAllocationError=4,
    MemoryError=5,
    UnableToReadFile=6,
    InternalError=8
  }
}