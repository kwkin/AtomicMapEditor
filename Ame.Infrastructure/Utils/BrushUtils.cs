using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;

namespace Ame.Infrastructure.Utils
{
    public static class BrushUtils
    {
        public static Mat CropImage(Mat image, Point topLeftPixel, Size pixelSize)
        {
            System.Drawing.Point topLeftDrawing = GeometryUtils.WindowsToDrawingPoint(topLeftPixel);
            System.Drawing.Size pixelSizeDrawing = GeometryUtils.WindowsToDrawingSize(pixelSize);
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(topLeftDrawing, pixelSizeDrawing);
            return new Mat(image, rectangle);
        }

        public static Mat CropImage(Mat image, Point topLeftPixel, Point bottomRightPixel)
        {
            Size pointSize = (Size)(bottomRightPixel - topLeftPixel);
            System.Drawing.Point topLeftDrawing = GeometryUtils.WindowsToDrawingPoint(topLeftPixel);
            System.Drawing.Size pixelSizeDrawing = GeometryUtils.WindowsToDrawingSize(pointSize);
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(topLeftDrawing, pixelSizeDrawing);
            return new Mat(image, rectangle);
        }
    }
}
