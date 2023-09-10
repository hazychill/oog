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
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using SixLabors.ImageSharp.Processing;

#endregion

namespace Oog.Viewer {
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class FullScreenViewerSettings {
    Resizer resizer;
    IResampler resampler;
    Color backColor;

    //const string SETTING_FILE_NAME = "OogSettings.xml";

    private FullScreenViewerSettings(Resizer resizer, IResampler resampler, Color backColor) {
      this.resizer = resizer;
      this.resampler = resampler;
      this.backColor = backColor;
    }

    private FullScreenViewerSettings(Resizer resizer, IResampler resampler)
      : this(resizer, resampler, DefaultBackColor) { }

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
      IResampler resampler;
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

      ResamplerConverter resamplerConverter = new ResamplerConverter();

      XmlElement resamplerElem = document.SelectSingleNode("settings/fullScreenViewer/resampler") as XmlElement;
      if (resamplerElem != null) {
        try {
          resampler = resamplerConverter.ConvertFrom(resamplerElem.GetAttribute("value")) as IResampler;
        }
        catch {
          resampler = DefaultResampler;
        }
      }
      else {
        resampler = DefaultResampler;
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

      return new FullScreenViewerSettings(resizer, resampler, backColor);
    }

    public static bool operator ==(FullScreenViewerSettings x, FullScreenViewerSettings y) {
      return (x.Resizer.Method == y.Resizer.Method) &&
        (x.Resampler == y.Resampler) &&
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
      return resampler.GetHashCode() + Resizer.GetHashCode() + backColor.GetHashCode();
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

      ResamplerConverter resamplerConverter = new ResamplerConverter();
      
      writer.WriteStartElement("resampler");
      writer.WriteAttributeString("value", resamplerConverter.ConvertToString(resampler));
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

    [TypeConverter(typeof(ResamplerConverter))]
    [DefaultValue(typeof(IResampler), "Bicubic")]
    [Description("Resizing quality.")]
    public IResampler Resampler {
      get { return resampler; }
      set { resampler = value; }
    }

    [Description("Background color of viewer.")]
    public Color BackColor {
      get { return backColor; }
      set { backColor = value; }
    }

    private static Resizer DefaultResizer {
      get { return new Resizer(ImageResizer.OriginalSize); }
    }
    private static IResampler DefaultResampler {
      get { return KnownResamplers.Bicubic; }
    }
    private static Color DefaultBackColor {
      get { return Color.Black; }
    }
    internal static FullScreenViewerSettings Default {
      get { return new FullScreenViewerSettings(DefaultResizer, DefaultResampler, DefaultBackColor); }
    }

  }
}
