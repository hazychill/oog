using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Linq;
using Oog.Plugin;
using Oog.Viewer;

namespace Oog {
  public partial class MainForm : Form {

    Dictionary<string, IExtractorFactory> extractorFactories;
    Dictionary<string, IImageCreator> imageCreators;
    IExtractor extractor;
    string[] names;
    OogSettings settings;

    private Comparison<string> nameComparison;

    private SettingsForm settingsForm;
    private AddJumpForm addJumpForm;
    private FullScreenViewer fullScreenViewer;
    private ContextMenuStrip jumpTargetContextMenu;

    public MainForm() {
      InitializeComponent();

      //nameComparison = CompareByExtension;
      nameComparison = CompareByName;

      onAfterSelect = DoNothing;
    }


    private void Form1_Load(object sender, EventArgs e) {
      fullScreenViewer = new FullScreenViewer();
      //this.components.Add(fullScreenViewer);

      SixLabors.ImageSharp.Configuration.Default.MemoryAllocator = SixLabors.Memory.ArrayPoolMemoryAllocator.CreateWithMinimalPooling();

      settings = OogSettings.Load();

      jumpTargetContextMenu = new ContextMenuStrip();
      this.components.Add(jumpTargetContextMenu);
      jumpTargetContextMenu.RenderMode = ToolStripRenderMode.System;
      ToolStripMenuItem removeMenu = new ToolStripMenuItem();
      jumpTargetContextMenu.Items.Add(removeMenu);
      removeMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
      removeMenu.Text = "&Remove";
      removeMenu.Click += delegate {
        ToolStripItem item = jumpTargetContextMenu.Tag as ToolStripItem;
        string itemName = item.Tag as string;
        if (item != null) {
          int index = toolStrip1.Items.IndexOf(item);
          if (index != -1) {
            toolStrip1.Items.RemoveAt(index);
            item.Dispose();
            settings.JumpPath.Remove(itemName);
            settings.Save();
          }
        }
      };

      SetJumpTargets(settings.JumpPath);

      this.Size = settings.WindowStartup.Size;
      this.Location = settings.WindowStartup.Location;
      this.WindowState = settings.WindowState;

      InitializePlugins();

      directoryTreeView.ExtractorFactories = extractorFactories;
      directoryTreeView.RefreshTree();
      SelectNode(directoryTreeView.Nodes[0].FullPath);

      ApplySettings(settings);

    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      this.Hide();
      try {
        Directory.Delete(PluginHelper.GetOogTempFolder(), true);
      }
      catch {}

      try {
        OogSettings settings = OogSettings.Load();
        if (this.WindowState == FormWindowState.Minimized) {
          settings.WindowState = FormWindowState.Normal;
        }
        else if (this.WindowState == FormWindowState.Normal) {
          settings.WindowStartup = new Rectangle(this.Location, this.Size);
          settings.WindowState = this.WindowState;
        }
        else {
          settings.WindowState = this.WindowState;
        }
        settings.Save();
      }
      catch {}
    }

    private void InitializePlugins() {
      extractorFactories = new Dictionary<string, IExtractorFactory>();
      imageCreators = new Dictionary<string, IImageCreator>();

      string exeDir = Path.GetDirectoryName(Application.ExecutablePath);

      AddDefaultExtractorFactory(extractorFactories, new FolderExtractorFactory());
      AddDefaultImageCreator(imageCreators, new DefaultImageCreator());

      var dllQuery = Directory.EnumerateFiles(exeDir, "*.dll")
        .Select(dllPath => {
          try {
            return Assembly.LoadFile(dllPath);
          }
          catch {
            return null;
          }
        } )
        .Where(asm => asm != null);

      var extractorFactoryQuery = GetPluginQuery<IExtractorFactory>(dllQuery)
        .SelectMany(factory => factory.SupportedExtensions.Select(ext => new { Key = ext, Value = factory }));
      var imageCreatorQuery = GetPluginQuery<IImageCreator>(dllQuery)
        .SelectMany(creator => creator.SupportedExtensions.Select(ext => new { Key = ext, Value = creator }));

      foreach (var extractorFactoryKeyValue in extractorFactoryQuery) {
        if (!extractorFactories.ContainsKey(extractorFactoryKeyValue.Key)) {
          extractorFactories.Add(extractorFactoryKeyValue.Key, extractorFactoryKeyValue.Value);
        }
      }

      foreach (var imageCreatorKeyValue in imageCreatorQuery) {
        if (!imageCreators.ContainsKey(imageCreatorKeyValue.Key)) {
          imageCreators.Add(imageCreatorKeyValue.Key, imageCreatorKeyValue.Value);
        }
      }
    }

