using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using Oog.Plugin;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using ImageSharp = SixLabors.ImageSharp;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.ComponentModel;

namespace Oog.Viewer {
  partial class FullScreenViewer {
    IExtractor extractor;
    Dictionary<string, IImageCreator> imageCreators;

    string[] imageNames;
    FullScreenViewerSettings settings;
    //InterpolationMode interpolationMode;
    //Resizer resizer;
    int currentIndex;
    Image lookAheadImage;

    //次のイメージに進めるときに実行される
    MethodInvoker nextImage;
    MethodInvoker prevImage;
    //先読みが完了したときに実行される
    MethodInvoker lookAheadComplete;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public ManualResetEventSlim ThumbnailWorkerBlocker { get; set; }

    private void SetImage() {
      Image image = GetImage(currentIndex);
      picture.Image = image;
      SetPictureLocation();

      if (currentIndex != imageNames.Length-1) {
        //先読み中に次の画像に進める要求があった場合は
        //先読み完了後すぐに画像を切り替える
        nextImage = BookLookAhead;
      }
      if (currentIndex != 0) {
        prevImage = SetPrevImage;
      }

      //先読みが終了したことを表す
      lookAheadComplete = ReadyForChange;

      LookAhead();
    }

    //次の画像を非同期で先読みする
    private void LookAhead() {
      MethodInvoker lookAhead = LookAheadCore;
      if (ThumbnailWorkerBlocker != null) {
        ThumbnailWorkerBlocker.Reset();
      }

      System.Threading.Tasks.Task.Factory.StartNew(LookAheadCore);
    }

    //先読みの実装
    private void LookAheadCore() {
      if (currentIndex != imageNames.Length-1) {
        lookAheadImage = GetImage(currentIndex+1);
        lookAheadComplete();
      }
      else {
        lookAheadImage = null;
      }

      prevImage = SetPrevImage;
    }


    //先読み後すぐに画像を切り替えるようにする
    private void BookLookAhead() {
      lookAheadComplete = SetLookAheadAsync;
      prevImage = DoNothing;
    }

    //先読みした画像に切り替え，現在の画像をDisposeする
    private void SetLookAhead() {
      Image temp = picture.Image;
      picture.Image = lookAheadImage;
      SetPictureLocation();
      temp.Dispose();
      currentIndex++;
      lookAheadComplete = ReadyForChange;
      if (currentIndex != imageNames.Length-1) {
        nextImage = BookLookAhead;
      }
      if (currentIndex != 0) {
        //prevImage = SetPrevImage;
        prevImage = ForcePrevImage;
      }
      LookAhead();
    }


    //別スレッドのlookAheadCompleteからSetLookAheadを呼ぶ
    private void SetLookAheadAsync() {
      MethodInvoker method = SetLookAhead;
      this.Invoke(method);
    }


    //画像を進める要求があった場合にすでに読み込み終わっている
    //先読み画像をすぐに渡す
    private void ReadyForChange() {
      nextImage = SetLookAhead;
    }

    private void SetPrevImage() {
      currentIndex--;
      Image temp = lookAheadImage;
      lookAheadImage = picture.Image;
      picture.Image = GetImage(currentIndex);
      SetPictureLocation();
      if (temp != null) temp.Dispose();
      nextImage = SetLookAhead;
    }

    private void ForcePrevImage() {
      //currentIndex--;
      //SetImage();
    }

    //画像の位置を設定する
    private void SetPictureLocation() {
      SetPictureLocation(PictureLocation.RightTop);
    }

    //画像の位置を設定する
    private void SetPictureLocation(PictureLocation location) {
      Size screen = Screen.PrimaryScreen.Bounds.Size;
      int x = 0, y = 0;
      if (picture.Width >= screen.Width) {

        switch (location) {
        case PictureLocation.LeftTop:
        case PictureLocation.LeftBottom:
          x = 0;
          break;
        case PictureLocation.RightTop:
        case PictureLocation.RightBottom:
          x = screen.Width - picture.Width;
          break;
        }

        if (picture.Height >= screen.Height) {
          //縦横共にスクリーンより大きい

          switch (location) {
          case PictureLocation.LeftTop:
          case PictureLocation.RightTop:
            y = 0;
            break;
          case PictureLocation.LeftBottom:
          case PictureLocation.RightBottom:
            y = screen.Height-picture.Height;
            break;
          }

          scrollPicture = ScrollBoth;
        }
        else {
          //横幅のみスクリーンより大きい

          y = (screen.Height - picture.Height) / 2;

          scrollPicture = ScrollHorizontal;
        }
      }
      else {
        x = (screen.Width - picture.Width) / 2;

        if (picture.Height >= screen.Height) {
          //縦幅のみスクリーンより大きい

          switch (location) {
          case PictureLocation.LeftTop:
          case PictureLocation.RightTop:
            y = 0;
            break;
          case PictureLocation.LeftBottom:
          case PictureLocation.RightBottom:
            y = screen.Height-picture.Height;
            break;
          }

          scrollPicture = ScrollVirtical;
        }
        else {
          //縦横共にスクリーンより小さい

          y = (screen.Height - picture.Height) / 2;

          scrollPicture = DoNothing;
        }
      }

      picture.Location = new Point(x, y);
    }

    enum PictureLocation {
      LeftTop,
      LeftBottom,
      RightTop,
      RightBottom
    }

    Dictionary<int, ImageSharp.Size> originalSizes = new Dictionary<int, ImageSharp.Size>();

    private Image GetImage(int index) {
      try {
        string name = imageNames[index];
        using (Stream data = extractor.GetData(name)) {

          if (data == null) return CreateErrorImage();

          string ext = Path.GetExtension(name).ToLower();
          IImageCreator creator = OogUtil.GetImageCreatorForName(imageCreators, name);

          using (var original = creator.GetImage(data)) {
            if (original == null) return CreateErrorImage();

            originalSizes[index] = original.Size;

            var resized = ImageResizer.Resize(original, OogUtil.ImageSharpSize(Screen.PrimaryScreen.Bounds.Size), settings.Resizer, settings.Resampler, menuRotate.Checked);
            var actuallyResized = resized == original;
            try {
              if (this.Height < resized.Height) {
                resized.Mutate(c => c.DrawLine(ImageSharp.Color.Red, 2f, [
                  new ImageSharp.PointF(0, resized.Height-1),
                  new ImageSharp.PointF(resized.Width, resized.Height-1),
                ]));
              }
              return OogUtil.GetImageFromSharpImage(resized);
            }
            finally {
              if (actuallyResized) {
                resized.Dispose();
              }
            }
          }
        }
      }
      finally {
        if (ThumbnailWorkerBlocker != null) {
          ThumbnailWorkerBlocker.Set();
        }
      }
    }

    public Image CreateErrorImage() {
      return new Bitmap(errorImage);
    }

    private void NextImage() {
      if (currentIndex != imageNames.Length-1) {
        nextImage();
      }
    }
    private void PrevImage() {
      if (currentIndex != 0) {
        prevImage();
      }
    }

    private void DoNothing() { }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public IResampler Resampler {
      get { return settings.Resampler; }
      set { settings.Resampler = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Resizer Resizer {
      get { return settings.Resizer; }
      set { settings.Resizer = value; }
    }

    public int CurrentIndex {
      get { return currentIndex; }
    }
  }


}
