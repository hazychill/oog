using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Oog.Plugin;

namespace Oog.Plugin {
  public class RarExtractor : IExtractor {

    [DllImport("unrar32.dll")]
    private static extern bool UnrarGetRunning();

    [DllImport("unrar32.dll")]
    private static extern bool UnrarSetCursorMode(bool _CursorMode);

    [DllImport("unrar32.dll")]
    private static extern int Unrar(IntPtr _hwnd, string _szCmdLine, StringBuilder _szOutput, uint _dwSize);
    private static int Unrar(string _szCmdLine) {
      IntPtr hwnd = IntPtr.Zero;
      return Unrar(hwnd, _szCmdLine, null, 0);
    }

    [DllImport("unrar32.dll")]
      private static extern bool UnrarCheckArchive(string _szFileName, int _iMode);
    private static bool UnrarCheckArchive(string _szFileName) {
      return UnrarCheckArchive(_szFileName, 0);
    }

    [DllImport("unrar32.dll")]
      private static extern IntPtr UnrarOpenArchive(IntPtr _hwnd, string _szFileName, uint _dwMode);
    private static IntPtr UnrarOpenArchive(string _szFileName) {
      IntPtr hwnd = IntPtr.Zero;
      return UnrarOpenArchive(hwnd, _szFileName, 0);
    }

    [DllImport("unrar32.dll")]
      private static extern int UnrarCloseArchive(IntPtr _harc);

    [DllImport("unrar32.dll")]
    private static extern int UnrarFindFirst(IntPtr _harc, string _szWildName, ref INDIVIDUALINFO lpSubInfo);

    [DllImport("unrar32.dll")]
    private static extern int UnrarFindNext(IntPtr _harc, ref INDIVIDUALINFO _lpSubInfo);

    const int FNAME_MAX32 = 512;

    [StructLayout(LayoutKind.Sequential)]
    private struct INDIVIDUALINFO {
      public uint   dwOriginalSize;
      public uint   dwCompressedSize;
      public uint   dwCRC;
      public uint   uFlag;
      public uint   uOSType;
      public ushort wRatio;
      public ushort wDate;
      public ushort wTime;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst=FNAME_MAX32 + 1)]
      public string szFileName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst=3)]
      public string dummy1;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]
      public string szAttribute;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]
      public string szMode;
    }




    string arcFilePath;
    string[] names;

    object lockObj;

    public RarExtractor(string path) {
      arcFilePath = path;

      if (UnrarCheckArchive(arcFilePath)) {
        names = InitializeNames();
      }
      else {
        names = new string[0];
      }

      lockObj = new object();
    }

    private string[] InitializeNames() {
      IntPtr harc = IntPtr.Zero;
      try {
        List<string> nameList = new List<string>();
        UnrarSetCursorMode(false);
        harc = UnrarOpenArchive(arcFilePath);
        INDIVIDUALINFO info = new INDIVIDUALINFO();

        int result = UnrarFindFirst(harc, "*", ref info);
        if (result == 0) {
          do {
            nameList.Add(info.szFileName);
            result = UnrarFindNext(harc, ref info);
          } while (result == 0);
        }

        return nameList.ToArray();
      }
      catch {
        return new string[0];
      }
      finally {
        if (harc != IntPtr.Zero) {
          try {
            UnrarCloseArchive(harc);
          }
          catch {}
        }
      }
    }

    public string[] GetNames() {
      return names;
    }

    public Stream GetData(string name) {
      string tempFolder = GetTempFolder();
      string tempPath = Path.Combine(tempFolder, name.Replace('/', '\\'));
      Console.WriteLine(tempFolder);
      Console.WriteLine(tempPath);

      if (File.Exists(tempPath)) {
        try {
          return File.OpenRead(tempPath);
        }
        catch {
          return null;
        }
      }

      string cmd = string.Format("-x -o- -q \"{0}\" \"{1}\" \"{2}\"", arcFilePath, tempFolder, name);
      int result = -1;
      lock (lockObj) {
        result = Unrar(cmd);
      }

      if (result==0 && File.Exists(tempPath)) {
        try {
          return File.OpenRead(tempPath);
        }
        catch {
          return null;
        }
      }
      else {
        return null;
      }

    }

    private string GetTempFolder() {
      string oogTempFolder = PluginHelper.GetOogTempFolder();
      string fileName = Path.GetFileName(arcFilePath);
      string tempFolder = Path.Combine(oogTempFolder, fileName);
      if (!tempFolder.EndsWith("\\")) tempFolder += "\\";
      return tempFolder;
    }

    public void Close() {}
    void IDisposable.Dispose() { Close(); }
  }
}