#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

#endregion

namespace Oog.Viewer {
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class FullScreenViewerSettings {
    Resizer resizer;
    InterpolationMode interpolationMode;
    Color backColor;

    //const string SETTING_FILE_NAME = "OogSettings.xml";

    private FullScreenViewerSettings(Resizer resizer, InterpolationMode interpolationMode, Color backColor) {
      this.resizer = resizer;
      this.interpolationMode = interpolationMode;
      this.backColor = backColor;
    }

    private FullScreenViewerSettings(Resizer resizer, InterpolationMode interpolationMode)
      : this(resizer, interpolationMode, DefaultBackColor) { }

    [Obsolete("Use Load(XmlDocument document) instead.")]
    public static FullScreenViewerSettings Load() {
      FullScreenViewerSettings settings = Default;

      string settingsFilePath = OogSettings.GetSettingFilePath();
      if (File.Exists(settingsFilePath)) {
        XmlDocument document = new XmlDocument();
        try {
          document.Load(settingsFilePath);
          settings = Load(document);
        }
        catch { }
      }

      return settings;
    }

    public static FullScreenViewerSettings Load(XmlDocument document) {
      Resizer resizer;
      InterpolationMode interpolationMode;
      Color backColor;

      ResizerConverter rsConv = new ResizerConverter();

      XmlElement resizerElem = document.SelectSingleNode("settings/fullScreenViewer/resizer") as XmlElement;
      if (resizerElem != null) {
        try {
          resizer = (Resizer)rsConv.ConvertFrom(null, null, resizerElem.GetAttribute("value"));
        }
        catch {
          resizer = DefaultResizer;
        }
      }
      else {
        resizer = DefaultResizer;
      }

      EnumConverter ipConv = new EnumConverter(typeof(InterpolationMode));

      XmlElement ipmodeElem = document.SelectSingleNode("settings/fullScreenViewer/interpolationMode") as XmlElement;
      if (ipmodeElem != null) {
        try {
          interpolationMode = (InterpolationMode)ipConv.ConvertFromString(ipmodeElem.GetAttribute("value"));
        }
        catch {
          interpolationMode = DefaultInterpolationMode;
        }
      }
      else {
        interpolationMode = DefaultInterpolationMode;
      }

      ColorConverter colorConv = new ColorConverter();

      XmlElement bcolorElem = document.SelectSingleNode("settings/fullScreenViewer/backColor") as XmlElement;
      if (bcolorElem != null) {
        try {
          backColor = (Color)colorConv.ConvertFromString(bcolorElem.GetAttribute("value"));
        }
        catch {
          backColor = DefaultBackColor;
        }
      }
      else {
        backColor = DefaultBackColor;
      }

      return new FullScreenViewerSettings(resizer, interpolationMode, backColor);
    }

    public static bool operator ==(FullScreenViewerSettings x, FullScreenViewerSettings y) {
      return (x.Resizer.Method == y.Resizer.Method) &&
        (x.InterpolationMode == y.InterpolationMode) &&
          (x.BackColor.ToArgb() == y.BackColor.ToArgb());
    }
    public static bool operator !=(FullScreenViewerSettings x, FullScreenViewerSettings y) {
      return !(x == y);
    }
    public override bool Equals(object o) {
      FullScreenViewerSettings settings = o as FullScreenViewerSettings;
      if (settings != null) {
        return (this == settings);
      }
      else {
        return false;
      }
    }
    public override int GetHashCode() {
      return interpolationMode.GetHashCode() + Resizer.GetHashCode() + backColor.GetHashCode();
    }

    public void Save() {
      OogSettings settings = OogSettings.Load();
      settings.FullScreenViewerSettings = this;
      settings.Save();
    }

    internal void Save(XmlWriter writer) {
      writer.WriteStartElement("fullScreenViewer");

      ResizerConverter rsConv = new ResizerConverter();
      
      writer.WriteStartElement("resizer");
      writer.WriteAttributeString("value", (string)rsConv.ConvertTo(null, null, resizer, typeof(string)));
      writer.WriteEndElement();

      EnumConverter ipConv = new EnumConverter(typeof(InterpolationMode));
      
      writer.WriteStartElement("interpolationMode");
      writer.WriteAttributeString("value", ipConv.ConvertToString(interpolationMode));
      writer.WriteEndElement();

      ColorConverter colorConv = new ColorConverter();
      
      writer.WriteStartElement("backColor");
      writer.WriteAttributeString("value", colorConv.ConvertToString(backColor));
      writer.WriteEndElement();
      
      writer.WriteEndElement();
    }
    
    [TypeConverter(typeof(ResizerConverter))]
    [Description("Resizing method used to adjust image to screen.")]
    public Resizer Resizer {
      get { return resizer; }
      set { resizer = value; }
    }

    [TypeConverter(typeof(InterpolationModeConverter))]
    [DefaultValue(InterpolationMode.High)]
    [Description("Resizing quality.")]
    public InterpolationMode InterpolationMode {
      get { return interpolationMode; }
      set { interpolationMode = value; }
    }

    [Description("Background color of viewer.")]
    public Color BackColor {
      get { return backColor; }
      set { backColor = value; }
    }

    private static Resizer DefaultResizer {
      get { return new Resizer(ImageResizer.OriginalSize); }
    }
    private static InterpolationMode DefaultInterpolationMode {
      get { return InterpolationMode.High; }
    }
    private static Color DefaultBackColor {
      get { return Color.Black; }
    }
    internal static FullScreenViewerSettings Default {
      get { return new FullScreenViewerSettings(DefaultResizer, DefaultInterpolationMode, DefaultBackColor); }
    }

  }
}
