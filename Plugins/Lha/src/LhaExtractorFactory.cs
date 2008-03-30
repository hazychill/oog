using System;
using System.IO;
using System.Drawing;
using Oog.Plugin;

namespace OogLha {
  public class LhaExtractorFactory : IExtractorFactory {

    public IExtractor Create(string path) {
      return new LhaExtractor(path);
    }

    public string[] SupportedExtensions {
      get { return new string[]{ ".lzh" }; }
    }

    public Image Icon {
      get {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LhaExtractorFactory));
        Bitmap bmp = ((System.Drawing.Bitmap)(resources.GetObject("icon_lha")));
        return bmp;
      }
    }
  }
}