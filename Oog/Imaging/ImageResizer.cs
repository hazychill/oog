using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using ImageSharp = SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using static SixLabors.ImageSharp.ImageExtensions;

namespace Oog {

  public delegate Size Resizer(Size original, Size target);

  static class ImageResizer {

    public static Size OriginalSize(Size original, Size target) {
      return original;
    }

    public static Size TargetSize(Size original, Size target) {
      return target;
    }

    public static Size ShrinkHoldingRatio(Size original, Size target) {
      if (original.Width > target.Width || original.Height > target.Height) {
        float wRatio = (float)original.Width / (float)target.Width;
        float hRatio = (float)original.Height / (float)target.Height;
        float shrinkRatio = Math.Max(wRatio, hRatio);
        float sizedWidth = (float)original.Width / shrinkRatio;
        float sizedHeight = (float)original.Height / shrinkRatio;
        return new Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static Size ExpandHoldingRatio(Size original, Size target) {
      if (original.Width < target.Width && original.Height < target.Height) {
        float wRatio = (float)target.Width / (float)original.Width;
        float hRatio = (float)target.Height / (float)original.Height;
        float expandRatio = Math.Min(wRatio, hRatio);
        float sizedWidth = (float)original.Width * expandRatio;
        float sizedHeight = (float)original.Height * expandRatio;
        return new Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static Size FitScreenHoldingRatio(Size original, Size target) {
      if (original.Width > target.Width || original.Height > target.Height) {
        return ShrinkHoldingRatio(original, target);
      }
      else {
        return ExpandHoldingRatio(original, target);
      }
    }

    public static Size ShrinkWidthHoldingRatio(Size original, Size target) {
      float shrinkRatio = (float)original.Width / target.Width;
      if (shrinkRatio > 1f) {
        float sizedWidth = (float)original.Width / shrinkRatio;
        float sizedHeight = (float)original.Height / shrinkRatio;
        return new Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static Size FitWidth(Size original, Size target) {
      float shrinkRatio = (float)original.Width / target.Width;
      float sizedWidth = (float)original.Width / shrinkRatio;
      float sizedHeight = (float)original.Height / shrinkRatio;
      return new Size((int)sizedWidth, (int)sizedHeight);
    }

    public static Size ShrinkHeightHoldingRatio(Size original, Size target) {
      float shrinkRatio = (float)original.Height / target.Height;
      if (shrinkRatio > 1f) {
        float sizedWidth = (float)original.Width / shrinkRatio;
        float sizedHeight = (float)original.Height / shrinkRatio;
        return new Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static Size FitHeight(Size original, Size target) {
      float shrinkRatio = (float)original.Height / target.Height;
      float sizedWidth = (float)original.Width / shrinkRatio;
      float sizedHeight = (float)original.Height / shrinkRatio;
      return new Size((int)sizedWidth, (int)sizedHeight);
    }

    public static Size ExpandTwice(Size original, Size target) {
      return new Size(original.Width*2, original.Height*2);
    }

    public static Size ExpandTreeSecond(Size original, Size target) {
      return new Size(original.Width*3/2, original.Height*3/2);
    }

    public static Size ShrinkHalf(Size original, Size target) {
      return new Size(original.Width/2, original.Height/2);
    }

    public static Size ShrinkTwoThird(Size original, Size target) {
      return new Size(original.Width*2/3, original.Height*2/3);
    }

    public static Size ShrinkThreeFourth(Size original, Size target) {
      return new Size(original.Width*3/4, original.Height*3/4);
    }
    public static Size ShrinkFourFifth(Size original, Size target) {
      return new Size(original.Width*4/5, original.Height*4/5);
    }
    public static Size ShrinkThreeFifth(Size original, Size target) {
      return new Size(original.Width*3/5, original.Height*3/5);
    }

    public static Size SizeWidthScreenRatio80(Size original, Size target){
      var width = target.Width * 4 / 5;
      var height = width * original.Height / original.Width;
      return new Size(width, height);
    }

//     public static Resizer GetSizeFixedResizer(Size size) {
//       Size sizeTemp = size;
//       Resizer resizer = delegate(Size original, Size target) {
//         return sizeTemp;
//       };
//     }

    public static Image Resize(Image original, Size target, Resizer resizer, InterpolationMode interpolationMode) {
      Size changedSize = resizer(original.Size, target);
      if (changedSize == original.Size) {
        return original;
      }
      else {
        using (MemoryStream origStream = new MemoryStream()) {
          original.Save(origStream, ImageFormat.Png);
          origStream.Flush();
          origStream.Seek(0, SeekOrigin.Begin);
          using (var imshImage = ImageSharp.Image.Load(origStream)) {
            imshImage.Mutate(x => x.Resize(changedSize.Width, changedSize.Height));
            using (MemoryStream resizedStream = new MemoryStream()) {
              imshImage.SaveAsPng(resizedStream);
              resizedStream.Flush();
              resizedStream.Seek(0, SeekOrigin.Begin);
              using (Image resizedImage = Image.FromStream(resizedStream)) {
                return new Bitmap(resizedImage);
              }
            }
          }
        }
      }
    }

    public static Image Resize(Image original, Size target, Resizer resizer, InterpolationMode interpolationMode, bool rotate) {
      if (rotate) {
        Size changedSize = resizer(new Size(original.Height, original.Width), target);

        using (MemoryStream origStream = new MemoryStream()) {
          original.Save(origStream, ImageFormat.Png);
          origStream.Flush();
          origStream.Seek(0, SeekOrigin.Begin);
          using (var imshImage = ImageSharp.Image.Load(origStream)) {
            imshImage.Mutate(x => x.Rotate(RotateMode.Rotate90).Resize(changedSize.Width, changedSize.Height));
            using (MemoryStream resizedStream = new MemoryStream()) {
              imshImage.SaveAsPng(resizedStream);
              resizedStream.Flush();
              resizedStream.Seek(0, SeekOrigin.Begin);
              using (Image resizedImage = Image.FromStream(resizedStream)) {
                return new Bitmap(resizedImage);
              }
            }
          }
        }
      }
      else {

        Size changedSize = resizer(original.Size, target);

        if (changedSize == original.Size) {
          return original;
        }
        else {
          using (MemoryStream origStream = new MemoryStream()) {
            original.Save(origStream, ImageFormat.Png);
            origStream.Flush();
            origStream.Seek(0, SeekOrigin.Begin);
            using (var imshImage = ImageSharp.Image.Load(origStream)) {
              imshImage.Mutate(x => x.Resize(changedSize.Width, changedSize.Height));
              using (MemoryStream resizedStream = new MemoryStream()) {
                imshImage.SaveAsPng(resizedStream);
                resizedStream.Flush();
                resizedStream.Seek(0, SeekOrigin.Begin);
                using (Image resizedImage = Image.FromStream(resizedStream)) {
                  return new Bitmap(resizedImage);
                }
              }
            }
          }
        }
      }
    }
  }
}
