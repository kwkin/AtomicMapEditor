using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Utils
{
    public class CoordinateTransform
    {
        #region fields

        public TransformGroup pixelToSelect = new TransformGroup();
        public TransformGroup pixelToRender = new TransformGroup();
        public TransformGroup pixelToTile = new TransformGroup();

        private ScaleTransform pixelToRenderScale = new ScaleTransform();
        private ScaleTransform pixelToTileScale = new ScaleTransform();
        private TranslateTransform pixelToTileTranslate = new TranslateTransform();
        private TranslateTransform pixelToSelectTranslate = new TranslateTransform();

        #endregion fields


        #region constructor

        public CoordinateTransform()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        #region configure transforms

        public void SetSlectionToPixel(double offsetX, double offsetY)
        {
            this.pixelToSelectTranslate.X = offsetX;
            this.pixelToSelectTranslate.Y = offsetY;
            this.pixelToSelect.Children.Clear();
            this.pixelToSelect.Children.Add(this.pixelToSelectTranslate);
        }

        /// <summary>
        /// Sets the tile to pixel transform
        /// </summary>
        /// <param name="pixelsInTileWidth">The width of the tile in pixels</param>
        /// <param name="pixelsInTileHeight">The height of the tile in pixels</param>
        public void SetPixelToTile(double pixelsInTileWidth, double pixelsInTileHeight)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = 1 / pixelsInTileWidth;
            this.pixelToTileScale.ScaleY = 1 / pixelsInTileHeight;
            this.pixelToTile.Children.Clear();
            this.pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToTile(double pixelsInWidth, double pixelsInHeight, double offsetX, double offsetY)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = 1 / pixelsInWidth;
            this.pixelToTileScale.ScaleY = 1 / pixelsInHeight;

            this.pixelToTileTranslate = new TranslateTransform();
            this.pixelToTileTranslate.X = -offsetX;
            this.pixelToTileTranslate.Y = -offsetY;

            this.pixelToTile.Children.Clear();
            this.pixelToTile.Children.Add(this.pixelToTileTranslate);
            this.pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToTile(double pixelsInWidth, double pixelsInHeight, double offsetX, double offsetY, double paddingX, double paddingY)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = 1 / (pixelsInWidth + 2 * paddingX);
            this.pixelToTileScale.ScaleY = 1 / (pixelsInHeight + 2 * paddingY);

            this.pixelToTileTranslate = new TranslateTransform();
            this.pixelToTileTranslate.X = -offsetX;
            this.pixelToTileTranslate.Y = -offsetY;

            this.pixelToTile.Children.Clear();
            this.pixelToTile.Children.Add(this.pixelToTileTranslate);
            this.pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToTile(GridModel gridModel)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = 1 / (gridModel.cellWidth + 2 * gridModel.paddingX);
            this.pixelToTileScale.ScaleY = 1 / (gridModel.cellHeight + 2 * gridModel.paddingY);

            this.pixelToTileTranslate = new TranslateTransform();
            this.pixelToTileTranslate.X = -gridModel.offsetX;
            this.pixelToTileTranslate.Y = -gridModel.offsetY;

            this.pixelToTile.Children.Clear();
            this.pixelToTile.Children.Add(this.pixelToTileTranslate);
            this.pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToRender(double renderWidth, double renderHeight)
        {
            this.pixelToRenderScale = new ScaleTransform();
            this.pixelToRenderScale.ScaleX = renderWidth;
            this.pixelToRenderScale.ScaleY = renderHeight;
            this.pixelToRender.Children.Clear();
            this.pixelToRender.Children.Add(this.pixelToRenderScale);
        }

        #endregion configure transforms

        #region util functions

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
            Line newLine = LineUtils.CreateLine(point1, point2);
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
            Point point1 = PointUtils.CreateIntPoint(line.X1, line.Y1);
            Point point2 = PointUtils.CreateIntPoint(line.X2, line.Y2);
            point1 = transform.Transform(point1);
            point2 = transform.Transform(point2);
            Line newLine = LineUtils.CreateLine(point1, point2);
            return newLine;
        }

        public GeneralTransform CreateTransform(params GeneralTransform[] transforms)
        {
            TransformGroup transformGroup = new TransformGroup();
            foreach (Transform transform in transforms)
            {
                transformGroup.Children.Add(transform);
            }
            return transformGroup;
        }

        public Point PixelToTopLeftTileEdge(Point point)
        {
            Point edgePoint = pixelToTile.Transform(point);
            return pixelToTile.Inverse.Transform(new Point((int)edgePoint.X, (int)edgePoint.Y));
        }

        public Point PixelToTopRightTileEdge(Point pixelPoint)
        {
            Point topRightTilePixel = this.PixelToTopLeftTileEdge(pixelPoint);
            topRightTilePixel.X = pixelPoint.X + (1 / this.pixelToTileScale.ScaleX);
            return topRightTilePixel;
        }

        public Point PixelToBottomLeftTileEdge(Point pixelPoint)
        {
            Point bottomLeftTilePixel = this.PixelToTopLeftTileEdge(pixelPoint);
            bottomLeftTilePixel.Y = pixelPoint.Y + (1 / this.pixelToTileScale.ScaleY);
            return bottomLeftTilePixel;
        }

        public Point PixelToBottomRightTileEdge(Point pixelPoint)
        {
            Point bottomRightTilePixel = this.PixelToTopLeftTileEdge(pixelPoint);
            bottomRightTilePixel.X = pixelPoint.X + (1 / this.pixelToTileScale.ScaleX);
            bottomRightTilePixel.Y = pixelPoint.Y + (1 / this.pixelToTileScale.ScaleY);
            return bottomRightTilePixel;
        }


        #endregion util functions

        #endregion methods
    }
}