    private void AddDefaultExtractorFactory(Dictionary<string, IExtractorFactory> extractorFactories, IExtractorFactory defaultExtractorFactory) {
      foreach (string key in defaultExtractorFactory.SupportedExtensions) {
        if (!extractorFactories.ContainsKey(key)) {
          extractorFactories.Add(key, defaultExtractorFactory);
        }
      }
    }

    private void AddDefaultImageCreator(Dictionary<string, IImageCreator> imageCreators, IImageCreator defaultImageCreator) {
      foreach (string key in defaultImageCreator.SupportedExtensions) {
        if (!imageCreators.ContainsKey(key)) {
          imageCreators.Add(key, defaultImageCreator);
        }
      }
    }

    private IEnumerable<T> GetPluginQuery<T>(IEnumerable<Assembly> asms) {
      return asms.SelectMany(asm => asm.GetExportedTypes())
        .Where(type => type.IsClass)
        .Where(type => !type.IsAbstract)
        .Where(type => type.GetInterfaces().Contains(typeof(T)))
        .Select(type => type.GetConstructor(Type.EmptyTypes))
        .Where(ctor => ctor != null)
        .Select(ctor => Instantiate<T>(ctor));
    }

    private T Instantiate<T>(ConstructorInfo ctor) {
      dynamic instance = ctor.Invoke(null);
      return instance;
    }

    static System.Text.RegularExpressions.Regex shortcutPattern = new System.Text.RegularExpressions.Regex("^(.*?)([a-zA-Z0-9])");

    private void SetJumpTargets(Dictionary<string, string> jumpPath) {
      ClearJumpTargets();
      int insertIndex = 5;
      foreach (string itemName in jumpPath.Keys) {
        ToolStripButton button = new ToolStripButton();
        button.DisplayStyle = ToolStripItemDisplayStyle.Text;
        button.Text = shortcutPattern.Replace(itemName, "$1&$2");
        button.Tag = itemName;

        string itemPath = jumpPath[itemName];
        button.Click += delegate {
          Jump(itemPath);
        };
        button.MouseUp += delegate(object sender, MouseEventArgs e) {
          if (e.Button == MouseButtons.Right) {
            jumpTargetContextMenu.Tag = button;
            jumpTargetContextMenu.Show(Cursor.Position);
          }
        };
        toolStrip1.Items.Insert(insertIndex, button);

        insertIndex++;
      }
    }

    private void ClearJumpTargets() {
      int removeIndex = 5;
      while (toolStrip1.Items.Count > 9) {
        ToolStripItem item = toolStrip1.Items[removeIndex];
        toolStrip1.Items.RemoveAt(removeIndex);
        item.Dispose();
      }
    }

    private void AddJumpPath(string itemName, string itemPath) {
      int insertIndex = toolStrip1.Items.Count - 4;

      ToolStripButton button = new ToolStripButton();
      button.DisplayStyle = ToolStripItemDisplayStyle.Text;
      button.Text = shortcutPattern.Replace(itemName, "$1&$2");
      button.Tag = itemName;

      button.Click += delegate {
        Jump(itemPath);
      };
      button.MouseUp += delegate(object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Right) {
          jumpTargetContextMenu.Tag = button;
          jumpTargetContextMenu.Show(Cursor.Position);
        }
      };
      toolStrip1.Items.Insert(insertIndex, button);
    }

    private void refreshToolStripButton_Click(object sender, EventArgs e) {
      directoryTreeView.RefreshTree();
      if (directoryTreeView.Nodes.Count > 0) {
        SelectNode(directoryTreeView.Nodes[0].FullPath);
      }
    }

    private void collapseToolStripButton_Click(object sender, EventArgs e) {
      directoryTreeView.CollapseAll();
      if (directoryTreeView.Nodes.Count > 0) {
        SelectNode(directoryTreeView.Nodes[0].FullPath);
      }
    }


