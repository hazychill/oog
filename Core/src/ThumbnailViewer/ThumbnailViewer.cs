using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Oog.Plugin;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Linq;

namespace Oog {
  public class ThumbnailViewer : ScrollableControl {

    public event ProgressChangedEventHandler ProgressChanged;

#region Fields

    private Thumbnail[] thumbnails;
    private int rows, cols;
    private Size cellSize;
    private ThumbnailSettings thumbnailSettings;

    private IExtractor extractor;
    private BackgroundWorker imageCreateWorker;
    private Dictionary<string, IImageCreator> imageCreators;

#endregion

#region Constructor

    public ThumbnailViewer() {
      SetStyle(ControlStyles.AllPaintingInWmPaint
               | ControlStyles.UserPaint
               | ControlStyles.OptimizedDoubleBuffer,
               true);

      InitializeWorker();

      BackColor = SystemColors.Window;
      thumbnails = new Thumbnail[0];

      thumbnailSettings = ThumbnailSettings.Default;
    }

    private void InitializeWorker() {
      imageCreateWorker = new BackgroundWorker();
      imageCreateWorker.WorkerSupportsCancellation = true;
      imageCreateWorker.WorkerReportsProgress = true;
      imageCreateWorker.DoWork += new DoWorkEventHandler(imageCreateWorker_DoWork);
      imageCreateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(imageCreateWorker_RunWorkerCompleted);
      imageCreateWorker.ProgressChanged += new ProgressChangedEventHandler(imageCreateWorker_ProgressChanged);
    }

#endregion

#region Properties

    public int SelectedIndex {
      get { return selectedIndex; }
    }

#endregion

    public void ApplySettings(ThumbnailSettings newSettings) {
      if (ClearRequired(thumbnailSettings, newSettings)) {
        thumbnailSettings = newSettings;
        this.BackColor = thumbnailSettings.BackColor;
        SetField();
        Array.ForEach<Thumbnail>(thumbnails, delegate(Thumbnail t) {
          t.BackColor = thumbnailSettings.BackColor;
        });
        ClearThumbnailImages();
        Invalidate();
        WaitWorkerCancellation(imageCreateWorker.RunWorkerAsync);
      }
      else if (thumbnailSettings != newSettings) {
        thumbnailSettings = newSettings;
        this.BackColor = thumbnailSettings.BackColor;
        Array.ForEach<Thumbnail>(thumbnails, delegate(Thumbnail t) {
          t.BackColor = thumbnailSettings.BackColor;
        });
        //SetField();
        //ClearThumbnailImages();
        Invalidate();
        //WaitWorkerCancellation(imageCreateWorker.RunWorkerAsync);
      }
    }

    private bool ClearRequired(ThumbnailSettings oldSettings, ThumbnailSettings newSettings) {
      return (oldSettings.Size != newSettings.Size) || (oldSettings.InterpolationMode != newSettings.InterpolationMode);
    }

    private void ClearThumbnailImages() {
      for (int i = 0; i < thumbnails.Length; i++) {
        thumbnails[i].ClearImage();
      }
    }

#region Paint

    protected override void OnPaint(PaintEventArgs e) {
      int rectTop = -this.AutoScrollPosition.Y + e.ClipRectangle.Y;
      int rowStart = rectTop / cellSize.Height; ;
      int rowEnd = (rectTop+e.ClipRectangle.Height) / cellSize.Height;

      Graphics g = e.Graphics;
      g.InterpolationMode = thumbnailSettings.InterpolationMode;
      for (int row = rowStart; row <= rowEnd; row++) {
        for (int col = 0; col < cols; col++) {
          int index = cols*row + col;
          if (0<=index && index < thumbnails.Length) {
            Point cellLocation = new Point(col*cellSize.Width, row*cellSize.Height+this.AutoScrollPosition.Y);
            Rectangle cellRect = new Rectangle(cellLocation, cellSize);
            thumbnails[index].DrawThumbnail(g, cellRect);
          }
        }
      }
      base.OnPaint(e);
    }

#endregion

    protected override void OnResize(EventArgs e) {
      SetField();
      base.OnResize(e);
    }

    private void SetField() {

      cols = this.ClientSize.Width / thumbnailSettings.Size.Width;
      if (cols < 1) {
        cols = 1;
      }
      rows = (thumbnails.Length-1) / cols + 1;

      cellSize = new Size(this.ClientSize.Width/cols, thumbnailSettings.Size.Height);

      this.AutoScrollMinSize = new Size(cols*cellSize.Width, rows*cellSize.Height);
    }

