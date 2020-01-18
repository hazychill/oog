using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Oog {
  public class InterpolationModeConverter : EnumConverter {
    public InterpolationModeConverter() : base(typeof(InterpolationMode)) {}

    public override TypeConverter.StandardValuesCollection GetStandardValues (ITypeDescriptorContext context) {
      return new TypeConverter.StandardValuesCollection(new InterpolationMode[]{
        InterpolationMode.Bicubic,
        InterpolationMode.Bilinear,
        InterpolationMode.Default,
        InterpolationMode.High,
        InterpolationMode.HighQualityBicubic,
        InterpolationMode.HighQualityBilinear,
        InterpolationMode.Low,
        InterpolationMode.NearestNeighbor
      });
    }
  }
}