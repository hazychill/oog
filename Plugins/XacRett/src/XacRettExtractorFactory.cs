using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Oog.Plugin {
  public class XacRettExtractorFactory : IExtractorFactory {
    public IExtractor Create(string path) {
      return new XacRettExtractor(path);
    }
    
    public string[] SupportedExtensions {
      get {
        return new string[] {
          ".cab",
          ".ace",
          ".arj",
          ".yz1",
          ".bga",
          ".gca",
          ".imp",
          ".zoo",
          ".zrc",
          ".cpt",
          ".pit",
          ".arg",
          ".asd",
          ".dzip",
          ".tar",
          ".cpio",
          ".rpm",
          ".shar",
          ".ar",
          ".zac"
        };
      }
    }

    
    public Image Icon {
      get {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XacRettExtractorFactory));
        Bitmap bmp = ((System.Drawing.Bitmap)(resources.GetObject("icon_xac")));
        return bmp;
      }
    }
  }
}