    public void SetThumbnails(IExtractor extractor, Dictionary<string, IImageCreator> imageCreators, string[] names) {
      Cursor.Current = Cursors.WaitCursor;

      if (imageCreateWorker.IsBusy) {
        WaitWorkerCancellation(delegate { SetThumbnails(extractor, imageCreators, names); });
        return;
      }

      ClearThumbnails();

      this.extractor = extractor;
      this.imageCreators = imageCreators;

      thumbnails = names
        .Select(name => Thumbnail.Create(name, thumbnailSettings))
        .ToArray();

      SetField();

      if (thumbnails.Length > 0) {
        SelectThumbnail(0);
        ScrollThumbnailIntoView(0);
      }

      this.Invalidate();

      Cursor.Current = Cursors.Default;

      imageCreateWorker.RunWorkerAsync();
    }

    private void ClearThumbnails() {
      foreach (Thumbnail thumbnail in thumbnails) {
        if (thumbnail.Image != null) {
          thumbnail.Image.Dispose();
        }
        thumbnail.Dispose();
      }
      thumbnails = null;
      if (extractor != null) {
        extractor.Close();
      }
      selectedIndex = -1;
    }

#region Create image background

    int prevPercentage;

    void imageCreateWorker_DoWork(object sender, DoWorkEventArgs e) {
      //Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
      Thread.CurrentThread.Priority = thumbnailSettings.ThumbnailThreadPriority;

      imageCreateWorker.ReportProgress(0);
      prevPercentage = 0;

      int count = thumbnails.Length;
      int current = 0;

      foreach (int index in EnumIndex()) {

        if (imageCreateWorker.CancellationPending) {
          e.Cancel = true;
          break;
        }
        CreateThumbnailImage(index);
        current++;
        int percentage = (current)*100/count;
        if (percentage > prevPercentage) {
          imageCreateWorker.ReportProgress(percentage);
          prevPercentage = percentage;
        }
      }

      if (!imageCreateWorker.CancellationPending) {
        imageCreateWorker.ReportProgress(100);
      }
    }

    private IEnumerable<int> EnumIndex() {
      var count = thumbnails.Length;
      var indexMap = Enumerable.Range(0, count)
        .ToDictionary(index => index,
                      _ => false);
      var yielded = 0;

      while (yielded < count) {
        int? indexToYield = null;
        var viewFirst = GetThumbnailAtPoint(0, 0);
        var viewLast = Math.Min(GetThumbnailAtPoint(this.ClientRectangle.Width-1, this.ClientRectangle.Height-1),
                                thumbnails.Length-1);
        for (var i = viewFirst; i <= viewLast; i++) {
          if (indexMap[i] == false) {
            indexToYield = i;
            break;
          }
        }

        if (indexToYield == null) {
          for (var i = 0; i < count; i++) {
            if (indexMap[i] == false) {
              indexToYield = i;
              break;
            }
          }
        }

        indexMap[indexToYield.Value] = true;
        yielded++;
        yield return indexToYield.Value;
      }
    }

    private void CreateThumbnailImage(int index) {
      Thumbnail thumbnail = thumbnails[index];
      string name = thumbnail.Name;

      using (Stream data = extractor.GetData(name)) {

        if (data == null) {
          thumbnail.SetImage(null);
          return;
        }

        string ext = Path.GetExtension(name).ToLower();
        IImageCreator creator = imageCreators[ext];

        using (Image original = creator.GetImage(data)) {

          if (original == null) {
            thumbnail.SetImage(null);
            return;
          }

          Image resized = ImageResizer.Resize(original, thumbnailSettings.Size, ImageResizer.ShrinkHoldingRatio, thumbnailSettings.InterpolationMode);
          if (resized == original) resized = new Bitmap(original);
          thumbnail.SetImage(resized);

          if (!imageCreateWorker.CancellationPending) DrawThumbnail(index);
        }
      }
    }

    void imageCreateWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      if (this.ProgressChanged != null) {
        ProgressChanged(this, e);
      }
    }

    MethodInvoker completed;

    void imageCreateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (completed != null) {
        completed();
        completed = delegate { };
      }
    }

#endregion

    public void Clear() {
      ClearThumbnails();
      Invalidate();
    }

#region Mouse events

    int selectedIndex = -1;

    protected override void OnMouseDown(MouseEventArgs e) {
      int index = GetThumbnailAtPoint(e.X, e.Y);
      if (0<=index && index<thumbnails.Length && selectedIndex!=index) {
        SelectThumbnail(index);
      }
      base.OnMouseDown(e);
    }

    public void SelectThumbnail(int index) {
      if (0<=index && index<thumbnails.Length) {
        if (selectedIndex != -1) {
          thumbnails[selectedIndex].Selected = false;
          DrawThumbnail(selectedIndex);
        }
        thumbnails[index].Selected = true;
        selectedIndex = index;
        DrawThumbnail(index);
      }
    }

    private int GetThumbnailAtPoint(int x, int y) {
      int h = -this.AutoScrollPosition.Y + y;
      int row = h / cellSize.Height;
      int col = x / cellSize.Width;
      int index = cols * row + col;
      return index;
    }

    const int WM_MOUSEWHEEL = 522;

    protected override void WndProc(ref Message m) {
      if (m.Msg == WM_MOUSEWHEEL) {
        int delta = m.WParam.ToInt32() >> 16;
        WheelScroll(delta);
      }
      else {
        base.WndProc(ref m);
      }
    }

    const int WHEEL_DELTA = 120;

    private void WheelScroll(int delta) {
      int scrollCount = delta / WHEEL_DELTA;
      int top = -this.AutoScrollPosition.Y - scrollCount*cellSize.Height;
      if (top < 0) {
        top = 0;
      }
      int max = cellSize.Height*rows-this.Height;
      if (top > max) {
        top = max;
      }
      this.AutoScrollPosition = new Point(0, top);
    }

