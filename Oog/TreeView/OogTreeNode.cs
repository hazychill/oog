using System;
using System.Windows.Forms;

namespace Oog {
  class OogTreeNode : TreeNode {
    public bool IsDirectory { get; set; }

    public OogTreeNode(bool isDirectory) : base() {
      IsDirectory = isDirectory;
    }

    public OogTreeNode() : this(true) { }
  }
}
