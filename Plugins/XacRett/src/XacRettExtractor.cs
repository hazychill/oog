using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Oog.Plugin;

namespace Oog.Plugin {
  public class XacRettExtractor : IExtractor {

    [DllImport("XacRett.dll")]
    private static extern bool XacRettGetRunning();

    [DllImport("XacRett.dll")]
    private static extern int XacRett(IntPtr _hwnd, string _szCmdLine, StringBuilder _szOutput, uint _dwSize);
    private static int XacRett(string _szCmdLine) {
      IntPtr hwnd = IntPtr.Zero;
      return XacRett(hwnd, _szCmdLine, null, 0);
    }

    [DllImport("XacRett.dll")]
    private static extern bool XacRettCheckArchive(string _szFileName, int _iMode);
    private static bool XacRettCheckArchive(string _szFileName) {
      return XacRettCheckArchive(_szFileName, 0);
    }

    [DllImport("XacRett.dll")]
    private static extern IntPtr XacRettOpenArchive(IntPtr _hwnd, string _szFileName, uint _dwMode);
    private static IntPtr XacRettOpenArchive(string _szFileName) {
      IntPtr hwnd = IntPtr.Zero;
      return XacRettOpenArchive(hwnd, _szFileName, 0);
    }

    [DllImport("XacRett.dll")]
    private static extern int XacRettCloseArchive(IntPtr _harc);

    [DllImport("XacRett.dll")]
    private static extern int XacRettFindFirst(IntPtr _harc, string _szWildName, ref INDIVIDUALINFO lpSubInfo);

    [DllImport("XacRett.dll")]
    private static extern int XacRettFindNext(IntPtr _harc, ref INDIVIDUALINFO _lpSubInfo);

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

    volatile object lockObj;

    public XacRettExtractor(string path) {
      arcFilePath = path;

      if (XacRettCheckArchive(arcFilePath)) {
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
        harc = XacRettOpenArchive(arcFilePath);
        INDIVIDUALINFO info = new INDIVIDUALINFO();

        int result = XacRettFindFirst(harc, "*", ref info);
        if (result == 0) {
          do {
            nameList.Add(info.szFileName);
            result = XacRettFindNext(harc, ref info);
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
            XacRettCloseArchive(harc);
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

      if (File.Exists(tempPath)) {
        try {
          return File.OpenRead(tempPath);
        }
        catch {
          return null;
        }
      }

      string cmd = string.Format("x -n1 -o1 \"{0}\" \"{1}\" \"{2}\"", arcFilePath, tempFolder, name);
      int result = -1;
      lock (lockObj) {
        result = XacRett(cmd);
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