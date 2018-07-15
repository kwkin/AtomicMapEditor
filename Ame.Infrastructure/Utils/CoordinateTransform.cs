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
    // TODO create a selection to pixel
    public class CoordinateTransform
    {
        #region fields

        private ScaleTransform pixelToRenderScale = new ScaleTransform();
        private ScaleTransform pixelToTileScale = new ScaleTransform();
        private TranslateTransform pixelToTileTranslate = new TranslateTransform();

        private TransformGroup pixelToRender = new TransformGroup();
        private TransformGroup pixelToTile = new TransformGroup();

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

        /// <summary>
        /// Sets the tile to pixel transform
        /// </summary>
        /// <param name="tileWidth">The width of the tile in pixels</param>
        /// <param name="tileHeight">The height of the tile in pixels</param>
        public void SetPixelToTile(double pixelsInWidth, double pixelsInHeight)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = 1 / pixelsInWidth;
            this.pixelToTileScale.ScaleY = 1 / pixelsInHeight;
            pixelToTile.Children.Clear();
            pixelToTile.Children.Add(this.pixelToTileScale);
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

        #region pixel to render

        public Point PixelToRender(Point point)
        {
            return pixelToRender.Transform(point);
        }

        public Size PixelToRender(Size size)
        {
            Point pointSize = pixelToRender.Transform(new Point(size.Width, size.Height));
            return new Size(pointSize.X, pointSize.Y);
        }

        public Line PixelToRender(Line line)
        {
            Point p1 = PixelToRender(new Point(line.X1, line.Y1));
            Point p2 = PixelToRender(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point PixelToRenderInt(Point point)
        {
            Point pixelToRender = PixelToRender(point);
            return new Point((int)pixelToRender.X, (int)pixelToRender.Y);
        }

        public Size PixelToRenderInt(Size size)
        {
            Size pixelToRender = PixelToRender(size);
            return new Size((int)pixelToRender.Width, (int)pixelToRender.Height);
        }

        public Line PixelToRenderInt(Line line)
        {
            Point p1 = PixelToRenderInt(new Point(line.X1, line.Y1));
            Point p2 = PixelToRenderInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        #endregion pixel to render

        #region render to pixel

        public Point RenderToPixel(Point point)
        {
            return pixelToRender.Inverse.Transform(point);
        }

        public Size RenderToPixel(Size size)
        {
            Point pointSize = pixelToRender.Inverse.Transform(new Point(size.Width, size.Height));
            return new Size(pointSize.X, pointSize.Y);
        }

        public Line RenderToPixel(Line line)
        {
            Point p1 = RenderToPixel(new Point(line.X1, line.Y1));
            Point p2 = RenderToPixel(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point RenderToPixelInt(Point point)
        {
            Point renderToPixel = RenderToPixel(point);
            return new Point((int)renderToPixel.X, (int)renderToPixel.Y);
        }

        public Size RenderToPixelInt(Size size)
        {
            Size renderToPixel = RenderToPixel(size);
            return new Size((int)renderToPixel.Width, (int)renderToPixel.Height);
        }

        public Line RenderToPixelInt(Line line)
        {
            Point p1 = RenderToPixelInt(new Point(line.X1, line.Y1));
            Point p2 = RenderToPixelInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        #endregion render to pixel

        #region pixel to tile

        public Point PixelToTile(Point point)
        {
            return pixelToTile.Transform(point);
        }

        public Size PixelToTile(Size size)
        {
            Point pointSize = pixelToTile.Transform(new Point(size.Width, size.Height));
            return new Size(pointSize.X, pointSize.Y);
        }

        public Line PixelToTile(Line line)
        {
            Point p1 = PixelToTile(new Point(line.X1, line.Y1));
            Point p2 = PixelToTile(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point PixelToTileInt(Point point)
        {
            Point pixelToTile = PixelToTile(point);
            return new Point((int)pixelToTile.X, (int)pixelToTile.Y);
        }

        public Size PixelToTileInt(Size size)
        {
            Size pixelToTile = PixelToTile(size);
            return new Size((int)pixelToTile.Width, (int)pixelToTile.Height);
        }

        public Line PixelToTileInt(Line line)
        {
            Point p1 = PixelToTileInt(new Point(line.X1, line.Y1));
            Point p2 = PixelToTileInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        #endregion pixel to tile

        #region tile to pixel

        public Point TileToPixel(Point point)
        {
            return pixelToTile.Inverse.Transform(point);
        }

        public Size TileToPixel(Size size)
        {
            Point pointSize = pixelToTileScale.Inverse.Transform(new Point(size.Width, size.Height));
            return new Size(pointSize.X, pointSize.Y);
        }

        public Line TileToPixel(Line line)
        {
            Point p1 = TileToPixel(new Point(line.X1, line.Y1));
            Point p2 = TileToPixel(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point TileToPixelInt(Point point)
        {
            Point tileToPixel = TileToPixel(point);
            return new Point((int)tileToPixel.X, (int)tileToPixel.Y);
        }

        public Point TileToPixelInt(Size size)
        {
            Size tileToPixel = TileToPixel(size);
            return new Point((int)tileToPixel.Width, (int)tileToPixel.Height);
        }

        public Line TileToPixelInt(Line line)
        {
            Point p1 = TileToPixelInt(new Point(line.X1, line.Y1));
            Point p2 = TileToPixelInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        #endregion tile to pixel

        #region render to tile

        public Point RenderToTile(Point point)
        {
            return pixelToTile.Transform(pixelToRender.Inverse.Transform(point));
        }

        public Size RenderToTile(Size size)
        {
            Point pointSize = pixelToTile.Transform(new Point(size.Width, size.Height));
            return new Size(pointSize.X, pointSize.Y);
        }

        public Line RenderToTile(Line line)
        {
            Point p1 = RenderToTile(new Point(line.X1, line.Y1));
            Point p2 = RenderToTile(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point RenderToTileInt(Point point)
        {
            Point renderToTile = RenderToTile(point);
            return new Point((int)renderToTile.X, (int)renderToTile.Y);
        }

        public Point RenderToTileInt(Size size)
        {
            Size renderToTile = RenderToTile(size);
            return new Point((int)renderToTile.Width, (int)renderToTile.Height);
        }

        public Line RenderToTileInt(Line line)
        {
            Point p1 = RenderToTileInt(new Point(line.X1, line.Y1));
            Point p2 = RenderToTileInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        #endregion render to tile

        #region tile to render

        public Point TileToRender(Point point)
        {
            return pixelToRender.Transform(pixelToTile.Inverse.Transform(point));
        }

        public Size TileToRender(Size size)
        {
            Point pointSize = pixelToTile.Transform(new Point(size.Width, size.Height));
            return new Size(pointSize.X, pointSize.Y);
        }

        public Line TileToRender(Line line)
        {
            Point p1 = TileToRender(new Point(line.X1, line.Y1));
            Point p2 = TileToRender(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point TileToRenderInt(Point point)
        {
            Point tileToRender = TileToRender(point);
            return new Point((int)tileToRender.X, (int)tileToRender.Y);
        }

        public Point TileToRenderInt(Size size)
        {
            Size tileToRender = TileToRender(size);
            return new Point((int)tileToRender.Width, (int)tileToRender.Height);
        }

        public Line TileToRenderInt(Line line)
        {
            Point p1 = TileToRenderInt(new Point(line.X1, line.Y1));
            Point p2 = TileToRenderInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        #endregion tile to render

        #region util functions
        
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
