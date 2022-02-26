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

    //���̃C���[�W�ɐi�߂�Ƃ��Ɏ��s�����
    MethodInvoker nextImage;
    MethodInvoker prevImage;
    //��ǂ݂����������Ƃ��Ɏ��s�����
    MethodInvoker lookAheadComplete;

    public ManualResetEventSlim ThumbnailWorkerBlocker { get; set; }

    private void SetImage() {
      Image image = GetImage(currentIndex);
      picture.Image = image;
      SetPictureLocation();

      if (currentIndex != imageNames.Length-1) {
        //��ǂݒ��Ɏ��̉摜�ɐi�߂�v�����������ꍇ��
        //��ǂ݊����シ���ɉ摜��؂�ւ���
        nextImage = BookLookAhead;
      }
      if (currentIndex != 0) {
        prevImage = SetPrevImage;
      }

      //��ǂ݂��I���������Ƃ�\��
      lookAheadComplete = ReadyForChange;

      LookAhead();
    }

    //���̉摜��񓯊��Ő�ǂ݂���
    private void LookAhead() {
      MethodInvoker lookAhead = LookAheadCore;
      if (ThumbnailWorkerBlocker != null) {
        ThumbnailWorkerBlocker.Reset();
      }

      System.Threading.Tasks.Task.Factory.StartNew(LookAheadCore);
    }

    //��ǂ݂̎���
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


    //��ǂ݌シ���ɉ摜��؂�ւ���悤�ɂ���
    private void BookLookAhead() {
      lookAheadComplete = SetLookAheadAsync;
      prevImage = DoNothing;
    }

    //��ǂ݂����摜�ɐ؂�ւ��C���݂̉摜��Dispose����
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


    //�ʃX���b�h��lookAheadComplete����SetLookAhead���Ă�
    private void SetLookAheadAsync() {
      MethodInvoker method = SetLookAhead;
      this.Invoke(method);
    }


    //�摜��i�߂�v�����������ꍇ�ɂ��łɓǂݍ��ݏI����Ă���
    //��ǂ݉摜�������ɓn��
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

    //�摜�̈ʒu��ݒ肷��
    private void SetPictureLocation() {
      SetPictureLocation(PictureLocation.RightTop);
    }

    //�摜�̈ʒu��ݒ肷��
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
          //�c�����ɃX�N���[�����傫��

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
          //�����̂݃X�N���[�����傫��

          y = (screen.Height - picture.Height) / 2;

          scrollPicture = ScrollHorizontal;
        }
      }
      else {
        x = (screen.Width - picture.Width) / 2;

        if (picture.Height >= screen.Height) {
          //�c���̂݃X�N���[�����傫��

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
          //�c�����ɃX�N���[����菬����

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

    Dictionary<int, Size> originalSizes = new Dictionary<int, Size>();

    private Image GetImage(int index) {
      try {
        string name = imageNames[index];
        using (Stream data = extractor.GetData(name)) {

          if (data == null) return CreateErrorImage();

          string ext = Path.GetExtension(name).ToLower();
          IImageCreator creator = OogUtil.GetImageCreatorForName(imageCreators, name);

          using (Image original = creator.GetImage(data)) {
            foreach (var propid in original.PropertyIdList) {
              original.RemovePropertyItem(propid);
            }
            if (original == null) return CreateErrorImage();
          
            originalSizes[index] = original.Size;

            Image resized = ImageResizer.Resize(original, Screen.PrimaryScreen.Bounds.Size, settings.Resizer, settings.InterpolationMode, menuRotate.Checked);
            if (resized == original) resized = new Bitmap(original);
            if (this.Height < resized.Height) {
              using (Graphics g  = Graphics.FromImage(resized)) {
                using (Pen pen = new Pen(Color.Red, 2)) {
                  g.DrawLine(pen, 0, resized.Height-1, resized.Width, resized.Height-1);
                }
              }
            }
            return resized;
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

    public InterpolationMode InterpolationMode {
      get { return settings.InterpolationMode; }
      set { settings.InterpolationMode = value; }
    }

    public Resizer Resizer {
      get { return settings.Resizer; }
      set { settings.Resizer = value; }
    }

    public int CurrentIndex {
      get { return currentIndex; }
    }
  }


}
