namespace Oog {
  partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

#region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.directoryTreeView = new Oog.DirectoryTreeView();
      this.thumbnailViewer1 = new Oog.ThumbnailViewer();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.refreshToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.collapseToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.settingToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.jumpToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.selectedPathTextBox = new System.Windows.Forms.TextBox();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.imageCountToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      //this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
      this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripControlHost(new PercentProgressBar());
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.toolStrip1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      //
      // splitContainer1
      //
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                           | System.Windows.Forms.AnchorStyles.Left)
                                                                          | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.splitContainer1.Location = new System.Drawing.Point(0, 50);
      this.splitContainer1.Name = "splitContainer1";
      //
      // splitContainer1.Panel1
      //
      this.splitContainer1.Panel1.Controls.Add(this.directoryTreeView);
      //
      // splitContainer1.Panel2
      //
      this.splitContainer1.Panel2.Controls.Add(this.thumbnailViewer1);
      this.splitContainer1.Size = new System.Drawing.Size(530, 356);
      this.splitContainer1.SplitterDistance = 204;
      this.splitContainer1.TabIndex = 0;
      this.splitContainer1.Text = "splitContainer1";
      //
      // directoryTreeView
      //
      this.directoryTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.directoryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.directoryTreeView.ExtractorFactories = null;
      this.directoryTreeView.Location = new System.Drawing.Point(0, 0);
      this.directoryTreeView.Name = "directoryTreeView";
      this.directoryTreeView.ResetNodeRequired = false;
      this.directoryTreeView.Size = new System.Drawing.Size(202, 354);
      this.directoryTreeView.TabIndex = 0;
      this.directoryTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.directoryTreeView_BeforeExpand);
      this.directoryTreeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.directoryTreeView_BeforeCollapse);
      this.directoryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.directoryTreeView_AfterSelect);
      this.directoryTreeView.MouseEnter += new System.EventHandler(this.directoryTreeView_MouseEnter);
      this.directoryTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.directoryTreeView_KeyDown);
      this.directoryTreeView.Click += new System.EventHandler(this.directoryTreeView_Click);
      //
      // thumbnailViewer1
      //
      this.thumbnailViewer1.AutoScroll = true;
      this.thumbnailViewer1.AutoScrollMinSize = new System.Drawing.Size(320, 200);
      this.thumbnailViewer1.BackColor = System.Drawing.SystemColors.Window;
      this.thumbnailViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.thumbnailViewer1.Location = new System.Drawing.Point(0, 0);
      this.thumbnailViewer1.Name = "thumbnailViewer1";
      this.thumbnailViewer1.Size = new System.Drawing.Size(320, 354);
      this.thumbnailViewer1.TabIndex = 0;
      this.thumbnailViewer1.Text = "thumbnailViewer1";
      //this.thumbnailViewer1.ThumbnailSize = new System.Drawing.Size(150, 200);
      this.thumbnailViewer1.MouseEnter += new System.EventHandler(this.thumbnailViewer1_MouseEnter);
      this.thumbnailViewer1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.thumbnailViewer1_KeyDown);
      this.thumbnailViewer1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.thumbnailViewer1_ProgressChanged);
      this.thumbnailViewer1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.thumbnailViewer1_Click);
      this.thumbnailViewer1.DoubleClick += new System.EventHandler(this.thumbnailViewer1_DoubleClick);
      //
      // toolStrip1
      //
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.refreshToolStripButton,
        this.collapseToolStripButton,
        this.settingToolStripButton,
        this.toolStripSeparator2,
        this.jumpToolStripButton,
        this.toolStripSeparator1,
        this.exitToolStripButton});
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.toolStrip1.Size = new System.Drawing.Size(530, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      //
      // refreshToolStripButton
      //
      this.refreshToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.refreshToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshToolStripButton.Image")));
      this.refreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.refreshToolStripButton.Name = "refreshToolStripButton";
      this.refreshToolStripButton.Text = "&Refresh";
      this.refreshToolStripButton.Click += new System.EventHandler(this.refreshToolStripButton_Click);
      //
      // collapseToolStripButton
      //
      this.collapseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.collapseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.collapseToolStripButton.Name = "collapseToolStripButton";
      this.collapseToolStripButton.Text = "&Collapse";
      this.collapseToolStripButton.Click += new System.EventHandler(this.collapseToolStripButton_Click);
      //
      // settingToolStripButton
      //
      this.settingToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.settingToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("settingToolStripButton.Image")));
      this.settingToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.settingToolStripButton.Name = "settingToolStripButton";
      this.settingToolStripButton.Text = "&Setting";
      this.settingToolStripButton.Click += new System.EventHandler(this.settingToolStripButton_Click);
      //
      // jumpToolStripButton
      //
      this.jumpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.jumpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("jumpToolStripButton.Image")));
      this.jumpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.jumpToolStripButton.Name = "jumpToolStripButton";
      this.jumpToolStripButton.Text = "&Jump";
      this.jumpToolStripButton.Click += new System.EventHandler(this.jumpToolStripButton_Click);
      //
      // toolStripSeparator1
      //
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      //
      // exitToolStripButton
      //
      this.exitToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.exitToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripButton.Image")));
      this.exitToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.exitToolStripButton.Name = "exitToolStripButton";
      this.exitToolStripButton.Text = "&Exit";
      this.exitToolStripButton.Click += new System.EventHandler(this.exitToolStripButton_Click);
      //
      // selectedPathTextBox
      //
      this.selectedPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                              | System.Windows.Forms.AnchorStyles.Right)));
      this.selectedPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.selectedPathTextBox.Location = new System.Drawing.Point(0, 28);
      this.selectedPathTextBox.Name = "selectedPathTextBox";
      this.selectedPathTextBox.Size = new System.Drawing.Size(530, 19);
      this.selectedPathTextBox.TabIndex = 2;
      this.selectedPathTextBox.Click += new System.EventHandler(this.selectedPathTextBox_Click);
      this.selectedPathTextBox.Enter += new System.EventHandler(this.selectedPathTextBox_Enter);
      this.selectedPathTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.selectedPathTextBox_KeyDown);
      //
      // statusStrip1
      //
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.imageCountToolStripStatusLabel,
        this.toolStripProgressBar1});
      this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
      this.statusStrip1.Location = new System.Drawing.Point(0, 408);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(530, 23);
      this.statusStrip1.TabIndex = 3;
      this.statusStrip1.Text = "statusStrip1";
      //
      // imageCountToolStripStatusLabel
      //
      this.imageCountToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.imageCountToolStripStatusLabel.Name = "imageCountToolStripStatusLabel";
      this.imageCountToolStripStatusLabel.Spring = true;
      this.imageCountToolStripStatusLabel.Text = "Ready";
      this.imageCountToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      //
      // toolStripProgressBar1
      //
      this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.toolStripProgressBar1.AutoSize = false;
      this.toolStripProgressBar1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
      this.toolStripProgressBar1.Name = "toolStripProgressBar1";
      this.toolStripProgressBar1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
      this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
      //this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.toolStripProgressBar1.Text = "toolStripProgressBar1";
      ((PercentProgressBar)this.toolStripProgressBar1.Control).Value = 50;
      //
      // MainForm
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(530, 431);
      this.Controls.Add(this.selectedPathTextBox);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.statusStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("Oog.ico")));
      this.Name = "MainForm";
      this.Text = "Oog";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.toolStrip1.ResumeLayout(false);
      this.statusStrip1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

#endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.TextBox selectedPathTextBox;
    private System.Windows.Forms.ToolStripButton refreshToolStripButton;
    private System.Windows.Forms.ToolStripButton collapseToolStripButton;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripButton settingToolStripButton;
    private System.Windows.Forms.ToolStripButton jumpToolStripButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton exitToolStripButton;
    private System.Windows.Forms.ToolStripStatusLabel imageCountToolStripStatusLabel;
    //private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
    private System.Windows.Forms.ToolStripControlHost toolStripProgressBar1;
    private DirectoryTreeView directoryTreeView;
    private ThumbnailViewer thumbnailViewer1;
  }
}

