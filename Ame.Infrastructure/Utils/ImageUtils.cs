using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Emgu.CV;

namespace Ame.Infrastructure.Utils
{
    public static class ImageUtils
    {
        public static BitmapImage MatToBitmap(Mat image)
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
    }
}
