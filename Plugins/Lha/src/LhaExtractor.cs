using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Oog.Plugin;

namespace OogLha {
  public class LhaExtractor : IExtractor {

    [DllImport("UNLHA32.dll")]
    private static extern bool UnlhaGetRunning();

    [DllImport("UNLHA32.dll")]
    private static extern bool UnlhaSetCursorMode(bool _CursorMode);

    [DllImport("UNLHA32.dll")]
    private static extern int Unlha(IntPtr _hwnd, string _szCmdLine, StringBuilder _szOutput, uint _dwSize);
    private static int Unlha(string _szCmdLine) {
      IntPtr hwnd = IntPtr.Zero;
      return Unlha(hwnd, _szCmdLine, null, 0);
    }

    [DllImport("UNLHA32.dll")]
      private static extern bool UnlhaCheckArchive(string _szFileName, int _iMode);
    private static bool UnlhaCheckArchive(string _szFileName) {
      return UnlhaCheckArchive(_szFileName, 0);
    }

    [DllImport("UNLHA32.dll")]
      private static extern IntPtr UnlhaOpenArchive(IntPtr _hwnd, string _szFileName, uint _dwMode);
    private static IntPtr UnlhaOpenArchive(string _szFileName) {
      IntPtr hwnd = IntPtr.Zero;
      return UnlhaOpenArchive(hwnd, _szFileName, 0);
    }

    [DllImport("UNLHA32.dll")]
      private static extern int UnlhaCloseArchive(IntPtr _harc);

    [DllImport("UNLHA32.dll")]
    private static extern int UnlhaFindFirst(IntPtr _harc, string _szWildName, ref INDIVIDUALINFO lpSubInfo);

    [DllImport("UNLHA32.dll")]
    private static extern int UnlhaFindNext(IntPtr _harc, ref INDIVIDUALINFO _lpSubInfo);

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

    public LhaExtractor(string path) {
      arcFilePath = path;

      if (UnlhaCheckArchive(arcFilePath)) {
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
        UnlhaSetCursorMode(false);
        harc = UnlhaOpenArchive(arcFilePath);
        INDIVIDUALINFO info = new INDIVIDUALINFO();

        int result = UnlhaFindFirst(harc, "*", ref info);
        if (result == 0) {
          do {
            nameList.Add(info.szFileName);
            result = UnlhaFindNext(harc, ref info);
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
            UnlhaCloseArchive(harc);
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

      string cmd = string.Format("x -n1 -gm1 \"{0}\" \"{1}\" \"{2}\"", arcFilePath, tempFolder, name);
      int result = -1;
      lock (lockObj) {
        result = Unlha(cmd);
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