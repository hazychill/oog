namespace Oog.Viewer {
  partial class FullScreenViewer {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      try {
        picture.Image.Dispose();
      }
      catch {}
      try {
        errorImage.Dispose();
      }
      catch {}
      try {
        lookAheadImage.Dispose();
      }
      catch {}
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FullScreenViewer));
      this.picture = new System.Windows.Forms.PictureBox();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.menuInfo = new System.Windows.Forms.ToolStripLabel();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.menuFirst = new System.Windows.Forms.ToolStripMenuItem();
      this.menuLast = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.menuSize = new System.Windows.Forms.ToolStripMenuItem();
      this.menuSizeOriginal = new System.Windows.Forms.ToolStripMenuItem();
      this.menuSizeScreen = new System.Windows.Forms.ToolStripMenuItem();
      this.menuSizeAdjustWidth = new System.Windows.Forms.ToolStripMenuItem();
      this.menuQuality = new System.Windows.Forms.ToolStripMenuItem();
      this.menuQualityHigh = new System.Windows.Forms.ToolStripMenuItem();
      this.menuQualityMiddle = new System.Windows.Forms.ToolStripMenuItem();
      this.menuQualityLow = new System.Windows.Forms.ToolStripMenuItem();
      this.menuRotate = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.menuHide = new System.Windows.Forms.ToolStripMenuItem();
      this.messageLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
      this.contextMenuStrip1.SuspendLayout();
      this.SuspendLayout();
      //
      // picture
      //
      this.picture.AutoSize = true;
      this.picture.Location = new System.Drawing.Point(201, 162);
      this.picture.Name = "picture";
      this.picture.Size = new System.Drawing.Size(100, 50);
      this.picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.picture.TabIndex = 0;
      this.picture.TabStop = false;
      //
      // contextMenuStrip1
      //
      this.contextMenuStrip1.Enabled = true;
      this.contextMenuStrip1.GripMargin = new System.Windows.Forms.Padding(2);
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.menuInfo,
        this.toolStripSeparator4,
        this.menuExit,
        this.toolStripSeparator1,
        this.menuFirst,
        this.menuLast,
        this.toolStripSeparator2,
        this.menuSize,
        this.menuQuality,
        this.toolStripSeparator3,
        this.menuRotate,
        new System.Windows.Forms.ToolStripSeparator(),
        this.menuHide});
      this.contextMenuStrip1.Location = new System.Drawing.Point(21, 36);
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      //this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.contextMenuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.contextMenuStrip1.Size = new System.Drawing.Size(124, 154);
      //
      // menuInfo
      //
      this.menuInfo.Name = "menuInfo";
      this.menuInfo.Text = "test";
      //
      // toolStripSeparator4
      //
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      //
      // menuExit
      //
      this.menuExit.Name = "menuExit";
      this.menuExit.Text = "&Exit viewer";
      this.menuExit.Click += new System.EventHandler(this.ExitViewer);
      //
      // toolStripSeparator1
      //
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      //
      // menuFirst
      //
      this.menuFirst.Name = "menuFirst";
      this.menuFirst.Text = "&First image";
      this.menuFirst.Click += new System.EventHandler(this.MoveToFirstImage);
      //
      // menuLast
      //
      this.menuLast.Name = "menuLast";
      this.menuLast.Text = "&Last image";
      this.menuLast.Click += new System.EventHandler(this.MoveToLastImage);
      //
      // toolStripSeparator2
      //
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      //
      // menuSize
      //
      this.menuSize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.menuSizeOriginal,
        this.menuSizeScreen,
        this.menuSizeAdjustWidth});
      this.menuSize.Name = "menuSize";
      this.menuSize.Text = "&Size mode";
      //
      // menuSizeOriginal
      //
      this.menuSizeOriginal.Name = "menuSizeOriginal";
      this.menuSizeOriginal.Text = "&Original";
      this.menuSizeOriginal.Click += new System.EventHandler(this.ChangeSizeMode);
      //
      // menuSizeScreen
      //
      this.menuSizeScreen.Name = "menuSizeScreen";
      this.menuSizeScreen.Text = "&Screen";
      this.menuSizeScreen.Click += new System.EventHandler(this.ChangeSizeMode);
      //
      // menuSizeAdjustWidth
      //
      this.menuSizeAdjustWidth.Name = "menuSizeAdjustWidth";
      this.menuSizeAdjustWidth.Text = "&Adjust width";
      this.menuSizeAdjustWidth.Click += new System.EventHandler(this.ChangeSizeMode);
      //
      // menuQuality
      //
      this.menuQuality.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.menuQualityHigh,
        this.menuQualityMiddle,
        this.menuQualityLow});
      this.menuQuality.Name = "menuQuality";
      this.menuQuality.Text = "&Quality";
      //
      // menuQualityHigh
      //
      this.menuQualityHigh.Name = "menuQualityHigh";
      this.menuQualityHigh.Text = "&High";
      this.menuQualityHigh.Click += new System.EventHandler(this.ChangeQuality);
      //
      // menuQualityMiddle
      //
      this.menuQualityMiddle.Name = "menuQualityMiddle";
      this.menuQualityMiddle.Text = "&Middle";
      this.menuQualityMiddle.Click += new System.EventHandler(this.ChangeQuality);
      //
      // menuQualityLow
      //
      this.menuQualityLow.Name = "menuQualityLow";
      this.menuQualityLow.Text = "&Low";
      this.menuQualityLow.Click += new System.EventHandler(this.ChangeQuality);
      //
      // menuRotate
      //
      this.menuRotate.Name = "menuRotate";
      this.menuRotate.Text = "&Rotate";
      this.menuRotate.Checked = false;
      this.menuRotate.Click += new System.EventHandler(this.menuRotate_Click);
      //
      // toolStripSeparator3
      //
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      //
      // menuHide
      //
      this.menuHide.Name = "menuHide";
      this.menuHide.Text = "&Hide menu";
      //
      // messageLabel
      //
      this.messageLabel.AutoSize = true;
      this.messageLabel.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 15f);
      this.messageLabel.BackColor = System.Drawing.Color.LightSteelBlue;
      this.messageLabel.ForeColor = System.Drawing.Color.DarkBlue;
      this.messageLabel.Visible = false;
      //
      // FullScreenViewer
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Black;
      this.ClientSize = new System.Drawing.Size(508, 396);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
        this.messageLabel,
        this.picture
        });
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FullScreenViewer";
      this.Text = "Oog : FullScreenViewer";
      this.Load += new System.EventHandler(this.OnLoad);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
      this.contextMenuStrip1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

#endregion

    private System.Windows.Forms.PictureBox picture;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripLabel menuInfo;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripMenuItem menuExit;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem menuFirst;
    private System.Windows.Forms.ToolStripMenuItem menuLast;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem menuSize;
    private System.Windows.Forms.ToolStripMenuItem menuQuality;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripMenuItem menuHide;
    private System.Windows.Forms.ToolStripMenuItem menuSizeOriginal;
    private System.Windows.Forms.ToolStripMenuItem menuSizeScreen;
    private System.Windows.Forms.ToolStripMenuItem menuSizeAdjustWidth;
    private System.Windows.Forms.ToolStripMenuItem menuQualityHigh;
    private System.Windows.Forms.ToolStripMenuItem menuQualityMiddle;
    private System.Windows.Forms.ToolStripMenuItem menuQualityLow;
    private System.Windows.Forms.ToolStripMenuItem menuRotate;
    private System.Windows.Forms.Label messageLabel;
  }
}