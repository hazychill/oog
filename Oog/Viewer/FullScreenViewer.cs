using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Oog.Plugin;
using System.IO;
using SixLabors.ImageSharp.Processing;

namespace Oog.Viewer {
  public partial class FullScreenViewer : Form {

    private Image errorImage;
    private Dictionary<Keys, MethodInvoker> keyEventMap;

    private bool cursorVisible;

    public FullScreenViewer() {
      InitializeComponent();
      InitializeSettings();
      InitializeMenuSettings();
      InitializeErrorImage();
      InitializeKeyEventMap();

      this.MouseDown += OnMouseDown;
      this.MouseUp += OnMouseUp;
      this.MouseMove += OnMouseMove;
      this.MouseWheel += OnMouseWheel;
      picture.MouseDown += OnMouseDown;
      picture.MouseUp += OnMouseUp;
      picture.MouseMove += OnMouseMove;
      picture.MouseWheel += OnMouseWheel;

      contextMenuStrip1.Opening += delegate {
        menuInfo.Text = string.Format("{0} of {1}", currentIndex+1, imageNames.Length);
      };
      
      onMouseDown = StartScroll;
      onMouseUp = DoNothing;
      onMouseMove = CheckMenuPosition;

      this.Size = Screen.PrimaryScreen.Bounds.Size;
      this.WindowState = FormWindowState.Maximized;

      imageNames = new string[0];
      currentIndex = -1;

      this.FormClosed += delegate {
        ShowCursor();
        try {
          picture.Image.Dispose();
          lookAheadImage.Dispose();
        }
        catch {}
        picture.Image = null;
        try {
          lookAheadImage.Dispose();
        }
        catch {}
      };

      cursorVisible = true;
    }

    public void Reset(IExtractor extractor, Dictionary<string, IImageCreator> imageCreators, string[] imageNames, int index) {
      this.extractor = extractor;
      this.imageCreators = imageCreators;
      this.imageNames = imageNames;

      if (0<=index && index<imageNames.Length) {
        currentIndex = index;
      }
    }

    private void InitializeSettings() {
      settings = FullScreenViewerSettings.Default;
      //interpolationMode = InterpolationMode.High;
      //resizer = ImageResizer.OriginalSize;
    }

    private void InitializeMenuSettings() {
      Resizer original = ImageResizer.OriginalSize;
      Resizer screen = ImageResizer.ShrinkHoldingRatio;
      Resizer adjustWidth = ImageResizer.ShrinkWidthHoldingRatio;
      Resizer width80p = ImageResizer.SizeWidthScreenRatio80;
      menuSizeOriginal.Tag = original;
      menuSizeScreen.Tag = screen;
      menuSizeAdjustWidth.Tag = adjustWidth;
      menuSizeWidth80p.Tag = width80p;

      menuQualityHigh.Tag = KnownResamplers.Bicubic;
      menuQualityMiddle.Tag = KnownResamplers.Bicubic;
      menuQualityLow.Tag = KnownResamplers.NearestNeighbor;
    }

    private void InitializeErrorImage() {
      errorImage = new Bitmap(200, 200);
      using (Graphics g = Graphics.FromImage(errorImage))
      using (Pen pen = new Pen(Color.Red, 4))
      using (Font tempFont = SystemFonts.DefaultFont)
      using (Font font = new Font(tempFont.FontFamily, 9))
      using (Brush brush = new SolidBrush(Color.Red)) {
        g.Clear(Color.White);
        string message = "Unable to display the image.";
        Size textSize = g.MeasureString(message, font).ToSize();
        Point p = new Point((200-textSize.Width)/2, 180);
        g.DrawString(message, font, brush, p);
        g.DrawRectangle(pen, new Rectangle(1, 1, 198, 198));
        g.DrawLine(pen, new Point(0, 0), new Point(199, 199));
        g.DrawLine(pen, new Point(199, 0), new Point(0, 199));
      }
    }

    public void ApplySettings(FullScreenViewerSettings settings) {
      this.settings = settings;
      this.BackColor = settings.BackColor;
      this.picture.BackColor = settings.BackColor;
    }

    private void ExitViewer(object sender, EventArgs e) {
      this.Close();
    }

    private void HideCursor() {
      if (cursorVisible == true) {
        Cursor.Hide();
        cursorVisible = false;
      }
    }

    private void ShowCursor() {
      if (cursorVisible == false) {
        Cursor.Show();
        cursorVisible = true;
      }
    }
  }
}