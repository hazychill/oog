using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Threading;

namespace Oog {
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class ThumbnailSettings {
    private const string KEY_SIZE = "thumbnailSize";
    private const string KEY_INTERPOLATION_MODE = "thumbnailInterpolationMode";
    private const string KEY_BACK_COLOR = "thumbnailBackColor";
    private const string KEY_THREAD_PRIORITY = "thumbnailThreadPrioirty";

    private Size size;
    private InterpolationMode interpolationMode;
    private Color backColor;
    private ThreadPriority threadPriority;
    
    [Description("Size of thumbnail.")]
    public Size Size {
      get { return size; }
      set { size = value; }
    }

    [TypeConverter(typeof(InterpolationModeConverter))]
    [DefaultValue(InterpolationMode.High)]
    [Description("Resizing quality.")]
    public InterpolationMode InterpolationMode {
      get { return interpolationMode; }
      set { interpolationMode = value; }
    }

    [Description("Background color of thumbnail.")]
    public Color BackColor {
      get { return backColor; }
      set { backColor = value; }
    }

    [DefaultValue(ThreadPriority.BelowNormal)]
    [Description("Priority of the thread that creates thumbnail images in the background.")]
    public ThreadPriority ThumbnailThreadPriority {
      get { return threadPriority; }
      set { threadPriority = value; }
    }


    private ThumbnailSettings(Size size, InterpolationMode interpolationMode, Color backColor, ThreadPriority threadPriority) {
      this.size = size;
      this.interpolationMode = interpolationMode;
      this.backColor = backColor;
      this.threadPriority = threadPriority;
    }

    //const string SETTING_FILE_NAME = "OogSettings.xml";

    [Obsolete("Use Load(XmlDocument document) instead.")]
    public static ThumbnailSettings Load() {
      ThumbnailSettings settings = Default;

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

    public static ThumbnailSettings Load(XmlDocument document) {
      Size size;
      InterpolationMode interpolationMode;
      Color backColor;
      ThreadPriority threadPriority;


      SizeConverter sizeConv = new SizeConverter();
      XmlElement sizeElem = document.SelectSingleNode("settings/thumbnail/size") as XmlElement;
      if (sizeElem != null) {
        try {
          size = (Size)sizeConv.ConvertFromString(sizeElem.GetAttribute("value"));
        }
        catch {
          size = DefaultSize;
        }
      }
      else {
        size = DefaultSize;
      }

      EnumConverter ipConv = new EnumConverter(typeof(InterpolationMode));
      XmlElement ipmodeElem = document.SelectSingleNode("settings/thumbnail/interpolationMode") as XmlElement;
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
      XmlElement bcolorElem = document.SelectSingleNode("settings/thumbnail/backColor") as XmlElement;
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

      EnumConverter priorityConv = new EnumConverter(typeof(ThreadPriority));
      XmlElement priorityElem = document.SelectSingleNode("settings/thumbnail/threadPriority") as XmlElement;
      if (priorityElem != null) {
        try {
          threadPriority = (ThreadPriority)priorityConv.ConvertFromString(priorityElem.GetAttribute("value"));
        }
        catch {
          threadPriority = DefaultThumbnailThreadPriority;
        }
      }
      else {
        threadPriority = DefaultThumbnailThreadPriority;
      }

      return new ThumbnailSettings(size, interpolationMode, backColor, threadPriority);
    }

    public void Save() {
      OogSettings settings = OogSettings.Load();
      settings.ThumbnailSettings = this;
      settings.Save();
    }

    internal void Save(XmlWriter writer) {
      writer.WriteStartElement("thumbnail");

      SizeConverter sizeConv = new SizeConverter();

      writer.WriteStartElement("size");
      writer.WriteAttributeString("value", sizeConv.ConvertToString(size));
      writer.WriteEndElement();

      EnumConverter ipConv = new EnumConverter(typeof(InterpolationMode));

      writer.WriteStartElement("interpolationMode");
      writer.WriteAttributeString("value", ipConv.ConvertToString(interpolationMode));
      writer.WriteEndElement();

      ColorConverter colorConv = new ColorConverter();

      writer.WriteStartElement("backColor");
      writer.WriteAttributeString("value", colorConv.ConvertToString(backColor));
      writer.WriteEndElement();

      EnumConverter priorityConv = new EnumConverter(typeof(ThreadPriority));

      writer.WriteStartElement("threadPriority");
      writer.WriteAttributeString("value", priorityConv.ConvertToString(threadPriority));
      writer.WriteEndElement();

      writer.WriteEndElement();
    }

    public override bool Equals(object obj) {
      if (obj==null || this.GetType()!=obj.GetType()) return false;

      ThumbnailSettings ts = obj as ThumbnailSettings;
      if (ts == null) return false;

      if (base.Equals(obj)) return true;

      return (this.Size==ts.Size) &&
        (this.InterpolationMode == ts.InterpolationMode) &&
        (this.BackColor.ToArgb() == ts.BackColor.ToArgb()) &&
        (this.ThumbnailThreadPriority == ts.ThumbnailThreadPriority);
    }

    public static bool operator ==(ThumbnailSettings x, ThumbnailSettings y) {
      return x.Equals(y);
    }
    public static bool operator !=(ThumbnailSettings x, ThumbnailSettings y) {
      return !x.Equals(y);
    }

    public override int GetHashCode() {
      return size.GetHashCode() + interpolationMode.GetHashCode();
    }

    private static Size DefaultSize {
      get { return new Size(150, 200); }
    }

    private static  InterpolationMode DefaultInterpolationMode {
      get { return InterpolationMode.High; }
    }
    private static Color DefaultBackColor {
      get { return SystemColors.Window; }
    }
    private static ThreadPriority DefaultThumbnailThreadPriority {
      get { return ThreadPriority.BelowNormal; }
    }
    internal static ThumbnailSettings Default {
      get { return new ThumbnailSettings(DefaultSize, DefaultInterpolationMode, DefaultBackColor, DefaultThumbnailThreadPriority); }
    }
  }
}
