using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Oog.Plugin;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using ImageSharp = SixLabors.ImageSharp;
using GDI = System.Drawing;
using SixLabors.ImageSharp.Processing;

namespace Oog {
  public class ThumbnailViewer : ScrollableControl {

    public event ProgressChangedEventHandler ProgressChanged;

#region Fields

    private Thumbnail[] thumbnails;
    private int rows, cols;
    private GDI.Size cellSize;
    private ThumbnailSettings thumbnailSettings;

    private IExtractor extractor;
    private BackgroundWorker imageCreateWorker;
    private Dictionary<string, IImageCreator> imageCreators;
    private ManualResetEventSlim workerBlocker;

#endregion

#region Constructor

    public ThumbnailViewer() {
      SetStyle(ControlStyles.AllPaintingInWmPaint
               | ControlStyles.UserPaint
               | ControlStyles.OptimizedDoubleBuffer,
               true);

      InitializeWorker();

      BackColor = GDI.SystemColors.Window;
      thumbnails = new Thumbnail[0];

      thumbnailSettings = ThumbnailSettings.Default;

      workerBlocker = new ManualResetEventSlim(true);
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

    public ManualResetEventSlim WorkerBlocker {
      get { return workerBlocker; }
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
      return (oldSettings.Size != newSettings.Size) || (oldSettings.Resampler != newSettings.Resampler);
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

      GDI.Graphics g = e.Graphics;
            for (int row = rowStart; row <= rowEnd; row++) {
        for (int col = 0; col < cols; col++) {
          int index = cols*row + col;
          if (0<=index && index < thumbnails.Length) {
            GDI.Point cellLocation = new GDI.Point(col*cellSize.Width, row*cellSize.Height+this.AutoScrollPosition.Y);
            GDI.Rectangle cellRect = new GDI.Rectangle(cellLocation, cellSize);
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

      cellSize = new GDI.Size(this.ClientSize.Width/cols, thumbnailSettings.Size.Height);

      this.AutoScrollMinSize = new GDI.Size(cols*cellSize.Width, rows*cellSize.Height);
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

    private Task ClearThumbnails() {
      var tempThumbnails = thumbnails;
      thumbnails = null;
      var tempExtractor = extractor;
      extractor = null;
      selectedIndex = -1;

      var temp = workerBlocker;
      workerBlocker = new ManualResetEventSlim(true);
      temp.Dispose();

      return Task.Factory.StartNew(() => {
        try {
          foreach (Thumbnail thumbnail in tempThumbnails) {
            if (thumbnail.Image != null) {
              thumbnail.Image.Dispose();
            }
            thumbnail.Dispose();
          }
          if (tempExtractor != null) {
            tempExtractor.Close();
          }
        }
        catch (Exception e) {
          Debug.WriteLine(e);
          throw;
        }
      });
    }

#region Create image background

    void imageCreateWorker_DoWork(object sender, DoWorkEventArgs e) {
      //Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
      Thread.CurrentThread.Priority = thumbnailSettings.ThumbnailThreadPriority;

      imageCreateWorker.ReportProgress(0);
      var prevPercentage = 0;

      int count = thumbnails.Length;
      int current = 0;

      bool doProgressReport = true;

      if (count == 0) return;

      var progressReportTask = Task.Factory.StartNew(() => {
        while (doProgressReport) {
          Thread.Sleep(100);
          var percentage = (current)*100/count;
          if (percentage > prevPercentage) {
            try {
              imageCreateWorker.ReportProgress(percentage);
              prevPercentage = percentage;
            }
            catch (InvalidOperationException ioe) {
              //TODO
              Debug.WriteLine(ioe);
              break;
            }
          }
          if (percentage >= 100) {
            break;
          }
          try {
            workerBlocker.Wait();
          }
          catch (ObjectDisposedException disposedError) {
            Debug.WriteLine(disposedError);
          }
        }
      });

      var ie2 = extractor as IExtractor2;

      if (ie2 != null && ie2.SynchronizationRequired == false) {

        var dop = Math.Max(1, Environment.ProcessorCount-1);

        var result = Parallel.ForEach(
          new OneByOnePartitioner<int>(EnumIndexAsync().GetConsumingEnumerable()),
          new ParallelOptions() { MaxDegreeOfParallelism = dop },
          (index, loopState) => {
            if (loopState.ShouldExitCurrentIteration) return;

            if (imageCreateWorker.CancellationPending) {
              loopState.Stop();
              return;
            }
            try {
              workerBlocker.Wait();
            }
            catch (ObjectDisposedException disposedError) {
              Debug.WriteLine(disposedError);
            }
            CreateThumbnailImage(index);
            Interlocked.Increment(ref current);
        });

        if (!result.IsCompleted) {
          e.Cancel = true;
        }
      }
      else {
        foreach (int index in EnumIndex()) {

          if (imageCreateWorker.CancellationPending) {
            e.Cancel = true;
            break;
          }
          CreateThumbnailImage(index);
          current++;
        }
      }

      doProgressReport = false;




      if (!imageCreateWorker.CancellationPending) {
        imageCreateWorker.ReportProgress(100);
      }
    }


    private BlockingCollection<int> EnumIndexAsync() {
//      var dop = Math.Max(1, Environment.ProcessorCount-1);
      var dop = 1;
      var queue = new BlockingCollection<int>(dop);

      var produceTask = Task.Factory.StartNew(() => {
        try {
          foreach (var index in EnumIndex()) {
            if (imageCreateWorker != null && imageCreateWorker.CancellationPending) {
              break;
            }
            queue.Add(index);
          }
        }
        finally {
          queue.CompleteAdding();
        }
//       }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
      });

      return queue;
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
        IImageCreator creator = OogUtil.GetImageCreatorForName(imageCreators, name);

        using (ImageSharp.Image originalRaw = creator.GetImage(data)) {
          if (originalRaw == null) {
            thumbnail.SetImage(null);
            return;
          }

          ImageSharp.Image resized = ImageResizer.Resize(originalRaw, OogUtil.ImageSharpSize(thumbnailSettings.Size), ImageResizer.ShrinkHoldingRatio, thumbnailSettings.Resampler);
          var actuallyResized = resized == originalRaw;
          try {
            var gdiImage = OogUtil.GetImageFromSharpImage(resized);
            thumbnail.SetImage(gdiImage);
          }
          finally {
            if (actuallyResized) {
              resized.Dispose();
            }
          }

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
        int delta;
        if (IntPtr.Size == 4) {
          delta = m.WParam.ToInt32() >> 16;
        }
        else if (IntPtr.Size == 8) {
          delta = (int)m.WParam.ToInt64();
          delta = delta >> 16;
        }
        else {
          delta = 0;
        }
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
      this.AutoScrollPosition = new GDI.Point(0, top);
    }

#endregion

#region Key events

    protected override bool ProcessDialogKey(Keys keyData) {
      if ((keyData&Keys.Alt)!=0 || (keyData&Keys.Control)!=0 || (keyData&Keys.Shift)!=0) {
        return base.ProcessDialogKey(keyData);
      }
      
      switch (keyData) {
      case Keys.Up:
      case Keys.K:
        SelectUp();
        return true;
      case Keys.Down:
      case Keys.J:
        SelectDown();
        return true;
      case Keys.Left:
      case Keys.H:
        SelectLeft();
        return true;
      case Keys.Right:
      case Keys.L:
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
      GDI.Rectangle rect = new GDI.Rectangle(
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
        this.AutoScrollPosition = new GDI.Point(0, row*cellSize.Height);
      }
      else if (this.AutoScrollPosition.Y + (row+1)*cellSize.Height > this.Height) {
        this.AutoScrollPosition = new GDI.Point(0, row*cellSize.Height - this.Height + cellSize.Height);
      }
    }

    private void WaitWorkerCancellation(MethodInvoker method) {
      if (imageCreateWorker.IsBusy) {
        completed = method;
        workerBlocker.Set();
        imageCreateWorker.CancelAsync();
      }
      else {
        method();
      }
    }

    protected override void Dispose(bool disposing) {
      ClearThumbnails();
      if (disposing) {
        workerBlocker.Dispose();
        imageCreateWorker.Dispose();
      }
      base.Dispose(disposing);
    }

  }
}
