using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ame.Infrastructure.Utils
{
    public static class GeometryUtils
    {
        #region line

        public static Line CreateLine(Point point1, Point point2)
        {
            Line newLine = new Line();
            newLine.X1 = point1.X;
            newLine.Y1 = point1.Y;
            newLine.X2 = point2.X;
            newLine.Y2 = point2.Y;
            return newLine;
        }

        public static Line CreateLine(double point1X, double point1Y, double point2X, double point2Y)
        {
            Line newLine = new Line();
            newLine.X1 = point1X;
            newLine.Y1 = point1Y;
            newLine.X2 = point2X;
            newLine.Y2 = point2Y;
            return newLine;
        }

        #endregion


        #region point

        /// <summary>
        /// Returns the top left point constructed from the given two points. Assumes that 0,0 is located in the top left.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point TopLeftPoint(Point point1, Point point2)
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
        public static Point TopRightPoint(Point point1, Point point2)
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
        public static Point BottomLeftPoint(Point point1, Point point2)
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
        public static Point BottomRightPoint(Point point1, Point point2)
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

        #endregion


        #region size

        public static System.Drawing.Size WindowsToDrawingPoint(Size size)
        {
            return new System.Drawing.Size((int)size.Width, (int)size.Height);
        }

        public static Size DrawingToWindowPoint(System.Drawing.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        #endregion


        #region transform


        public static Point Transform(GeneralTransform transform, Point point)
        {
            return transform.Transform(point);
        }

        public static Size Transform(GeneralTransform transform, Size size)
        {
            return (Size)transform.Transform((Point)size);
        }

        public static Line Transform(GeneralTransform transform, Line line)
        {
            Point point1 = new Point(line.X1, line.Y1);
            Point point2 = new Point(line.X2, line.Y2);
            point1 = transform.Transform(point1);
            point2 = transform.Transform(point2);
            Line newLine = GeometryUtils.CreateLine(point1, point2);
            return newLine;
        }

        public static Point TransformInt(GeneralTransform transform, Point point)
        {
            Point newPoint = transform.Transform(point);
            return new Point((int)newPoint.X, (int)newPoint.Y);
        }

        public static Size TransformInt(GeneralTransform transform, Size size)
        {
            Point newPoint = transform.Transform((Point)size);
            return new Size((int)newPoint.X, (int)newPoint.Y);
        }

        public static Line TransformInt(GeneralTransform transform, Line line)
        {
            Point point1 = GeometryUtils.CreateIntPoint(line.X1, line.Y1);
            Point point2 = GeometryUtils.CreateIntPoint(line.X2, line.Y2);
            point1 = transform.Transform(point1);
            point2 = transform.Transform(point2);
            Line newLine = GeometryUtils.CreateLine(point1, point2);
            return newLine;
        }

        public static GeneralTransform CreateTransform(params GeneralTransform[] transforms)
        {
            TransformGroup transformGroup = new TransformGroup();
            foreach (Transform transform in transforms)
            {
                transformGroup.Children.Add(transform);
            }
            return transformGroup;
        }

        #endregion
    }
}
