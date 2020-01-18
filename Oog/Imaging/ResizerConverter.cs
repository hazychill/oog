using System;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;

namespace Oog {
  public class ResizerConverter : TypeConverter {
    public override bool GetStandardValuesSupported (ITypeDescriptorContext context) { return true; }

    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) {
      if (sourceType == typeof(string)) return true;
      return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType) {
      if (destinationType == typeof(string)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override Object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, Object value) {
      string name = value as string;
      if (name == null) throw new NotSupportedException();

      Type t = typeof(ImageResizer);
      MethodInfo method = t.GetMethod(name, new Type[]{ typeof(Size), typeof(Size) });
      if (method == null) throw new NotSupportedException();
      if (method.ReturnType != typeof(Size)) throw new NotSupportedException();

      return Delegate.CreateDelegate(typeof(Resizer), method);
    }

    public override Object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType) {
      if (destinationType != typeof(string)) throw new NotSupportedException();

      Resizer resizer = value as Resizer;
      if (resizer == null) throw new NotSupportedException();
      return resizer.Method.Name;
    }
    
    public override bool IsValid (ITypeDescriptorContext context, Object value) {
      string name = value as string;
      if (name == null) return false;

      Type t = typeof(ImageResizer);
      MethodInfo method = t.GetMethod(name, new Type[]{ typeof(Size), typeof(Size) });
      if (method == null) return false;
      if (method.ReturnType != typeof(Size)) return false;

      return true;
    }

    public override StandardValuesCollection GetStandardValues (ITypeDescriptorContext context) {
      List<Resizer> resizers = new List<Resizer>();
      Type t = typeof(ImageResizer);
      MethodInfo[] methods = t.GetMethods();
      Array.Sort<MethodInfo>(methods, delegate(MethodInfo x, MethodInfo y) {
        return x.Name.CompareTo(y.Name);
      });
      foreach (MethodInfo method in methods) {
        if (method.ReturnType != typeof(Size)) continue;

        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length != 2) continue;
        if (parameters[0].ParameterType != typeof(Size)) continue;
        if (parameters[1].ParameterType != typeof(Size)) continue;

        resizers.Add(Delegate.CreateDelegate(typeof(Resizer), method) as Resizer);
      }

      return new StandardValuesCollection(resizers);
    }

    public override bool GetStandardValuesExclusive (ITypeDescriptorContext context) {
      return true;
    }

  }
}