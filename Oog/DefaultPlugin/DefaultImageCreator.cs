using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ImageSharp = SixLabors.ImageSharp;

namespace Oog.Plugin {
  public class DefaultImageCreator : IImageCreator {
#region IImageCreator Members

    public ImageSharp.Image GetImage(Stream data) {
      try {
        return ImageSharp.Image.Load(data);
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
          ".ico",
          ".tif"
          };
      }
    }

#endregion
  }
}
