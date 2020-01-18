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

    bool keyDownThenMouseMove = false;

    private void OnLoad(object sender, EventArgs e) {
      contextMenuStrip1.Show();
      contextMenuStrip1.Hide();
      SetImage();
    }

    void StartScroll(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        start = GetScreenPoint(sender, e);
        anchor = picture.Location;

        //MouseDown��}�E�X����������X�N���[��������
        onMouseMove = ScrollPictureFirst;
        //MouseDown��}�E�X����������MouseUp���ꂽ�Ƃ��́A�摜�����ւ���
        if (currentIndex != 0) onMouseUp = PrevImage;
      }
      else if (e.Button == MouseButtons.Right) {
        start = GetScreenPoint(sender, e);
        //�}�E�X�E�{�^���ł̓X�N���[�������Ȃ�
        //�}�E�X�����������ǂ����̃`�F�b�N����
        onMouseMove = CheckMove;
        //MouseDown��}�E�X����������MouseUp���ꂽ�Ƃ��́A�摜�����ւ���
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

    //MouseMove�����m��MouseUp���ɉ摜�����ւ��Ȃ��悤�ɂ���
    void ScrollPictureFirst(object sender, MouseEventArgs e) {
      Point current = GetScreenPoint(sender, e);
      Size diff = new Size(current.X - start.X, current.Y - start.Y);
      if (diff.Width*diff.Width + diff.Height*diff.Height > 100) {
        onMouseUp = EndScrollPicture;
        onMouseMove = scrollPicture;
      }
      scrollPicture(sender, e);
    }

    //�c�������ɃX�N���[��������
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

    //���������ɂ����X�N���[��������
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

    //���������ɂ����X�N���[��������
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
      menuQualityHigh.Checked = false;
      menuQualityMiddle.Checked = false;
      menuQualityLow.Checked = false;

      if (settings.InterpolationMode == InterpolationMode.High) {
        menuQualityHigh.Checked = true;
      }
      else if (settings.InterpolationMode == InterpolationMode.Low) {
        menuQualityMiddle.Checked = true;
      }
      else if (settings.InterpolationMode == InterpolationMode.NearestNeighbor) {
        menuQualityLow.Checked = true;
      }
    }

    private void CheckSize() {
      menuSizeScreen.Checked = false;
      menuSizeOriginal.Checked = false;
      menuSizeAdjustWidth.Checked = false;

      if (settings.Resizer == new Resizer(ImageResizer.ShrinkHoldingRatio)) {
        menuSizeScreen.Checked = true;
      }
      else if (settings.Resizer == new Resizer(ImageResizer.OriginalSize)) {
        menuSizeOriginal.Checked = true;
      }
      else if (settings.Resizer == new Resizer(ImageResizer.ShrinkWidthHoldingRatio)) {
        menuSizeAdjustWidth.Checked = true;
      }
      else {
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
    void OnMouseMove(object sender, MouseEventArgs e) {
      if (keyDownThenMouseMove == true) {
        keyDownThenMouseMove = false;
      }
      else {
        ShowCursor();
      }
      onMouseMove(sender, e);
    }

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
      if (e.KeyCode == Keys.Apps) {
        ShowCursor();
      }
      else {
        HideCursor();
      }
      keyDownThenMouseMove = true;

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