    private void settingToolStripButton_Click(object sender, EventArgs e) {
      if (settingsForm == null) {
        settingsForm = new SettingsForm();
        //this.components.Add(settingsForm);
      }

      settingsForm.Settings = OogSettings.Load();
      if (settingsForm.ShowDialog() == DialogResult.OK) {
        settings = settingsForm.Settings;
        settings.Save();
        ApplySettings(settings);
      }
    }

    private void jumpToolStripButton_Click(object sender, EventArgs e) {
      if (addJumpForm == null) {
        addJumpForm = new AddJumpForm();
        this.components.Add(addJumpForm);
      }

      addJumpForm.ItemPath = selectedPathTextBox.Text;
      addJumpForm.ItemName = Path.GetFileName(selectedPathTextBox.Text);
      if (addJumpForm.ShowDialog() == DialogResult.OK) {
        string itemName = addJumpForm.ItemName;
        string itemPath = addJumpForm.ItemPath;
        if (settings.JumpPath.ContainsKey(itemName)) {
          MessageBox.Show(string.Format("Name \"{0}\" already exists.", itemName));
          jumpToolStripButton_Click(sender, e);
        }
        else if (itemName.IndexOf('&') != -1) {
          MessageBox.Show("Name cannot contain charactor \'&\'");
          jumpToolStripButton_Click(sender, e);
        }
        else {
          settings.JumpPath.Add(itemName, itemPath);
          settings.Save();
          AddJumpPath(itemName, itemPath);
        }
      }
    }

    private void checkToolStripButton_Click(object sender, EventArgs e) {
      String path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "checked");
      using (Stream output = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
        output.Seek(0, SeekOrigin.End);
        using (TextWriter writer = new StreamWriter(output, new UTF8Encoding())) {
          writer.WriteLine(selectedPathTextBox.Text);
        }
      }
    }

    private void exitToolStripButton_Click(object sender, EventArgs e) {
      this.Close();
    }


#region TreeView

    private void Jump(string path) {
      Jump(path, true);
    }

    private void Jump(string path, bool checkCurrent) {
      if (checkCurrent && selectedPathTextBox.Text.TrimEnd('\\') == path.TrimEnd('\\')) return;
      TreeNode node = directoryTreeView.FindNode(path);
      if (node != null) {
        node.Expand();
        directoryTreeView.SelectedNode = node;
        SelectNode(node.FullPath);
      }
      else {
        SetStatusText(string.Format("Not found ({0})", path));
      }
    }

    private void SelectNode(string path) {
      selectedPathTextBox.Text = path;
      IExtractorFactory factory;
      extractor = null;
      string dir = string.Concat(path, "\\");
      if (Directory.Exists(dir)) {
        factory = extractorFactories[string.Empty];
        extractor = factory.Create(dir);
      }
      else if (File.Exists(path)) {
        string ext = Path.GetExtension(path).ToLower();
        if (extractorFactories.TryGetValue(ext, out factory)) {
          extractor = factory.Create(path);
        }
      }
      else {
        SetStatusText(string.Format("Not found ({0})", path));
      }

      toolStripProgressBar1.ForeColor = Color.LightSteelBlue;

      if (extractor != null) {
        names = OogUtil.GetNames(extractor.GetNames(), imageCreators);

        if (nameComparison != null) {
          Array.Sort<string>(names, nameComparison);
        }

        SetStatusText(string.Format("{0} items.", names.Length));

        thumbnailViewer1.SetThumbnails(extractor, imageCreators, names);
      }
    }

    private void SetStatusText(string text) {
      imageCountToolStripStatusLabel.Text = text;
    }

    private int CompareByName(string x, string y) {
      return x.CompareTo(y);
    }
    
    private int CompareByExtension(string x, string y) {
      string ext_x = Path.GetExtension(x);
      string ext_y = Path.GetExtension(y);
      if (ext_x.Equals(ext_y, StringComparison.OrdinalIgnoreCase)) {
        return x.CompareTo(y);
      }
      else {
        return ext_x.CompareTo(ext_y);
      }
    }

    private void SelectNode(object sender, TreeViewEventArgs e) {
      SelectNode(e.Node.FullPath);
      onAfterSelect = DoNothing;
    }

    private void directoryTreeView_KeyDown(object sender, KeyEventArgs e) {
      onAfterSelect = DoNothing;
      switch (e.KeyCode) {
      case Keys.Enter:
        SelectNode(directoryTreeView.SelectedNode.FullPath);
        thumbnailViewer1.Focus();
        e.Handled = true;
        break;
      case Keys.Tab:
        thumbnailViewer1.Focus();
        e.Handled = true;
        break;
      case Keys.L:
        if (e.Alt == false && e.Control == true) {
          selectedPathTextBox.Focus();
          e.Handled = true;
        }
        break;
      case Keys.C:
        if (e.Alt == false && e.Control == true) {
          checkToolStripButton.PerformClick();
          MessageBox.Show(this, "checked");
          e.Handled = true;
        }
        break;
      }
    }

    private void DoNothing(object sender, TreeViewEventArgs e) { }

    TreeViewEventHandler onAfterSelect;

    private void directoryTreeView_Click(object sender, EventArgs e) {
      onAfterSelect = SelectNode;
    }

    private void directoryTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
      onAfterSelect = DoNothing;
    }

    private void directoryTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      onAfterSelect = DoNothing;
    }

    private void directoryTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      onAfterSelect(sender, e);
    }

    private void directoryTreeView_MouseEnter(object sender, EventArgs e) {
      directoryTreeView.Focus();
    }

