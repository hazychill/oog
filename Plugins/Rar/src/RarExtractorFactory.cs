using System;
using System.IO;
using System.Drawing;
using Oog.Plugin;

namespace Oog.Plugin {
  public class RarExtractorFactory : IExtractorFactory {

    public IExtractor Create(string path) {
      return new RarExtractor(path);
    }

    public string[] SupportedExtensions {
      get { return new string[]{ ".rar" }; }
    }

    public Image Icon {
      get {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RarExtractorFactory));
        Bitmap bmp = ((System.Drawing.Bitmap)(resources.GetObject("icon_rar")));
        return bmp;
      }
    }
  }
}