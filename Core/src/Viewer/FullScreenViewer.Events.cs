using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Oog.Viewer {
  partial class FullScreenViewer {

#region Mouse events

    MouseEventHandler scrollPicture;

    MouseEventHandler onMouseDown;
    MouseEventHandler onMouseUp;
    MouseEventHandler onMouseMove;

    Point start;
    Point anchor;


    private void OnLoad(object sender, EventArgs e) {
      contextMenuStrip1.Show();
      contextMenuStrip1.Hide();
      SetImage();
    }

    void StartScroll(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        start = GetScreenPoint(sender, e);
        anchor = picture.Location;

        //MouseDown後マウスが動いたらスクロールさせる
        onMouseMove = ScrollPictureFirst;
        //MouseDown後マウスが動かずにMouseUpされたときは、画像を入れ替える
        if (currentIndex != 0) onMouseUp = PrevImage;
      }
      else if (e.Button == MouseButtons.Right) {
        start = GetScreenPoint(sender, e);
        //マウス右ボタンではスクロールさせない
        //マウスが動いたかどうかのチェックだけ
        onMouseMove = CheckMove;
        //MouseDown後マウスが動かずにMouseUpされたときは、画像を入れ替える
        if (currentIndex != imageNames.Length-1) onMouseUp = NextImage;
      }
    }

    void PrevImage(object sender, MouseEventArgs e) {
      onMouseMove = CheckMenuPosition;
      onMouseUp = DoNothing;
      if (currentIndex != 0) {
        PrevImage();
      }
    }

    void NextImage(object sender, MouseEventArgs e) {
      onMouseMove = CheckMenuPosition;
      onMouseUp = DoNothing;
      if (currentIndex != imageNames.Length-1) {
        NextImage();
      }
    }

    void CheckMove(object sender, MouseEventArgs e) {
      Point current = GetScreenPoint(sender, e);
      Size diff = new Size(current.X - start.X, current.Y - start.Y);
      if (diff.Width*diff.Width + diff.Height*diff.Height > 100) {
        onMouseUp = EndScrollPicture;
      }
    }

    private static Point GetScreenPoint(object sender, MouseEventArgs e) {
      Control c = sender as Control;
      Point current = c.PointToScreen(new Point(e.X, e.Y));
      return current;
    }

    //MouseMoveを感知しMouseUp時に画像を入れ替えないようにする
    void ScrollPictureFirst(object sender, MouseEventArgs e) {
      Point current = GetScreenPoint(sender, e);
      Size diff = new Size(current.X - start.X, current.Y - start.Y);
      if (diff.Width*diff.Width + diff.Height*diff.Height > 100) {
        onMouseUp = EndScrollPicture;
        onMouseMove = scrollPicture;
      }
      scrollPicture(sender, e);
    }

    //縦横両方にスクロールを許す
    void ScrollBoth(object sender, MouseEventArgs e) {
      Point current = GetScreenPoint(sender, e);
      Size diff = new Size(current.X - start.X, current.Y - start.Y);
      ScrollBoth(diff);
    }

    private void ScrollBoth(Size diff) {
      Point target = anchor + diff;
      Size screen = Screen.PrimaryScreen.Bounds.Size;
      int xMin = target.X;
      int xMax = target.X + picture.Width;
      int yMin = target.Y;
      int yMax = target.Y + picture.Height;
      if (xMin > 0) {
        target.X = 0;
      }
      else if (xMax < screen.Width) {
        target.X = screen.Width - picture.Width;
      }
      if (yMin > 0) {
        target.Y = 0;
      }
      else if (yMax < screen.Height) {
        target.Y = screen.Height - picture.Height;
      }
      picture.Location = target;
    }

    //水平方向にだけスクロールさせる
    void ScrollHorizontal(object sender, MouseEventArgs e) {
      Point current = GetScreenPoint(sender, e);
      Size diff = new Size(current.X - start.X, 0);
      ScrollHorizontal(diff);
    }

    private void ScrollHorizontal(Size diff) {
      Size screen = Screen.PrimaryScreen.Bounds.Size;
      Point target = anchor + diff;
      int xMin = target.X;
      int xMax = target.X + picture.Width;
      if (xMin > 0) {
        target.X = 0;
      }
      else if (xMax < screen.Width) {
        target.X = screen.Width - picture.Width;
      }
      picture.Location = target;
    }

    //垂直方向にだけスクロールさせる
    void ScrollVirtical(object sender, MouseEventArgs e) {
      Point current = GetScreenPoint(sender, e);
      Size diff = new Size(0, current.Y - start.Y);
      ScrollVirtical(diff);
    }

    private void ScrollVirtical(Size diff) {
      Point target = anchor + diff;
      Size screen = Screen.PrimaryScreen.Bounds.Size;
      int yMin = target.Y;
      int yMax = target.Y + picture.Height;
      if (yMin > 0) {
        target.Y = 0;
      }
      else if (yMax < screen.Height) {
        target.Y = screen.Height - picture.Height;
      }
      picture.Location = target;
    }

    void EndScrollPicture(object sender, MouseEventArgs e) {
      onMouseMove = CheckMenuPosition;
    }

    Point menuPoint = new Point(Screen.PrimaryScreen.Bounds.Width-1, 0);

    void CheckMenuPosition(object sender, MouseEventArgs e) {
      Point p = GetScreenPoint(sender, e);
      if (p == menuPoint) {
        CheckSize();
        CheckQuality();
        contextMenuStrip1.Show(menuPoint);
      }
    }

    private void CheckQuality() {
      if (settings.InterpolationMode == InterpolationMode.High) {
        menuQualityHigh.Checked = true;
        menuQualityMiddle.Checked = false;
        menuQualityLow.Checked = false;
      }
      else if (settings.InterpolationMode == InterpolationMode.Low) {
        menuQualityHigh.Checked = false;
        menuQualityMiddle.Checked = true;
        menuQualityLow.Checked = false;
      }
      else if (settings.InterpolationMode == InterpolationMode.NearestNeighbor) {
        menuQualityHigh.Checked = false;
        menuQualityMiddle.Checked = false;
        menuQualityLow.Checked = true;
      }
      else {
        menuQualityHigh.Checked = false;
        menuQualityMiddle.Checked = false;
        menuQualityLow.Checked = false;
      }
    }

    private void CheckSize() {
      if (settings.Resizer == new Resizer(ImageResizer.ShrinkHoldingRatio)) {
        menuSizeScreen.Checked = true;
        menuSizeOriginal.Checked = false;
        menuSizeAdjustWidth.Checked = false;
      }
      else if (settings.Resizer == new Resizer(ImageResizer.OriginalSize)) {
        menuSizeScreen.Checked = false;
        menuSizeOriginal.Checked = true;
        menuSizeAdjustWidth.Checked = false;
      }
      else if (settings.Resizer == new Resizer(ImageResizer.ShrinkWidthHoldingRatio)) {
        menuSizeScreen.Checked = false;
        menuSizeOriginal.Checked = false;
        menuSizeAdjustWidth.Checked = true;
      }
      else {
        menuSizeScreen.Checked = false;
        menuSizeOriginal.Checked = false;
        menuSizeAdjustWidth.Checked = false;
      }
    }

    void OnMouseWheel(object sender, MouseEventArgs e) {
      const int WHEEL_DELTA = 120;
      int count = e.Delta / WHEEL_DELTA;

      start = new Point(0, 0);
      anchor = picture.Location;
      MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks, 0, 120*count, e.Delta);
      scrollPicture(sender, me);
    }


    void OnMouseDown(object sender, MouseEventArgs e) { onMouseDown(sender, e); }
    void OnMouseUp(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Middle) {
        Close();
      }
      onMouseUp(sender, e);
    }
    void OnMouseMove(object sender, MouseEventArgs e) { onMouseMove(sender, e); }

    void DoNothing(object sender, MouseEventArgs e) { }

