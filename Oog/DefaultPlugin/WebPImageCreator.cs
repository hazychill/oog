using System.Diagnostics;
using System.IO;
using ImageSharp = SixLabors.ImageSharp;

namespace Oog.Plugin;

public class WebPImageCreator : IImageCreator {
    public string[] SupportedExtensions => new [] { ".webp" };

    public ImageSharp.Image GetImage(Stream data) {
        try {
            return ImageSharp.Image.Load(data);
            
        }
        catch (System.Exception e) {
            Debug.WriteLine(e);
            return null;
        }
    }
}