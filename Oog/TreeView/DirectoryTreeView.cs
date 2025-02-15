using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Oog.Plugin;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace Oog {
  class DirectoryTreeView : TreeView {

    private Dictionary<string, IExtractorFactory> extractorFactories;
    private Dictionary<string, int> imageIndexDic;
    ImageList imageList;

    private bool resetNodeRequired;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool ResetNodeRequired {
      get { return resetNodeRequired; }
      set { resetNodeRequired = value; }
    }

    public DirectoryTreeView() {
      resetNodeRequired = false;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Dictionary<string, IExtractorFactory> ExtractorFactories {
      get { return extractorFactories; }
      set { extractorFactories = value; }
    }

    public void RefreshTree() {
      InitializeImageIndex();
      this.Nodes.Clear();
      Array.ForEach(Environment.GetLogicalDrives(), delegate(string drive) {
        if (Directory.Exists(drive)) {
          TreeNode node = new OogTreeNode();
          node.Text = drive.TrimEnd('\\');
          node.ImageIndex = 0;
          this.Nodes.Add(node);

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

    private IEnumerable<string>GetFiles(string dir) {
      var dirPath = Path.GetDirectoryName(dir);
      var findPattern = Path.GetFileName(dir);
      if (Directory.Exists(dirPath)) {
        IEnumerable<FileSystemInfo> fsiQuery;
        try {
          fsiQuery = new DirectoryInfo(dirPath).EnumerateFileSystemInfos(findPattern, SearchOption.TopDirectoryOnly);
        }
        catch (System.Exception e) {
          Debug.WriteLine(e);
          yield break;
        }
          var query = fsiQuery.Select(x => ((x.Attributes & FileAttributes.Directory) == 0) ? (x.FullName) : ($"{x.FullName}{Path.DirectorySeparatorChar}"));
          foreach (var x in query) {
            yield return x;
          }
      }
      else {
        yield break;
      }
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
          node.Nodes.Add(new OogTreeNode());
          break;
        }
        else {
          string ext = Path.GetExtension(s).ToLower();
          if (!string.IsNullOrEmpty(ext)) {
            if (extractorFactories.ContainsKey(ext)) {
              node.Nodes.Add(new OogTreeNode());
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
          var dirNodes = dir.GetDirectories()
            .Select(subDir => {
              TreeNode childNode = new OogTreeNode(true);
              childNode.Text = subDir.Name;
              childNode.ImageIndex = imageIndexDic[string.Empty];
              childNode.SelectedImageIndex = imageIndexDic[string.Empty];
              return childNode;
            });

          var extNodes = dir.GetFiles()
            .Select(file => new { File = file, Ext = Path.GetExtension(file.Name).ToLower() })
            .Where(fileExt => !string.IsNullOrEmpty(fileExt.Ext))
            .Where(fileExt => extractorFactories.ContainsKey(fileExt.Ext))
            .Select(fileExt => {
              TreeNode childNode = new OogTreeNode(false);
              childNode.Text = fileExt.File.Name;
              childNode.ImageIndex = imageIndexDic[fileExt.Ext];
              childNode.SelectedImageIndex = imageIndexDic[fileExt.Ext];
              return childNode;
            });

          node.Nodes.AddRange(Enumerable.Concat(dirNodes, extNodes).ToArray());
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
      try {
        if (resetNodeRequired || ChildNotSet(e.Node)) {
          e.Node.Nodes.Clear();
          SetChildNodes(e.Node);
        }
        foreach (TreeNode node in e.Node.Nodes) {
          if (resetNodeRequired || ChildNotFound(node)) {
            FindChildNode(node);
          }
        }
        base.OnBeforeExpand(e);
      }
      finally {
        Cursor.Current = Cursors.Default;
      }
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