#endregion

    private void thumbnailViewer1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      MethodInvoker mi = delegate {
        ((PercentProgressBar)toolStripProgressBar1.Control).Value = e.ProgressPercentage;
      };
      this.Invoke(mi);
    }

    private void thumbnailViewer1_MouseEnter(object sender, EventArgs e) {
      thumbnailViewer1.Focus();
    }

    private void thumbnailViewer1_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
      case Keys.Tab:
        directoryTreeView.Focus();
        e.Handled = true;
        break;
      case Keys.Enter:
        ShowViewer();
        e.Handled = true;
        break;
      case Keys.L:
        if (e.Alt == false && e.Control == true) {
          selectedPathTextBox.Focus();
          e.Handled = true;
        }
        break;
      case Keys.C:
        if (e.Alt == false && e.Control == true) {
          checkToolStripButton.PerformClick();
          MessageBox.Show(this, "checked");
          e.Handled = true;
        }
        break;
      }
    }

    private void thumbnailViewer1_Click(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Middle) {
        ShowViewer();
      }
    }

    private void thumbnailViewer1_DoubleClick(object sender, EventArgs e) {
      ShowViewer();
    }

    bool textBoxFocused = false;

    private void selectedPathTextBox_Click(object sender, EventArgs e) {
      if (textBoxFocused) {
        selectedPathTextBox.SelectAll();
      }
      textBoxFocused = false;
    }

    private void selectedPathTextBox_Enter(object sender, EventArgs e) {
      textBoxFocused = true;
    }

    private void selectedPathTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        string path = selectedPathTextBox.Text;
        Jump(path.TrimEnd('\\'), false);
        thumbnailViewer1.Focus();
      }
    }

    private void ShowViewer() {
      int index = thumbnailViewer1.SelectedIndex;
      if (0<=index && index<names.Length) {
        fullScreenViewer.ThumbnailWorkerBlocker = thumbnailViewer1.WorkerBlocker;
        fullScreenViewer.Reset(extractor, imageCreators, names, index);
        fullScreenViewer.ShowDialog();

        index = fullScreenViewer.CurrentIndex;
        if (0<=index && index<names.Length) {
          thumbnailViewer1.SelectThumbnail(index);
          thumbnailViewer1.ScrollThumbnailIntoView(index);
        }
      }
    }

    private void ApplySettings(OogSettings settings) {
      if (!HasSameType(directoryTreeView.TreeViewNodeSorter, settings.TreeNodeComparer)) {
        directoryTreeView.TreeViewNodeSorter = settings.TreeNodeComparer;
      }
      thumbnailViewer1.ApplySettings(settings.ThumbnailSettings);
      fullScreenViewer.ApplySettings(settings.FullScreenViewerSettings);
    }

    private bool HasSameType(object x, object y) {
      if (x == null &&
          y != null) {
        return false;
      }

      if (x != null &&
          y == null) {
        return false;
      }

      if (x == null &&
          y == null) {
        return true;
      }

      return x.GetType() == y.GetType();
    }
  }
}