#endregion

#region Key events

    protected override bool ProcessDialogKey(Keys keyData) {
      if ((keyData&Keys.Alt)!=0 || (keyData&Keys.Control)!=0 || (keyData&Keys.Shift)!=0) {
        return base.ProcessDialogKey(keyData);
      }
      
      switch (keyData) {
      case Keys.Up:
        SelectUp();
        return true;
      case Keys.Down:
        SelectDown();
        return true;
      case Keys.Left:
        SelectLeft();
        return true;
      case Keys.Right:
        SelectRight();
        return true;
      case Keys.PageUp:
        SelectPageUp();
        return true;
      case Keys.PageDown:
        SelectPageDown();
        return true;
      case Keys.Home:
        SelectHome();
        return true;
      case Keys.End:
        SelectEnd();
        return true;
      case Keys.Enter:
      case Keys.Tab:
        OnKeyDown(new KeyEventArgs(keyData));
        return true;
      }
      return base.ProcessDialogKey(keyData);
    }

    private void SelectPageUp() {
      if (thumbnails.Length > 0) {
        int rowCount = this.ClientRectangle.Height / cellSize.Height;
        if (rowCount == 0) {
          rowCount = 1;
        }
        int index;
        while ((index=selectedIndex-rowCount*cols) < 0) {
          rowCount--;
        }
        SelectThumbnail(index);
        ScrollThumbnailIntoView(index);
      }
    }

    private void SelectPageDown() {
      if (thumbnails.Length > 0) {
        int rowCount = this.ClientRectangle.Height / cellSize.Height;
        if (rowCount == 0) {
          rowCount = 1;
        }
        int index;
        while ((index=selectedIndex+rowCount*cols) > thumbnails.Length-1) {
          rowCount--;
        }
        SelectThumbnail(index);
        ScrollThumbnailIntoView(index);
      }
    }

    private void SelectHome() {
      if (thumbnails.Length > 0) {
        SelectThumbnail(0);
        ScrollThumbnailIntoView(0);
      }
    }

    private void SelectEnd() {
      if (thumbnails.Length > 0) {
        SelectThumbnail(thumbnails.Length-1);
        ScrollThumbnailIntoView(thumbnails.Length-1);
      }
    }

    private void SelectUp() {
      int index = selectedIndex - cols;
      if (0<=index && index<thumbnails.Length) {
        SelectThumbnail(index);
        ScrollThumbnailIntoView(index);
      }
    }

    private void SelectDown() {
      int index = selectedIndex + cols;
      if (0<=index && index<thumbnails.Length) {
        SelectThumbnail(index);
        ScrollThumbnailIntoView(index);
      }
    }

    private void SelectLeft() {
      int index = selectedIndex - 1;
      if (0<=index && index<thumbnails.Length) {
        SelectThumbnail(index);
        ScrollThumbnailIntoView(index);
      }
    }

    private void SelectRight() {
      int index = selectedIndex + 1;
      if (0<=index && index<thumbnails.Length) {
        SelectThumbnail(index);
        ScrollThumbnailIntoView(index);
      }
    }

#endregion

    private void DrawThumbnail(int index) {
      Rectangle rect = new Rectangle(
        (index % cols) * cellSize.Width,
        (index / cols) * cellSize.Height + this.AutoScrollPosition.Y,
        cellSize.Width,
        cellSize.Height);
      this.Invalidate(rect);
    }

    public void ScrollThumbnailIntoView(int index) {
      if (index<0 || thumbnails.Length<=index) return;
      int row = index / cols;
      if (this.AutoScrollPosition.Y + row*cellSize.Height < 0) {
        this.AutoScrollPosition = new Point(0, row*cellSize.Height);
      }
      else if (this.AutoScrollPosition.Y + (row+1)*cellSize.Height > this.Height) {
        this.AutoScrollPosition = new Point(0, row*cellSize.Height - this.Height + cellSize.Height);
      }
    }

    private void WaitWorkerCancellation(MethodInvoker method) {
      if (imageCreateWorker.IsBusy) {
        completed = method;
        imageCreateWorker.CancelAsync();
      }
      else {
        method();
      }
    }

    protected override void Dispose(bool disposing) {
      ClearThumbnails();
      if (disposing) {
        imageCreateWorker.Dispose();
      }
      base.Dispose(disposing);
    }

  }
}