#endregion



#region Key events


    private void InitializeKeyEventMap() {
      keyEventMap = new Dictionary<Keys, MethodInvoker>();
      keyEventMap.Add(Keys.Enter,     SmartScroll);
      keyEventMap.Add(Keys.Space,     SmartScroll);
      keyEventMap.Add(Keys.B,         SmartScrollBack);
      keyEventMap.Add(Keys.Decimal,   SmartScrollBack);
      keyEventMap.Add(Keys.ShiftKey,  SmartScrollBack);
      keyEventMap.Add(Keys.Left,      PrevImage);
      keyEventMap.Add(Keys.Right,     NextImage);
      keyEventMap.Add(Keys.Up,        ScrollUp);
      keyEventMap.Add(Keys.Down,      ScrollDown);
      keyEventMap.Add(Keys.Apps,      ShowMenu);
      keyEventMap.Add(Keys.Home,      ScrollRightTop);
      keyEventMap.Add(Keys.End,       ScrollLeftTop);
      keyEventMap.Add(Keys.Escape,    Close);
    }

    private void OnKeyDown(object sender, KeyEventArgs e) {
      MethodInvoker handlerFunc;
      if (keyEventMap.TryGetValue(e.KeyCode, out handlerFunc)) {
        handlerFunc();
      }
    }

    private void ShowMenu() {
        CheckSize();
        CheckQuality();
        contextMenuStrip1.Show(menuPoint);
    }

    private void SmartScroll() {
      //int vDiff = this.Height;
      int vDiff = 400;
      if (picture.Top+picture.Height > this.Height) {
        start = new Point(0, 0);
        anchor = picture.Location;
        MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, 0, -vDiff, 0);
        scrollPicture(this, e);
      }
      else if (picture.Left < 0) {
        start = new Point(0, 0);
        anchor = picture.Location;
        MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, this.Width, -picture.Top, 0);
        scrollPicture(this, e);
      }
      else {
        NextImage(null, null);
      }
    }

    private void SmartScrollBack() {
      //int vDiff = this.Height;
      int vDiff = 400;
      if (picture.Top < 0) {
        start = new Point(0, 0);
        anchor = picture.Location;
        MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, 0, vDiff, 0);
        scrollPicture(this, e);
      }
      else if (picture.Left+picture.Width > this.Width) {
        start = new Point(this.Width, -picture.Top);
        anchor = picture.Location;
        MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        scrollPicture(this, e);
      }
      else {
        PrevImage(null, null);
      }
    }

    private void ShowMessage(string text) {
      messageLabel.Text = text;
      messageLabel.Location = new Point((this.Width-messageLabel.Width)/2, (this.Height-messageLabel.Height)/2);
      messageLabel.Visible = true;

      MethodInvoker waitAndHide = delegate {
        System.Threading.Thread.Sleep(1*1000);
        MethodInvoker hideMessageLabel = delegate {
          messageLabel.Visible = false;
        };
        this.Invoke(hideMessageLabel);
      };
      waitAndHide.BeginInvoke(delegate(IAsyncResult ar) { waitAndHide.EndInvoke(ar); }, null);
    }

    private void ScrollDown() {
      start = new Point(0, 0);
      anchor = picture.Location;
      MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, 0, -200, 0);
      scrollPicture(this, e);
    }

    private void ScrollUp() {
      start = new Point(0, 0);
      anchor = picture.Location;
      MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, 0, 200, 0);
      scrollPicture(this, e);
    }

    private void ScrollLeftTop() {
      SetPictureLocation(PictureLocation.LeftTop);
    }

    private void ScrollRightTop() {
       SetPictureLocation(PictureLocation.RightTop);
   }

