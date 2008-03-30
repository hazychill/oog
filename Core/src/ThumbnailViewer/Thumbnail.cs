using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Oog {
  class Thumbnail : IDisposable {
    private string name;
    private Image image;
    private bool selected;
    private bool imageCreated;

    private Color backColor;
    private Font font;
    private SolidBrush nameBrush;
    private Pen framePen;

    internal static readonly int NameHeight = 20;

    public Thumbnail() {
      this.name = "";
      this.image = null;
      this.selected = false;
      this.imageCreated = false;

      backColor = SystemColors.Window;
      font = SystemFonts.IconTitleFont;
      nameBrush = new SolidBrush(Color.DarkBlue);
      framePen = new Pen(Color.LightSteelBlue);
      framePen.Width = 4;
    }

    public static Thumbnail Create(string name) {
      Thumbnail thumbnail = new Thumbnail();
      thumbnail.Name = name;
      return thumbnail;
    }

    public static Thumbnail Create(string name, ThumbnailSettings settings) {
      Thumbnail thumbnail = new Thumbnail();
      thumbnail.Name = name;
      thumbnail.BackColor = settings.BackColor;
      return thumbnail;
    }
    

    public virtual void DrawThumbnail(Graphics g, Rectangle rect) {
      g.SetClip(rect);
      g.Clear(backColor);

      DrawImage(g, rect);
      DrawName(g, rect);
      DrawFrame(g, rect);
    }

    protected virtual void DrawImage(Graphics g, Rectangle rect) {
      if (image != null) {
        Point p = new Point(
          rect.X + (rect.Width-image.Width) / 2,
          rect.Y + (rect.Height-image.Height) / 2);

        g.DrawImage(image, new Rectangle(p, image.Size));
      }
    }

    protected virtual void DrawName(Graphics g, Rectangle rect) {
      Rectangle nameRect = new Rectangle(
        rect.X,
        rect.Y + rect.Height - NameHeight,
        rect.Width,
        NameHeight);

      using (Brush nameBackBrush = CreateNameBackBrush(nameRect)) {
        g.FillRectangle(nameBackBrush, nameRect);
      }

      string dispName = GetDispName(name);

      Size nameSize = g.MeasureString(dispName, font).ToSize();
      Point nameLocation;
      if (nameSize.Width <= nameRect.Width) {
        nameLocation = new Point(
          nameRect.X + (nameRect.Width-nameSize.Width)/2,
          nameRect.Y + (nameRect.Height-nameSize.Height)/2);
      }
      else {
        nameLocation = new Point(
          nameRect.X,
          nameRect.Y + (nameRect.Height-nameSize.Height)/2);
      }

      g.DrawString(dispName, font, nameBrush, nameLocation);
    }

    private Brush CreateNameBackBrush(Rectangle nameRect) {
      GraphicsPath path = new GraphicsPath();
      path.AddRectangle(nameRect);

      PathGradientBrush nameBackBrush = new PathGradientBrush(path);

      if (selected) {
        nameBackBrush.CenterColor = Color.Tomato;
      }
      else {
        nameBackBrush.CenterColor = Color.LightSteelBlue;
      }

      Color surroundColor = Color.Transparent;
      nameBackBrush.SurroundColors = new Color[] {
        surroundColor,
        surroundColor,
        surroundColor,
        surroundColor};

      return nameBackBrush;
    }

    private string GetDispName(string name) {
      int lastSeparator = Math.Max(name.LastIndexOf('/'), name.LastIndexOf('\\'));
      if (lastSeparator<=0 || name.Length-2<=lastSeparator) {
        return name;
      }
      else {
        return name.Substring(lastSeparator+1);
      }
    }

    protected virtual void DrawFrame(Graphics g, Rectangle rect) {
      if (selected) {
        g.DrawRectangle(framePen, new Rectangle(
          rect.X+1,
          rect.Y+1,
          rect.Width-2,
          rect.Height-2));
      }
    }

    public void SetImage(Image image) {
      this.image = image;
      imageCreated = true;
    }

    public void ClearImage() {
      if (this.Image != null) {
        this.Image.Dispose();
        this.Image = null;
      }
      imageCreated = false;
    }

    public string Name {
      get { return name; }
      set { name = value; }
    }

    public Image Image {
      get { return image; }
      set { image = value; }
    }

    public bool ImageCreated {
      get { return imageCreated; }
    }

    public bool Selected {
      get { return selected; }
      set { selected = value; }
    }

    public Color BackColor {
      get { return backColor; }
      set { backColor = value; }
    }

    public Color FrameColor {
      get { return framePen.Color; }
      set { framePen.Color = value; }
    }

    public float FrameWidth {
      get { return framePen.Width; }
      set { framePen.Width = value; }
    }

#region IDisposable Members

    public void Dispose() {
      font.Dispose();
      nameBrush.Dispose();
      framePen.Dispose();
    }

#endregion
  }
}
