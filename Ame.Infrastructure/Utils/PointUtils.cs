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
        /// <summary>
        /// Returns the top left point constructed from the given two points. Assumes that 0,0 is located in the top left.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point TopLeft(Point point1, Point point2)
        {
            Point topLeft = new Point();
            topLeft.X = Math.Min(point1.X, point2.X);
            topLeft.Y = Math.Min(point1.Y, point2.Y);
            return topLeft;
        }

        /// <summary>
        /// Returns the top right constructed from the given two points. Assumes that 0,0 is located in the top left.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point TopRight(Point point1, Point point2)
        {
            Point topRight = new Point();
            topRight.X = Math.Max(point1.X, point2.X);
            topRight.Y = Math.Min(point1.Y, point2.Y);
            return topRight;
        }

        /// <summary>
        /// Returns the bottom left constructed from the given two points. Assumes that 0,0 is located in the top left.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point BottomLeft(Point point1, Point point2)
        {
            Point bottomLeft = new Point();
            bottomLeft.X = Math.Min(point1.X, point2.X);
            bottomLeft.Y = Math.Max(point1.Y, point2.Y);
            return bottomLeft;
        }

        /// <summary>
        /// Returns the bottom right constructed from the given two points. Assumes that 0,0 is located in the top left.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point BottomRight(Point point1, Point point2)
        {
            Point bottomRight = new Point();
            bottomRight.X = Math.Max(point1.X, point2.X);
            bottomRight.Y = Math.Max(point1.Y, point2.Y);
            return bottomRight;
        }

        public static Size ComputeSize(Point point1, Point point2)
        {
            Size size = (Size)Point.Subtract(point1, point2);
            size.Width = Math.Abs(size.Width) + 1;
            size.Height = Math.Abs(size.Height) + 1;
            return size;
        }

        public static Point CreateIntPoint(Point point)
        {
            return new Point((int)point.X, (int)point.Y);
        }

        public static Point CreateIntPoint(double x, double y)
        {
            return new Point((int)x, (int)y);
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
