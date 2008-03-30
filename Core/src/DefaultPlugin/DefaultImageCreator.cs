using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Oog.Plugin {
  public class DefaultImageCreator : IImageCreator {
#region IImageCreator Members

    public Image GetImage(Stream data) {
      try {
        return Image.FromStream(data);
        //Image original = Image.FromStream(data);
        //Bitmap bmp = new Bitmap(original);
        //original.Dispose();
        //return bmp;
      }
      catch {
        return null;
      }
    }

    public string[] SupportedExtensions {
      get {
        return new string[] {
          ".jpg",
          ".jpeg",
          ".png",
          ".gif",
          ".bmp",
          ".ico"
          };
      }
    }

#endregion
  }
}
