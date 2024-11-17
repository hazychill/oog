using System.Drawing;
using System.IO;
using ImageSharp = SixLabors.ImageSharp;

namespace Oog.Plugin;

public class WebPImageCreator : IImageCreator {
    public string[] SupportedExtensions => new [] { ".webp" };

    public System.Drawing.Image GetImage(Stream data) {
        using (var imshImage = ImageSharp.Image.Load(data)) {
            return OogUtil.GetImageFromSharpImage(imshImage);
        }
        
    }
}