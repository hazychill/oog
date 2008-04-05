using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Oog.Plugin;
using SpiUtility;

namespace OogXp3 {
  public class Xp3Extractor : IExtractor {
    string filename;
    string[] names;
    Dictionary<string, DataLocation> dataLocations;

    public Xp3Extractor(string filename) {
      this.filename = filename;
    }

    public string[] GetNames() {
      if (names == null) names = CreateNames();

      return names;
    }

    private unsafe string[] CreateNames() {
      if (!IsSupported()) return new string[0];

      fileInfo** lphInf;
      if (GetArchiveInfo(filename, 0, ApiFlags.InputFromFile, out lphInf) == ErrorCode.Scceeded) {
        dataLocations = new Dictionary<string, DataLocation>();

        fileInfo* pf = *lphInf;

        while (true) {
          if (*(pf->method) == 0) break;

          string path = BytePointerToString(pf->path, 200);
          string lastName = BytePointerToString(pf->filename, 200);
          string name = Path.Combine(path, lastName);
          uint offset = pf->position;
          uint length = pf->filesize;

          dataLocations.Add(name, new DataLocation(offset, length));

          pf++;
        }
        LocalFree((IntPtr)lphInf);

        string[] names = new string[dataLocations.Keys.Count];
        int i = 0;
        foreach (string name in dataLocations.Keys) {
          names[i] = name;
          i++;
        }
        return names;
      }
      else {
        return new string[0];
      }

    }

    private unsafe bool IsSupported() {

      byte[] header = new byte[1024*2];
      ReadHeader(header);

      fixed (byte* p = header) {
        int result = IsSupported(filename, (uint)p);
        return result != 0;
      }
    }

    private void ReadHeader(byte[] header) {
      using (FileStream fs = File.OpenRead(filename)) {
        int count = fs.Read(header, 0, 1024);
        if (count == 1024) {
          count += fs.Read(header, 1024, 1024);
        }
        for (int i = count; i < 1024*2; i++) {
          header[i] = 0;
        }

        return;
      }
    }

    static unsafe string BytePointerToString(byte* p, int length) {
      List<byte> buffer = new List<byte>();
      for (int i = 0; i < length; i++) {
        byte b = *(p+i);
        if (b != 0) {
          buffer.Add(b);
        }
        else break;
      }

      return Encoding.Default.GetString(buffer.ToArray());
    }

    public unsafe Stream GetData(string name) {
      DataLocation dl;
      if (!dataLocations.TryGetValue(name, out dl)) return null;

      uint offset = dl.Offset;
      uint len = dl.Length;
      ApiFlags flag = ApiFlags.InputFromFile | ApiFlags.OutputToImage;
      byte** dest;
      ProgressCallback progressCallback = delegate { return 0; };

      if (GetFile(filename, (int)offset, out dest, flag, progressCallback, 0) == ErrorCode.Scceeded) {
        CloseStreamDelegate closeAction = delegate { LocalFree((IntPtr)dest); };
        return new BytePointerStream(dest, len, closeAction);
      }
      else {
        return null;
      }
    }

    public void Close() {}

    public void Dispose() {
      Close();
    }

    [DllImport(@"ax_xp3.spi")]
      static unsafe extern ErrorCode GetArchiveInfo(string buf, int len, ApiFlags flag, [Out] out fileInfo** lphInf);

    [DllImport(@"ax_xp3.spi")]
    static extern int IsSupported(string filename, uint dw);

    [DllImport(@"ax_xp3.spi")]
    static unsafe extern ErrorCode GetFile(string src, int len, [Out] out byte** dest, ApiFlags flag, ProgressCallback progressCallback, int lData);

    [DllImport("kernel32.dll", SetLastError=true)]
    static extern IntPtr LocalFree(IntPtr hMem);

  }
}