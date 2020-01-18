using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.Win32;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace Oog.Plugin {
  public class FolderExtractorFactory : IExtractorFactory {

#region IExtractorFactory Members

    public IExtractor Create(string path) {
      if (Directory.Exists(path)) {
        try {
          return new FolderExtractor(path);
        }
        catch {
          return null;
        }
      }
      else {
        return null;
      }
    }

    public string[] SupportedExtensions {
      get { return new string[] { string.Empty }; }
    }

    public Image Icon {
      get {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderExtractorFactory));
        Bitmap bmp = ((System.Drawing.Bitmap)(resources.GetObject("icon_folder")));
        return bmp;
      }
    }
#endregion


  }
}
