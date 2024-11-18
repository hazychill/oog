using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using ImageSharp = SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Oog.Plugin {
  public class ZipImageCreator : IImageCreator {

    static ZipImageCreator() {
      string fileName = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "oogtrace.log");
      // string name = "ZipImageCreatorTrace";
      // TraceListener listener = new TextWriterTraceListener(fileName, name);
      // Debug.Listeners.Add(listener);
      // Debug.AutoFlush = true;
    }

    public ImageSharp.Image GetImage(Stream data) {
      ZipFile zip = null;
      try {
        if (data.CanSeek == false){
          return GetDefaultImage();
        }

        zip = new ZipFile(data);

        foreach (ZipEntry entry in zip ) {
          if (IsImageEntry(entry)) {
            try {
              using (Stream input = zip.GetInputStream(entry)) {
                return ImageSharp.Image.Load(input);
              }
            }
            catch (ZipException e) {
              Debug.WriteLine(e);
              continue;
            }
            catch (ArgumentException e) {
              Debug.WriteLine(e);
              continue;
            }
          }
        }
        return GetDefaultImage();
      }
      catch (IOException e) {
        Debug.WriteLine(e);
        return GetDefaultImage();
      }
      catch (ZipException e) {
        Debug.WriteLine(e);
        return GetDefaultImage();
      }
      finally {
        Debug.WriteLine("finally");
        if (zip != null) {
          zip.Close();
        }
      }
    }

    private ImageSharp.Image GetDefaultImage() {
      var image = new ImageSharp.Image<ImageSharp.PixelFormats.Argb32>(50, 50);
      image.Mutate(c => c.BackgroundColor(ImageSharp.Color.Silver));
      return image;
    }

    static Regex imageExtRegex = new Regex("\\.(?i:jpe?g|gif|png|bmp)$");

    private bool IsImageEntry(ZipEntry entry) {
      return imageExtRegex.IsMatch(entry.Name);
    }

    public string[] SupportedExtensions {
      get { return new string[]{ ".zip" }; }
    }
  }
}