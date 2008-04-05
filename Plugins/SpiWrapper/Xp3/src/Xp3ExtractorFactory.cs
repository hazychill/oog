using System;
using System.Collections.Generic;
using System.Text;
using Oog.Plugin;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace OogXp3 {
  public class Xp3ExtractorFactory : IExtractorFactory {
#region IExtractorFactory Members

    public IExtractor Create(string path) {
      return new Xp3Extractor(path);
    }

    public string[] SupportedExtensions {
      get { return new string[] { ".xp3" }; }
    }

    public Image Icon {
      get {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Xp3ExtractorFactory));
        Bitmap bmp = ((System.Drawing.Bitmap)(resources.GetObject("icon_xp3")));
        return bmp;
      }
    }

#endregion

  }
}
