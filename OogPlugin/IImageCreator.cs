using System;
using System.IO;
using ImageSharp = SixLabors.ImageSharp;

namespace Oog.Plugin {
    public interface IImageCreator {
        ImageSharp.Image GetImage(Stream data);
        string[] SupportedExtensions { get;}
    }
}
