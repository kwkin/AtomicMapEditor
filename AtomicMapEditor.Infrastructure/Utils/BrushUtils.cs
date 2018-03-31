using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;

namespace AtomicMapEditor.Infrastructure.Utils
{
    public static class BrushUtils
    {
        public static Mat CropImage(Mat image, Point topLeftPixel, Size pixelSize)
        {
            System.Drawing.Point topLeftDrawing = PointUtils.WindowsToDrawingPoint(topLeftPixel);
            System.Drawing.Size pixelSizeDrawing = SizeUtils.WindowsToDrawingPoint(pixelSize);
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(topLeftDrawing, pixelSizeDrawing);
            return new Mat(image, rectangle);
        }
    }
}
