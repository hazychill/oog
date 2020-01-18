using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace Oog.Plugin {
  public class ZipExtractorFactory : IExtractorFactory {
#region IExtractorFactory Members

    public IExtractor Create(string path) {
      try {
        return new ZipExtractor(path);
      }
      catch {
        return new EmptyExtractor();
      }
    }

    public string[] SupportedExtensions {
      get { return new string[] { ".zip" }; }
    }

    public Image Icon {
      get {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZipExtractorFactory));
        Bitmap bmp = ((System.Drawing.Bitmap)(resources.GetObject("icon_zip")));
        return bmp;
      }
    }

#endregion
  }
}
