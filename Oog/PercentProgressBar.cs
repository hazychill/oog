using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Oog {
  public class PercentProgressBar : Control {
    int max, min, val;
    Rectangle frameRect;
    Rectangle barRect;
    string percentText;
    Pen framePen;
    Brush barBrush;
    Brush textBrush;
    Brush backBrush;

    public PercentProgressBar() {
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      
      max = 100;
      min = 0;
      val = 0;

      framePen = new Pen(Color.Black);
      barBrush = new SolidBrush(Color.LightSteelBlue);
      textBrush = new SolidBrush(Color.DarkBlue);
      backBrush = new SolidBrush(Color.White);
    }

    protected override Size DefaultSize {
      get { return new Size(100, 18); }
    }


    protected override void OnPaintBackground(PaintEventArgs e) {
      e.Graphics.Clear(BackColor);
    }

    protected override void OnPaint(PaintEventArgs e) {
      if (!this.IsDisposed) {

        SizeF textSize = e.Graphics.MeasureString(percentText, Font);
        PointF p = new PointF(
          this.ClientRectangle.Top + (this.ClientRectangle.Width-textSize.Width) / 2,
          this.ClientRectangle.Left + (this.ClientRectangle.Height-textSize.Height) / 2);

        e.Graphics.FillRectangle(backBrush, frameRect);
        e.Graphics.DrawRectangle(framePen, frameRect);
        e.Graphics.FillRectangle(barBrush, barRect);
        e.Graphics.DrawString(percentText, Font, textBrush, p);
      }

      base.OnPaint(e);
    }

    private void SetDrawElement() {
      frameRect = new Rectangle(
        this.ClientRectangle.X,
        this.ClientRectangle.Y,
        this.ClientRectangle.Width - 1,
        this.ClientRectangle.Height - 3);

      int max = this.Maximum;
      int min = this.Minimum;
      int val = this.Value;
      int maxWidth = this.ClientRectangle.Width-2;
      barRect = new Rectangle(
        this.ClientRectangle.X+1,
        this.ClientRectangle.Y+1,
        maxWidth * (val-min) / (max-min),
        this.ClientRectangle.Height - 4);

      int percent = (val-min) * 100 / (max-min);
      percentText = string.Format("{0}%", percent);
    }

    private void DrawFrame(PaintEventArgs e) {
      Pen pen = new Pen(Color.Black);
      Rectangle rect = new Rectangle(
        this.ClientRectangle.Top+1,
        this.ClientRectangle.Left+1,
        this.ClientRectangle.Width - 2,
        this.ClientRectangle.Height - 4);
      e.Graphics.DrawRectangle(pen, rect);

    }

    public int Maximum {
      get { return max; }
      set {
        max = value;
        SetDrawElement();
        this.Invalidate();
      }
    }
    public int Minimum {
      get { return min; }
      set { min = value;
        SetDrawElement();
        this.Invalidate();
      }
    }
    public int Value {
      get { return val; }
      set {
        val = value;
        SetDrawElement();
        this.Invalidate();
      }
    }

    protected override void OnSizeChanged(EventArgs e) {
      if (!this.IsDisposed) {
        SetDrawElement();
        base.OnSizeChanged(e);
        this.Invalidate(true);
      }

    }

    protected override void Dispose(bool disposing) {
      framePen.Dispose();
      barBrush.Dispose();
      textBrush.Dispose();
      backBrush.Dispose();
      base.Dispose(disposing);
    }

  }
}