using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Utils
{
    public class CoordinateTransform
    {
        // TODO add size to the conversion
        // TODO add "module" point to tiled point conversion

        #region fields

        private ScaleTransform pixelToRenderScale = new ScaleTransform();
        private ScaleTransform pixelToTileScale = new ScaleTransform();

        private TransformGroup pixelToRender = new TransformGroup();
        private TransformGroup pixelToTile = new TransformGroup();

        #endregion fields


        #region constructor & destructer

        public CoordinateTransform()
        {
        }

        #endregion constructor & destructer


        #region properties

        #endregion properties


        #region methods
        
        /// <summary>
        /// Sets the tile to pixel transform
        /// </summary>
        /// <param name="tileWidth">The width of the tile in pixels</param>
        /// <param name="tileHeight">The height of the tile in pixels</param>
        public void SetPixelToTile(double pixelsInWidth, double pixelsInHeight)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = (double)1 / pixelsInWidth;
            this.pixelToTileScale.ScaleY = (double)1 / pixelsInHeight;
            pixelToTile.Children.Clear();
            pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToTile(double pixelsInWidth, double pixelsInHeight, double offsetX, double offsetY)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = (double)1 / pixelsInWidth;
            this.pixelToTileScale.ScaleY = (double)1 / pixelsInHeight;

            TranslateTransform pixelToTileTranslate = new TranslateTransform();
            pixelToTileTranslate.X = -offsetX;
            pixelToTileTranslate.Y = -offsetY;

            pixelToTile.Children.Clear();
            pixelToTile.Children.Add(pixelToTileTranslate);
            pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToTile(double pixelsInWidth, double pixelsInHeight, double offsetX, double offsetY, double paddingX, double paddingY)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = (double)1 / (pixelsInWidth + 2 * paddingX);
            this.pixelToTileScale.ScaleY = (double)1 / (pixelsInHeight + 2 * paddingY);

            TranslateTransform pixelToTileTranslate = new TranslateTransform();
            pixelToTileTranslate.X = -offsetX;
            pixelToTileTranslate.Y = -offsetY;

            pixelToTile.Children.Clear();
            pixelToTile.Children.Add(pixelToTileTranslate);
            pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToTile(GridModel gridModel)
        {
            this.pixelToTileScale = new ScaleTransform();
            this.pixelToTileScale.ScaleX = (double)1 / (gridModel.cellWidth + 2 * gridModel.paddingX);
            this.pixelToTileScale.ScaleY = (double)1 / (gridModel.cellHeight + 2 * gridModel.paddingY);

            TranslateTransform pixelToTileTranslate = new TranslateTransform();
            pixelToTileTranslate.X = -gridModel.offsetX;
            pixelToTileTranslate.Y = -gridModel.offsetY;

            pixelToTile.Children.Clear();
            pixelToTile.Children.Add(pixelToTileTranslate);
            pixelToTile.Children.Add(this.pixelToTileScale);
        }

        public void SetPixelToRender(double renderWidth, double renderHeight)
        {
            this.pixelToRenderScale = new ScaleTransform();
            this.pixelToRenderScale.ScaleX = renderWidth;
            this.pixelToRenderScale.ScaleY = renderHeight;
            pixelToRender.Children.Clear();
            pixelToRender.Children.Add(this.pixelToRenderScale);
        }

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

        public Point RenderToPixel(Point point)
        {
            return pixelToRender.Inverse.Transform(point);
        }

        public Line RenderToPixel(Line line)
        {
            Point p1 = RenderToPixel(new Point(line.X1, line.Y1));
            Point p2 = RenderToPixel(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point PixelToRenderInt(Point point)
        {
            Point pixelToRender = PixelToRender(point);
            return new Point((int)pixelToRender.X, (int)pixelToRender.Y);
        }

        public Line PixelToRenderInt(Line line)
        {
            Point p1 = PixelToRenderInt(new Point(line.X1, line.Y1));
            Point p2 = PixelToRenderInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point RenderToPixelInt(Point point)
        {
            Point renderToPixel = RenderToPixel(point);
            return new Point((int)renderToPixel.X, (int)renderToPixel.Y);
        }

        public Line RenderToPixelInt(Line line)
        {
            Point p1 = RenderToPixelInt(new Point(line.X1, line.Y1));
            Point p2 = RenderToPixelInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }


        public Point PixelToTile(Point point)
        {
            return pixelToTile.Transform(point);
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

        public Line PixelToTileInt(Line line)
        {
            Point p1 = PixelToTileInt(new Point(line.X1, line.Y1));
            Point p2 = PixelToTileInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

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

        public Line TileToPixelInt(Line line)
        {
            Point p1 = TileToPixelInt(new Point(line.X1, line.Y1));
            Point p2 = TileToPixelInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point RenderToTile(Point point)
        {
            return pixelToTile.Transform(pixelToRender.Inverse.Transform(point));
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

        public Line RenderToTileInt(Line line)
        {
            Point p1 = RenderToTileInt(new Point(line.X1, line.Y1));
            Point p2 = RenderToTileInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        public Point TileToRender(Point point)
        {
            return pixelToRender.Transform(pixelToTile.Inverse.Transform(point));
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

        public Line TileToRenderInt(Line line)
        {
            Point p1 = TileToRenderInt(new Point(line.X1, line.Y1));
            Point p2 = TileToRenderInt(new Point(line.X2, line.Y2));
            return LineUtils.CreateLine(p1, p2);
        }

        #endregion methods
    }
}