#endregion

#region Menu events

    private void MoveToFirstImage(object sender, EventArgs e) {
      currentIndex = 0;
      SetImage();
    }

    private void MoveToLastImage(object sender, EventArgs e) {
      currentIndex = imageNames.Length-1;
      SetImage();
    }

    private void ChangeSizeMode(object sender, EventArgs e) {
      ToolStripMenuItem c = sender as ToolStripMenuItem;

      Resizer resizer = (Resizer)c.Tag;
      settings.Resizer = resizer;
      settings.Save();

      if (picture.Image != null) {
        Size size = picture.Image.Size;
        Size originalSize;
        if (originalSizes.TryGetValue(currentIndex, out originalSize)) {
          if (resizer(originalSize, this.Size) == size) {
            return;
          }
        }
      }
      
      SetImage();
      //new FullScreenViewerSettings(resizer, interpolationMode).Save();
    }

    private void ChangeQuality(object sender, EventArgs e) {
      ToolStripMenuItem c = sender as ToolStripMenuItem;
      settings.InterpolationMode = (InterpolationMode)c.Tag;
      SetImage();
      settings.Save();
      //new FullScreenViewerSettings(resizer, interpolationMode).Save();
    }

    private void menuRotate_Click(object sender, EventArgs e) {
      menuRotate.Checked = !menuRotate.Checked;
      SetImage();
    }


#endregion
  }
}
