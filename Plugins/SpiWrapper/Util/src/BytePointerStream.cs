using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SpiUtility {

  public delegate void CloseStreamDelegate();

  public unsafe class BytePointerStream : Stream {
    byte** handle;
    long position;
    //byte* current;
    long length;

    CloseStreamDelegate closeAction;

    public unsafe BytePointerStream(byte** handle, uint length, CloseStreamDelegate closeAction) {
      this.handle = handle;
      this.position = 0;
      //this.current = pointer;
      this.length = length;

      this.closeAction = closeAction;
    }

    public override bool CanRead {
      get { return true; }
    }

    public override bool CanSeek {
      get { return true; }
    }

    public override bool CanWrite {
      get { return false; }
    }

    public override void Flush() { }

    public override long Length {
      get { return length; }
    }

    public override long Position {
      get {
        return position;
      }
      set {
        position = value;
      }
    }

    public override int Read(byte[] buffer, int offset, int count) {
      long readCount;
      if (position+count < length) {
        readCount = count;
      }
      else {
        readCount = length - position;
      }

      for (int i = 0; i < readCount; i++) {
        buffer[offset+i] = *(*handle + position + i);
      }

      position += readCount;
      return (int)readCount;
    }

    public override long Seek(long offset, SeekOrigin origin) {
      switch (origin) {
      case SeekOrigin.Begin:
        position = offset;
        break;
      case SeekOrigin.Current:
        position += offset;
        break;
      case SeekOrigin.End:
        position = (length-1) + offset;
        break;
      }

      return position;
    }

    public override void SetLength(long value) {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count) {
      throw new NotSupportedException();
    }

    public override void Close() {
      if (closeAction != null) {
        closeAction();
      }
    }

    public byte** Handle {
      get { return handle; }
    }
  }
}
