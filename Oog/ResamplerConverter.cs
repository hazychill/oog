using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Oog {
  public class ResamplerConverter : TypeConverter {
    static Dictionary<String, IResampler> convertFromMap;
    static Dictionary<IResampler, String> convertToMap;

    static ResamplerConverter() {
      convertFromMap = new Dictionary<string, IResampler>() {
        { "Bicubic", KnownResamplers.Bicubic },
        { "Box", KnownResamplers.Box },
        { "CatmullRom", KnownResamplers.CatmullRom },
        { "Hermite", KnownResamplers.Hermite },
        { "Lanczos2", KnownResamplers.Lanczos2 },
        { "Lanczos3", KnownResamplers.Lanczos3 },
        { "Lanczos5", KnownResamplers.Lanczos5 },
        { "Lanczos8", KnownResamplers.Lanczos8 },
        { "MitchellNetravali", KnownResamplers.MitchellNetravali },
        { "NearestNeighbor", KnownResamplers.NearestNeighbor },
        { "Robidoux", KnownResamplers.Robidoux },
        { "RobidouxSharp", KnownResamplers.RobidouxSharp },
        { "Spline", KnownResamplers.Spline },
        { "Triangle", KnownResamplers.Triangle },
        { "Welch", KnownResamplers.Welch },
      };

      convertToMap = new Dictionary<IResampler, string>() {
        { KnownResamplers.Bicubic, "Bicubic" },
        { KnownResamplers.Box, "Box" },
        { KnownResamplers.CatmullRom, "CatmullRom" },
        { KnownResamplers.Hermite, "Hermite" },
        { KnownResamplers.Lanczos2, "Lanczos2" },
        { KnownResamplers.Lanczos3, "Lanczos3" },
        { KnownResamplers.Lanczos5, "Lanczos5" },
        { KnownResamplers.Lanczos8, "Lanczos8" },
        { KnownResamplers.MitchellNetravali, "MitchellNetravali" },
        { KnownResamplers.NearestNeighbor, "NearestNeighbor" },
        { KnownResamplers.Robidoux, "Robidoux" },
        { KnownResamplers.RobidouxSharp, "RobidouxSharp" },
        { KnownResamplers.Spline, "Spline" },
        { KnownResamplers.Triangle, "Triangle" },
        { KnownResamplers.Welch, "Welch" },
      };
    }

    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) {
      if (sourceType == typeof(string)) {
        return true;
      }
      else {
        return false;
      }
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType) {
      if (destinationType == typeof(string)) {
        return true;
      }
      else {
        return false;
      }
    }

    public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
      if (value == null) {
        throw new ArgumentNullException(nameof(value));
      }
      String resamplerName;
      if (CanConvertFrom(null, value.GetType())) {
        resamplerName = value as string;
      }
      else {
        throw new ArgumentException($"cannot convert from {value.GetType().Name}", nameof(value));
      }

      if (convertFromMap.TryGetValue(resamplerName, out IResampler resampler)) {
        return resampler;
      }
      else {
        throw new ArgumentException($"cannot convert value of {resamplerName}", nameof(value));
      }
    }

    public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
      if (value == null) {
        throw new ArgumentNullException(nameof(value));
      }
      if (destinationType == null) {
        throw new ArgumentNullException(nameof(destinationType));
      }
      IResampler resampler;
      if (value is IResampler) {
        resampler = value as IResampler;
      }
      else {
        throw new ArgumentException($"value must be type of IResampler", nameof(value));
      }
      if (!CanConvertTo(null, destinationType)) {
        throw new ArgumentException($"cannot convert to {destinationType.Name}", nameof(destinationType));
      }

      if (convertToMap.TryGetValue(resampler, out string resamplerName)) {
        return resamplerName;
      }
      else {
        throw new ArgumentException($"unknown resampler {value.GetType().Name}", nameof(value));
      }
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
      return true;
    }

    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
      return new StandardValuesCollection(new IResampler[] {
        KnownResamplers.Bicubic,
        KnownResamplers.Box,
        KnownResamplers.CatmullRom,
        KnownResamplers.Hermite,
        KnownResamplers.Lanczos2,
        KnownResamplers.Lanczos3,
        KnownResamplers.Lanczos5,
        KnownResamplers.Lanczos8,
        KnownResamplers.MitchellNetravali,
        KnownResamplers.NearestNeighbor,
        KnownResamplers.Robidoux,
        KnownResamplers.RobidouxSharp,
        KnownResamplers.Spline,
        KnownResamplers.Triangle,
        KnownResamplers.Welch,
      });
    }
  }
}