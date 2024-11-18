using System;
using System.Collections.Generic;
using GDI = System.Drawing;
using System.IO;
using ImageSharp = SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static SixLabors.ImageSharp.ImageExtensions;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Oog {

  public delegate ImageSharp.Size Resizer(ImageSharp.Size original, ImageSharp.Size target);

  static class ImageResizer {

    static Dictionary<GDI.Drawing2D.InterpolationMode, IResampler> resamplerSelector;

    static ImageResizer() {
      resamplerSelector = new Dictionary<GDI.Drawing2D.InterpolationMode, IResampler>();
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.Bicubic, KnownResamplers.Bicubic);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.Bilinear, KnownResamplers.Triangle);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.Default, KnownResamplers.Bicubic);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.High, KnownResamplers.Bicubic);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.HighQualityBicubic, KnownResamplers.Bicubic);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.HighQualityBilinear, KnownResamplers.Bicubic);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.Invalid, KnownResamplers.Bicubic);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.Low, KnownResamplers.NearestNeighbor);
      resamplerSelector.Add(GDI.Drawing2D.InterpolationMode.NearestNeighbor, KnownResamplers.NearestNeighbor);
    }

    public static ImageSharp.Size OriginalSize(ImageSharp.Size original, ImageSharp.Size target) {
      return original;
    }

    public static ImageSharp.Size TargetSize(ImageSharp.Size original, ImageSharp.Size target) {
      return target;
    }

    public static ImageSharp.Size ShrinkHoldingRatio(ImageSharp.Size original, ImageSharp.Size target) {
      if (original.Width > target.Width || original.Height > target.Height) {
        float wRatio = (float)original.Width / (float)target.Width;
        float hRatio = (float)original.Height / (float)target.Height;
        float shrinkRatio = Math.Max(wRatio, hRatio);
        float sizedWidth = (float)original.Width / shrinkRatio;
        float sizedHeight = (float)original.Height / shrinkRatio;
        return new ImageSharp.Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static ImageSharp.Size ExpandHoldingRatio(ImageSharp.Size original, ImageSharp.Size target) {
      if (original.Width < target.Width && original.Height < target.Height) {
        float wRatio = (float)target.Width / (float)original.Width;
        float hRatio = (float)target.Height / (float)original.Height;
        float expandRatio = Math.Min(wRatio, hRatio);
        float sizedWidth = (float)original.Width * expandRatio;
        float sizedHeight = (float)original.Height * expandRatio;
        return new ImageSharp.Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static ImageSharp.Size FitScreenHoldingRatio(ImageSharp.Size original, ImageSharp.Size target) {
      if (original.Width > target.Width || original.Height > target.Height) {
        return ShrinkHoldingRatio(original, target);
      }
      else {
        return ExpandHoldingRatio(original, target);
      }
    }

    public static ImageSharp.Size ShrinkWidthHoldingRatio(ImageSharp.Size original, ImageSharp.Size target) {
      float shrinkRatio = (float)original.Width / target.Width;
      if (shrinkRatio > 1f) {
        float sizedWidth = (float)original.Width / shrinkRatio;
        float sizedHeight = (float)original.Height / shrinkRatio;
        return new ImageSharp.Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static ImageSharp.Size FitWidth(ImageSharp.Size original, ImageSharp.Size target) {
      float shrinkRatio = (float)original.Width / target.Width;
      float sizedWidth = (float)original.Width / shrinkRatio;
      float sizedHeight = (float)original.Height / shrinkRatio;
      return new ImageSharp.Size((int)sizedWidth, (int)sizedHeight);
    }

    public static ImageSharp.Size ShrinkHeightHoldingRatio(ImageSharp.Size original, ImageSharp.Size target) {
      float shrinkRatio = (float)original.Height / target.Height;
      if (shrinkRatio > 1f) {
        float sizedWidth = (float)original.Width / shrinkRatio;
        float sizedHeight = (float)original.Height / shrinkRatio;
        return new ImageSharp.Size((int)sizedWidth, (int)sizedHeight);
      }
      else {
        return original;
      }
    }

    public static ImageSharp.Size FitHeight(ImageSharp.Size original, ImageSharp.Size target) {
      float shrinkRatio = (float)original.Height / target.Height;
      float sizedWidth = (float)original.Width / shrinkRatio;
      float sizedHeight = (float)original.Height / shrinkRatio;
      return new ImageSharp.Size((int)sizedWidth, (int)sizedHeight);
    }

    public static ImageSharp.Size ExpandTwice(ImageSharp.Size original, ImageSharp.Size target) {
      return new ImageSharp.Size(original.Width*2, original.Height*2);
    }

    public static ImageSharp.Size ExpandTreeSecond(ImageSharp.Size original, ImageSharp.Size target) {
      return new ImageSharp.Size(original.Width*3/2, original.Height*3/2);
    }

    public static ImageSharp.Size ShrinkHalf(ImageSharp.Size original, ImageSharp.Size target) {
      return new ImageSharp.Size(original.Width/2, original.Height/2);
    }

    public static ImageSharp.Size ShrinkTwoThird(ImageSharp.Size original, ImageSharp.Size target) {
      return new ImageSharp.Size(original.Width*2/3, original.Height*2/3);
    }

    public static ImageSharp.Size ShrinkThreeFourth(ImageSharp.Size original, ImageSharp.Size target) {
      return new ImageSharp.Size(original.Width*3/4, original.Height*3/4);
    }
    public static ImageSharp.Size ShrinkFourFifth(ImageSharp.Size original, ImageSharp.Size target) {
      return new ImageSharp.Size(original.Width*4/5, original.Height*4/5);
    }
    public static ImageSharp.Size ShrinkThreeFifth(ImageSharp.Size original, ImageSharp.Size target) {
      return new ImageSharp.Size(original.Width*3/5, original.Height*3/5);
    }

    public static ImageSharp.Size SizeWidthScreenRatio80(ImageSharp.Size original, ImageSharp.Size target){
      var width = target.Width * 4 / 5;
      var height = width * original.Height / original.Width;
      return new ImageSharp.Size(width, height);
    }

//     public static Resizer GetSizeFixedResizer(Size size) {
//       Size sizeTemp = size;
//       Resizer resizer = delegate(Size original, Size target) {
//         return sizeTemp;
//       };
//     }

    [Obsolete("use Resize(Image, Size, Resizer, IResampler) instead")]
    public static ImageSharp.Image Resize(ImageSharp.Image original, ImageSharp.Size target, Resizer resizer, GDI.Drawing2D.InterpolationMode interpolationMode) {
      IResampler resampler = SelectResampler(interpolationMode);
      return Resize(original, target, resizer, resampler);
    }

    private static IResampler SelectResampler(GDI.Drawing2D.InterpolationMode interpolationMode) {
      IResampler resampler;
      if (resamplerSelector.TryGetValue(interpolationMode, out var selected)) {
        resampler = selected;
      }
      else {
        resampler = KnownResamplers.Bicubic;
      }
      return resampler;
    }

    public static ImageSharp.Image Resize(ImageSharp.Image original, ImageSharp.Size target, Resizer resizer, IResampler resampler) {
      return Resize(original, target, resizer, resampler, false);
    }

    [Obsolete("use Resize(Image, Size, Resizer, IResampler, bool) instead")]
    public static ImageSharp.Image Resize(ImageSharp.Image original, ImageSharp.Size target, Resizer resizer, GDI.Drawing2D.InterpolationMode interpolationMode, bool rotate) {
      IResampler resampler = SelectResampler(interpolationMode);
      return Resize(original, target, resizer, resampler);
    }

    public static ImageSharp.Image Resize(ImageSharp.Image original, ImageSharp.Size target, Resizer resizer, IResampler resampler, bool rotate) {
      if (rotate) {
        ImageSharp.Size changedSize = resizer(new ImageSharp.Size(original.Height, original.Width), target);
        return original.Clone(x => x
          .Rotate(RotateMode.Rotate90)
          .Resize(changedSize.Width, changedSize.Height, resampler));
      }
      else {

        ImageSharp.Size changedSize = resizer(original.Size, target);

        if (changedSize == original.Size) {
          return original;
        }
        else {
          return original.Clone(c => c.Resize(changedSize.Width, changedSize.Height, resampler));
        }
      }
    }
  }
}
