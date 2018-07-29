using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Ame.Infrastructure.Utils
{
    public static class ImageUtils
    {
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
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
            Bitmap bitmap = BitmapImageToBitmap(bitmapImage);
            Image<Bgr, Byte> imageCV = new Image<Bgr, Byte>(bitmap);
            return imageCV.Mat;
        }

        public static ImageDrawing MatToImageDrawing(Mat mat)
        {
            Rect drawingRect = new Rect(new System.Windows.Size(mat.Size.Width, mat.Size.Height));
            return MatToImageDrawing(mat, drawingRect);
        }

        public static ImageDrawing MatToImageDrawing(Mat mat, Rect drawingRect)
        {
            BitmapSource source;
            using (Bitmap bitmap = mat.Bitmap)
            {
                IntPtr ptr = bitmap.GetHbitmap();
                source = Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            ImageDrawing imageDrawing = new ImageDrawing(source, drawingRect);
            return imageDrawing;
        }

        public static DrawingImage MatToDrawingImage(Mat mat)
        {
            ImageDrawing imageDrawing = MatToImageDrawing(mat);
            DrawingImage drawingImage = new DrawingImage(imageDrawing);
            return drawingImage;
        }

        public static DrawingGroup MatToDrawingGroup(Mat mat)
        {
            ImageDrawing imageDrawing = MatToImageDrawing(mat);
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(imageDrawing);
            return drawingGroup;
        }

        public static bool Intersects(Mat mat, System.Windows.Point point)
        {
            if (point.X >= mat.Width || point.X < 0 || point.Y >= mat.Height || point.Y < 0)
            {
                return false;
            }
            return true;
        }

        public static bool Intersects(ImageDrawing drawing, System.Windows.Point point)
        {
            System.Windows.Point topLeft = drawing.Bounds.TopLeft;
            System.Windows.Point bottomRight = drawing.Bounds.BottomRight;
            if (point.X >= bottomRight.X || point.X < topLeft.X || point.Y >= bottomRight.Y || point.Y < topLeft.Y)
            {
                return false;
            }
            return true;
        }

        public static bool Intersects(DrawingImage drawing, System.Windows.Point point)
        {
            if (point.X >= drawing.Width || point.X < 0 || point.Y >= drawing.Height || point.Y < 0)
            {
                return false;
            }
            return true;
        }

        public static bool Intersects(DrawingGroup drawing, System.Windows.Point point)
        {
            System.Windows.Point topLeft = drawing.Bounds.TopLeft;
            System.Windows.Point bottomRight = drawing.Bounds.BottomRight;
            if (point.X >= bottomRight.X || point.X < topLeft.X || point.Y >= bottomRight.Y || point.Y < topLeft.Y)
            {
                return false;
            }
            return true;
        }

        public static System.Windows.Media.Color ColorAt(Mat image, System.Windows.Point pixelPoint)
        {
            byte[] colorsBGR = image.GetData((int)pixelPoint.Y, (int)pixelPoint.X);
            return System.Windows.Media.Color.FromRgb(colorsBGR[2], colorsBGR[1], colorsBGR[0]);
        }

        public static Mat ColorToTransparent(Mat image, System.Windows.Media.Color transparentColor)
        {
            Mat trasparentMask = new Mat();
            IInputArray transparency = new ScalarArray(new MCvScalar(transparentColor.B, transparentColor.G, transparentColor.R, transparentColor.A));
            CvInvoke.InRange(image, transparency, transparency, trasparentMask);
            CvInvoke.BitwiseNot(trasparentMask, trasparentMask);
            image.CopyTo(trasparentMask, trasparentMask);
            return trasparentMask;
        }
    }
}
