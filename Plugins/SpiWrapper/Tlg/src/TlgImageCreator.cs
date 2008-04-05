using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using Oog.Plugin;
using SpiUtility;

namespace OogTlg {
  public class TlgImageCreator : IImageCreator {

    public Image GetImage(Stream data) {
      if (!data.CanSeek) return null;
      if (!IsSupported(data)) return null;

      if (data is BytePointerStream) {
        return CreateImageFromPointer(data as BytePointerStream);
      }
      else {
        return CreateImageFromStream(data);
      }
    }

    private unsafe bool IsSupported(Stream data) {

      byte[] header = new byte[1024*2];
      ReadHeader(header, data);

      fixed (byte* p = header) {
        int result = IsSupported("", (uint)p);
        return result != 0;
      }
    }

    private void ReadHeader(byte[] header, Stream data) {
      int count = data.Read(header, 0, 2048);
      for (int i = count; i < 2048; i++) {
        header[i] = 0;
      }

      data.Seek(0, SeekOrigin.Begin);

      return;
    }

    private unsafe Image CreateImageFromPointer(BytePointerStream data) {
      byte* buf = *data.Handle;
      int len = (int)data.Length;
      ApiFlags flag = ApiFlags.InputFromImage;
      BITMAPINFO** pHBInfo = (BITMAPINFO**)0;
      byte** pHBm = (byte**)0;
      ProgressCallback lpPrgressCallback = delegate { return 0; };

      Bitmap temp = null;

      try {
        if (GetPicture(buf, len, flag, out pHBInfo, out pHBm, lpPrgressCallback, 0) == ErrorCode.Scceeded) {
          BITMAPINFO info = **pHBInfo;
          int width = info.bmiHeader.biWidth;
          int height = info.bmiHeader.biHeight;
          int stride = (info.bmiHeader.biBitCount/8) * width;
          PixelFormat format = GetFormat(info.bmiHeader.biBitCount);
          temp = new Bitmap(width, height, stride, format, (IntPtr)(*pHBm));
          SetPalette(temp.Palette, info);
          Bitmap bmp = new Bitmap(temp);

          temp.Dispose();
          LocalFree((IntPtr)pHBInfo);
          LocalFree((IntPtr)pHBm);

          bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);


          return bmp;
        }
        else {
          return null;
        }
      }
      catch {
        return null;
      }
      finally {
        if (temp != null) temp.Dispose();
        if ((IntPtr)pHBInfo != IntPtr.Zero) LocalFree((IntPtr)pHBInfo);
        if ((IntPtr)pHBm != IntPtr.Zero) LocalFree((IntPtr)pHBm);
      }
    }

    private unsafe Image CreateImageFromStream(Stream data) {
      BinaryReader reader = new BinaryReader(data);
      byte[] byteData = reader.ReadBytes((int)data.Length);
      reader.Close();

      fixed (byte* buf = byteData) {
        int len = (int)data.Length;
        ApiFlags flag = ApiFlags.InputFromImage;
        BITMAPINFO** pHBInfo = (BITMAPINFO**)0;
        byte** pHBm = (byte**)0;
        ProgressCallback lpPrgressCallback = delegate { return 0; };

        Bitmap temp = null;

        try {
          if (GetPicture(buf, len, flag, out pHBInfo, out pHBm, lpPrgressCallback, 0) == ErrorCode.Scceeded) {
            BITMAPINFO info = **pHBInfo;
            int width = info.bmiHeader.biWidth;
            int height = info.bmiHeader.biHeight;
            int stride = (info.bmiHeader.biBitCount/8) * width;
            PixelFormat format = GetFormat(info.bmiHeader.biBitCount);
            temp = new Bitmap(width, height, stride, format, (IntPtr)(*pHBm));
            SetPalette(temp.Palette, info);

            Bitmap bmp = new Bitmap(temp);


            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bmp;
          }
          else {
            return null;
          }
        }
        catch {
          return null;
        }
        finally {
          if (temp != null) temp.Dispose();
          if ((IntPtr)pHBInfo != IntPtr.Zero) LocalFree((IntPtr)pHBInfo);
          if ((IntPtr)pHBm != IntPtr.Zero) LocalFree((IntPtr)pHBm);
        }
      }
    }

    private PixelFormat GetFormat(ushort bitCount) {
      switch (bitCount) {
      case 0:
        return PixelFormat.DontCare;
      case 1:
        return PixelFormat.Format1bppIndexed;
      case 4:
        return PixelFormat.Format4bppIndexed;
      case 8:
        return PixelFormat.Format8bppIndexed;
      case 16:
        return PixelFormat.Format16bppRgb555;
      case 24:
        return PixelFormat.Format24bppRgb;
      case 32:
        return PixelFormat.Format32bppRgb;
      default:
        return PixelFormat.Undefined;
      }
    }

    private unsafe void SetPalette(ColorPalette colorPalette, BITMAPINFO info) {
      ushort bitCount = info.bmiHeader.biBitCount;
      RGBQUAD rgb = new RGBQUAD();
      int* pRgb = info.bmiColors;
      if (bitCount==1 || bitCount==4 || bitCount==8) {
        for (int i = 0; i < colorPalette.Entries.Length; i++) {
          int value = *(pRgb+i);
          rgb.value = value;
          int r = rgb.rgbRed;
          int g = rgb.rgbGreen;
          int b = rgb.rgbBlue;
          colorPalette.Entries[i] = Color.FromArgb(r, g, b);
        }
      }
    }

    public string[] SupportedExtensions {
      get { return new string[] { ".tlg" }; }
    }


    [DllImport(@"if_tlg.spi")]
      static extern int IsSupported(string filename, uint dw);

    [DllImport(@"if_tlg.spi")]
    static unsafe extern int GetPictureInfo(
      byte* buf,
      int len,
      ApiFlags flag,
      [In, Out] ref PictureInfo lpInfo
      );

    [DllImport(@"if_tlg.spi")]
    static unsafe extern ErrorCode GetPicture(
      byte* buf,
      int len,
      ApiFlags flag,
      [Out] out BITMAPINFO** pHBInfo,
      [Out] out byte** pHBm,
      ProgressCallback lpPrgressCallback,
      int lData
      );

    [DllImport("kernel32.dll", SetLastError=true)]
    static extern IntPtr LocalFree(IntPtr hMem);
  }
}
