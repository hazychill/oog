using System;
using System.Drawing;
using System.IO;

namespace Oog.Plugin {
    public interface IImageCreator {
        Image GetImage(Stream data);
        string[] SupportedExtensions { get;}
    }
}
