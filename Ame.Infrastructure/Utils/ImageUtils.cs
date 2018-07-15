using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Ame.Infrastructure.Utils
{
    public static class ImageUtils
    {
        // TODO look into wrapping mat with these utility functions
        public static Bitmap BitMapImageToBitMap(BitmapImage bitmapImage)
        {
            //return new Bitmap(bitmapImage.StreamSource);
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage MatToBitmapImage(Mat image)
        {
            BitmapImage croppedBitmap = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                image.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                croppedBitmap.BeginInit();
                croppedBitmap.StreamSource = ms;
                croppedBitmap.CacheOption = BitmapCacheOption.OnLoad;
                croppedBitmap.EndInit();
            }
            return croppedBitmap;
        }

        public static Mat BitmapImageToMat(BitmapImage bitmapImage)
        {
            Bitmap bitmap = BitMapImageToBitMap(bitmapImage);
            Image<Bgr, Byte> imageCV = new Image<Bgr, Byte>(bitmap);
            return imageCV.Mat;
        }

        public static DrawingImage MatToDrawingImage(Mat mat)
        {
            Rect drawingRect = new Rect(new System.Windows.Size(mat.Size.Width, mat.Size.Height));
            ImageDrawing imageDrawing = new ImageDrawing(MatToBitmapImage(mat), drawingRect);
            DrawingImage drawingImage = new DrawingImage(imageDrawing);
            return drawingImage;
        }

        public static DrawingGroup MatToDrawingGroup(Mat mat)
        {
            Rect drawingRect = new Rect(new System.Windows.Size(mat.Size.Width, mat.Size.Height));
            ImageDrawing imageDrawing = new ImageDrawing(MatToBitmapImage(mat), drawingRect);
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(imageDrawing);
            return drawingGroup;
        }

        public static DrawingGroup MatToDrawingGroup(Mat mat, Rect drawingRect)
        {
            ImageDrawing imageDrawing = new ImageDrawing(MatToBitmapImage(mat), drawingRect);
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(imageDrawing);
            return drawingGroup;
        }

        public static ImageDrawing MatToImageDrawing(Mat mat)
        {
            Rect drawingRect = new Rect(new System.Windows.Size(mat.Size.Width, mat.Size.Height));

            BitmapSource bs;
            using (Bitmap source = mat.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            ImageDrawing imageDrawing = new ImageDrawing(bs, drawingRect);
            return imageDrawing;
        }

        public static bool Intersects(Mat mat, System.Windows.Point point)
        {
            if (point.X >= mat.Width || point.X < 0 || point.Y >= mat.Height || point.Y < 0)
            {
                return false;
            }
            return true;
        }
    }
}
