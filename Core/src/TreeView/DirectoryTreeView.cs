using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Oog.Plugin;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Oog {
  class DirectoryTreeView : TreeView {

    private Dictionary<string, IExtractorFactory> extractorFactories;
    private Dictionary<string, int> imageIndexDic;
    ImageList imageList;

    private bool resetNodeRequired;

    public bool ResetNodeRequired {
      get { return resetNodeRequired; }
      set { resetNodeRequired = value; }
    }

    public DirectoryTreeView() {
      resetNodeRequired = false;
    }


    public Dictionary<string, IExtractorFactory> ExtractorFactories {
      get { return extractorFactories; }
      set { extractorFactories = value; }
    }

    public void RefreshTree() {
      InitializeImageIndex();
      this.Nodes.Clear();
      Array.ForEach(Environment.GetLogicalDrives(), delegate(string drive) {
        if (Directory.Exists(drive)) {
          TreeNode node = new TreeNode();
          node.Text = drive.TrimEnd('\\');
          node.ImageIndex = 0;
          this.Nodes.Add(node);

          //test
          //SetChildNodes(node);
          FindChildNode(node);
        }
      });

      if (this.Nodes.Count > 0) {
        this.SelectedNode = this.Nodes[0];
      }
    }

    private void InitializeImageIndex() {
      if (imageList != null) {
        imageList.Dispose();
      }
      imageList = new ImageList();
      imageList.ColorDepth = ColorDepth.Depth16Bit;
      if (imageIndexDic == null) {
        imageIndexDic = new Dictionary<string, int>();
      }

      imageIndexDic.Clear();


      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectoryTreeView));
      Bitmap bmp = ((System.Drawing.Bitmap)(resources.GetObject("icon_drive")));

      imageList.Images.Add(bmp);

      int index = 1;

      foreach (string ext in extractorFactories.Keys) {
        Image icon = null;
        IExtractorFactory factory;

        if (extractorFactories.TryGetValue(ext, out factory)) {
          icon = factory.Icon;
        }

        if (icon == null) {
          icon = extractorFactories[ext].Icon;
        }

        imageList.Images.Add(icon);
        imageIndexDic.Add(ext, index);

        index++;
      }
      this.ImageList = imageList;
    }

    static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    [DllImport("Kernel32")]
    static extern IntPtr FindFirstFile(string lpFileName, [In, Out] ref WIN32_FIND_DATA lpFindFileData);

    [DllImport("Kernel32")]
    static extern int FindNextFile(IntPtr hFindFile, [In, Out] ref WIN32_FIND_DATA lpFindFileData);

    [DllImport("Kernel32")]
    static extern int FindClose(IntPtr hFindFile);

    private IEnumerable<string>GetFiles(string dir) {
      WIN32_FIND_DATA findFileData = new WIN32_FIND_DATA();
      IntPtr h = FindFirstFile(dir, ref findFileData);
      if (h == INVALID_HANDLE_VALUE) {
        yield break;
      }
      try {
        do {
          if (findFileData.cFileName == "." || findFileData.cFileName == "..") {
            continue;
          }
          if ((findFileData.dwFileAttributes & 0x00000010u) == 0) {
            yield return findFileData.cFileName;
          }
          else {
            yield return string.Concat(findFileData.cFileName, "\\");
          }
        } while (FindNextFile(h, ref findFileData) != 0);
      }
      finally {
        if ((int)h > 0) {
          int ret = FindClose(h);
          if (ret == 0) {
            throw new Exception();
          }
        }
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WIN32_FIND_DATA {
      public uint     dwFileAttributes; // 属性
      public FILETIME ftCreateTime; // 作成日時
      public FILETIME ftLastAccessTime; // 最終アクセス日時
      public FILETIME ftLastWriteTime; // 最終更新日時
      public uint     nFileSizeHigh; // ファイルサイズ(上位32ビット)
      public uint     nFileSizeLow; // ファイルサイズ(下位32ビット)
      public uint     dwReserved0; // リパースタグ
      public uint     dwReserved1; // 予約
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)]
      public string   cFileName; // ファイル名
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst=14)]
      public string   cAlternateFileName; // 8.3形式のファイル名
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILETIME {
      public uint dwLowDateTime;
      public uint dwHighDateTime;
    }

    static readonly object findObj = new object();
    static readonly object setObj = new object();

    private void FindChildNode(TreeNode node) {
      if (node.Tag == findObj || node.Tag == setObj) {
        return;
      }
      node.Nodes.Clear();
      string find = string.Concat(node.FullPath, "\\*");
      foreach (string s in GetFiles(find)) {
        if (s.EndsWith("\\")) {
          node.Nodes.Add(new TreeNode());
          break;
        }
        else {
          string ext = Path.GetExtension(s).ToLower();
          if (!string.IsNullOrEmpty(ext)) {
            IExtractorFactory factory;
            if (extractorFactories.TryGetValue(ext, out factory)) {
              node.Nodes.Add(new TreeNode());
              break;
            }
          }
        }
      }
      node.Tag = findObj;
    }

    private void SetChildNodes(TreeNode node) {
      string path = string.Concat(node.FullPath, "\\");
      DirectoryInfo dir = new DirectoryInfo(path);
      if (dir.Exists) {
        try {
          TreeNode childNode;
          DirectoryInfo[] dirs = dir.GetDirectories();
          Array.Sort<DirectoryInfo>(dirs, delegate(DirectoryInfo x, DirectoryInfo y) {
            return x.Name.CompareTo(y.Name);
          });

          Array.ForEach<DirectoryInfo>(dirs, delegate(DirectoryInfo subDir) {
            childNode = new TreeNode();
            childNode.Text = subDir.Name;
            childNode.ImageIndex = imageIndexDic[string.Empty];
            childNode.SelectedImageIndex = imageIndexDic[string.Empty];
            node.Nodes.Add(childNode);
          });

          FileInfo[] files = dir.GetFiles();
          Array.Sort(files, delegate(FileInfo x, FileInfo y) {
            return x.Name.CompareTo(y.Name);
          });

          Array.ForEach(files, delegate(FileInfo file) {
            string ext = Path.GetExtension(file.Name).ToLower();
            if (!string.IsNullOrEmpty(ext)) {
              IExtractorFactory factory;
              if (extractorFactories.TryGetValue(ext, out factory)) {
                childNode = new TreeNode();
                childNode.Text = file.Name;
                childNode.ImageIndex = imageIndexDic[ext];
                childNode.SelectedImageIndex = imageIndexDic[ext];
                node.Nodes.Add(childNode);
              }
            }
          });
        }
        catch { }
      }
      node.Tag = setObj;
    }

    public TreeNode FindNode(string path) {
      string[] items = path.Split('\\');
      TreeNode pathNode = null;
      foreach (TreeNode node in this.Nodes) {
        if (items[0].Equals(node.Text, StringComparison.OrdinalIgnoreCase)) {
          pathNode = node;
        }
      }
      if (pathNode == null) return null;

      for (int i = 1; i < items.Length; i++) {
        //pathNode.Nodes.Clear();
        //test
        //if (pathNode.Nodes.Count == 0) {
        if (ChildNotSet(pathNode)) {
          pathNode.Nodes.Clear();
          SetChildNodes(pathNode);
        }
        bool found = false;
        foreach (TreeNode node in pathNode.Nodes) {
          if (items[i].Equals(node.Text, StringComparison.OrdinalIgnoreCase)) {
            pathNode = node;
            found = true;
            break;
          }
        }
        if (!found) return null;
      }

      //test
      //if (pathNode.Nodes.Count == 0) {
      if (ChildNotSet(pathNode)) {
        pathNode.Nodes.Clear();
        SetChildNodes(pathNode);
      }
      return pathNode;
    }

    private bool ChildNotSet(TreeNode node) {
      return node.Tag != setObj;
    }

    private bool ChildNotFound(TreeNode node) {
      return node.Tag != findObj;
    }

    protected override void OnBeforeExpand(TreeViewCancelEventArgs e) {
      Cursor.Current = Cursors.WaitCursor;
      //test
      //if (resetNodeRequired) {
      if (resetNodeRequired || ChildNotSet(e.Node)) {
        e.Node.Nodes.Clear();
        SetChildNodes(e.Node);
      }
      foreach (TreeNode node in e.Node.Nodes) {
        if (resetNodeRequired || ChildNotFound(node)) {
          FindChildNode(node);
        }
//        if (resetNodeRequired) {
//          node.Nodes.Clear();
//          SetChildNodes(node);
//        }
//        else if (node.Nodes.Count == 0) {
//          SetChildNodes(node);
//        }
      }
      base.OnBeforeExpand(e);
      Cursor.Current = Cursors.Default;
    }

    protected override bool ProcessDialogKey(Keys keyData) {
      if ((keyData&Keys.Tab) != 0) {
        OnKeyDown(new KeyEventArgs(keyData));
        return true;
      }
      else {
        return base.ProcessDialogKey(keyData);
      }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      if (e.KeyCode == Keys.ShiftKey) {
        resetNodeRequired = true;
        e.Handled = true;
      }
      base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e) {
      if (e.KeyCode == Keys.ShiftKey) {
        resetNodeRequired = false;
        e.Handled = true;
      }
      base.OnKeyUp(e);
    }
  }
}
