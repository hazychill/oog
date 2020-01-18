using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace Oog.Plugin {
  public class ZipImageCreator : IImageCreator {

    static ZipImageCreator() {
      string fileName = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "oogtrace.log");
      // string name = "ZipImageCreatorTrace";
      // TraceListener listener = new TextWriterTraceListener(fileName, name);
      // Debug.Listeners.Add(listener);
      // Debug.AutoFlush = true;
    }

    public Image GetImage(Stream data) {
      ZipFile zip = null;
      try {
        if (data.CanSeek == false){
          return GetDefaultImage();
        }

        zip = new ZipFile(data);

        foreach (ZipEntry entry in zip ) {
          if (IsImageEntry(entry)) {
            try {
              using (Stream input = zip.GetInputStream(entry))
              using (Image original = Image.FromStream(input)) {
                return new Bitmap(original);
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

    private Image GetDefaultImage() {
      Image img = new Bitmap(50, 50);
      using (Graphics g = Graphics.FromImage(img)) {
        g.Clear(Color.Silver);
      }
      return img;
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