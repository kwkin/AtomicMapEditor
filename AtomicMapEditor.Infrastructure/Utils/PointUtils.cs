using System;
using System.Windows;

namespace AtomicMapEditor.Infrastructure.Utils
{
    public static class PointUtils
    {
        public static Point TopLeft(Point point1, Point point2)
        {
            Point topLeft = new Point();
            topLeft.X = Math.Min(point1.X, point2.X);
            topLeft.Y = Math.Min(point1.Y, point2.Y);
            return topLeft;
        }

        public static Size SelectionSize(Point point1, Point point2)
        {
            Size size = (Size)Point.Subtract(point1, point2);
            size.Width = Math.Abs(size.Width) + 1;
            size.Height = Math.Abs(size.Height) + 1;
            return size;
        }
    }
}
