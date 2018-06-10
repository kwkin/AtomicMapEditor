using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ame.Infrastructure.Utils
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

        public static Point IntPoint(Point point)
        {
            return new Point((int)point.X, (int)point.Y);
        }

        public static System.Drawing.Point WindowsToDrawingPoint(Point point)
        {
            return new System.Drawing.Point((int)point.X, (int)point.Y);
        }

        public static Point DrawingToWindowPoint(System.Drawing.Point point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
