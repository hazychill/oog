using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Oog.Plugin;
using GDI = System.Drawing;
using ImageSharp = SixLabors.ImageSharp;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;

namespace Oog {
    public class OogUtil {
        private const string EX_EXT_PATTERN = "(.(?:jpe?g|png|bmp|gif|tiff?))$";

        private static string GetExExtension(string name) {
            string exExt;

            var match = Regex.Match(name, EX_EXT_PATTERN, RegexOptions.IgnoreCase);
            if (match.Success) {
                exExt = match.Groups[1].Value.ToLower();
                exExt = Regex.Replace(exExt, "^.", ".");
            }
            else {
                exExt = string.Empty;
            }

            return exExt;
        }

        public static string[] GetNames(IEnumerable<string> srcNames, Dictionary<string, IImageCreator> imageCreators) {
            var names = srcNames
                .Where(x => {
                    var ext = Path.GetExtension(x).ToLower();
                    if (imageCreators.ContainsKey(ext)) {
                        return true;
                    }
                    else {
                        var ext2 = GetExExtension(x);
                        if (imageCreators.ContainsKey(ext2)) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                }).ToArray();

            return names;
        }
        public static IImageCreator GetImageCreatorForName(Dictionary<string, IImageCreator> imageCreators, string name) {
            IImageCreator creator = null;

            var ext = Path.GetExtension(name);
            if (!imageCreators.TryGetValue(ext, out creator)) {
                var ext2 = GetExExtension(name);
                creator = imageCreators[ext2];
            }
            return creator;
        }

        public static GDI.Image GetImageFromSharpImage(ImageSharp.Image imshImage) {
            using (var ms = new MemoryStream()) {
                // imshImage.SaveAsPng(ms);
                imshImage.SaveAsJpeg(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return GDI.Image.FromStream(ms);
            }
        }

        public static ImageSharp.Image GetImageSharpImage(GDI.Image image) {
            using (MemoryStream origStream = new MemoryStream()) {
                image.Save(origStream, ImageFormat.Jpeg);
                origStream.Flush();
                origStream.Seek(0, SeekOrigin.Begin);
                return ImageSharp.Image.Load(origStream);
            }
        }

        public static ImageSharp.Size ImageSharpSize(GDI.Size gdiSize) {
            return new ImageSharp.Size(gdiSize.Width, gdiSize.Height);
        }
    }
}

