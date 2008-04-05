using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Oog.Plugin {
  public class TextImageCreator : IImageCreator {

    const int WIDTH = 300;
    const int HEIGHT = 300;
    
    public Image GetImage(Stream data) {
      string text = "";
      try {
        text = GetText(data);
      }
      catch {
        return null;
      }
      Bitmap bmp = new Bitmap(WIDTH, HEIGHT);
      using (Graphics g = Graphics.FromImage(bmp))
      using (Brush brush = new SolidBrush(Color.Black))
      using (Font font = SystemFonts.DefaultFont){
        g.Clear(Color.White);
        g.DrawString(text, font, brush, new Point(0, 0));
      }

      return bmp;
    }

    const int BUFFER_LENGTH = 1024;

    private string GetText(Stream data) {
      StreamReader reader = new StreamReader(data, Encoding.Default);
      char[] buffer = new char[BUFFER_LENGTH];
      int count = reader.Read(buffer, 0, BUFFER_LENGTH);
      return new string(buffer, 0, count);
    }

    static readonly string[] exts = new string[] {
      ".txt",
      ".html",
      ".htm",
      ".css",
      ".js",
      ".shtml",
      ".vbs",
      ".xml",
      ".ini",
      ".c",
      ".cpp",
      ".cs",
      ".bat",
      ".log",
      ".tex"
      };

    public string[] SupportedExtensions {
      get {
        return exts;
      }
    }
  }
}