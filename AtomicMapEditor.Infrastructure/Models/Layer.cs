using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ame.Infrastructure.Models
{
    public class Layer : ILayer
    {
        #region fields

        private List<Tile> occupiedTiles;
        private DrawingGroup imageDrawings;

        #endregion fields


        #region constructor & destructer

        public Layer(int tileWidth, int tileHeight)
        {
            this.LayerName = "";
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Position = LayerPosition.Base;

            this.occupiedTiles = new List<Tile>();
        }

        public Layer(int tileWidth, int tileHeight, int rows, int columns)
        {
            this.LayerName = "";
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;

            this.occupiedTiles = new List<Tile>();
            ResetLayerItems();
        }

        public Layer(string layerName, int tileWidth, int tileHeight)
        {
            this.LayerName = layerName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Position = LayerPosition.Base;

            this.occupiedTiles = new List<Tile>();
        }

        public Layer(string layerName, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.LayerName = layerName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;

            this.occupiedTiles = new List<Tile>();
            ResetLayerItems();
        }

        #endregion constructor & destructer


        #region properties
        
        public string LayerName { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public bool IsImmutable { get; set; }
        public LayerPosition Position { get; set; }

        public DrawingImage LayerItems { get; set; }

        #endregion properties


        #region methods

        public int GetPixelWidth()
        {
            return this.TileWidth * this.Columns;
        }

        public int GetPixelHeight()
        {
            return this.TileHeight * this.Rows;
        }

        public void SetTile(BitmapImage image, Point tilePoint)
        {
            // TODO improve this
            // TODO look into rendering using an array
            Rect rect = new Rect(tilePoint.X, tilePoint.Y, image.Width, image.Height);
            ImageDrawing tileImage = new ImageDrawing(image, rect);
            foreach (Tile tile in occupiedTiles)
            {
                if (tile.Position == tilePoint)
                {
                    this.occupiedTiles.Remove(tile);
                    this.imageDrawings.Children.Remove(tile.TileImage);
                    break;
                }
            }
            this.occupiedTiles.Add(new Models.Tile(tilePoint, tileImage));
            this.imageDrawings.Children.Add(tileImage);
            this.LayerItems = new DrawingImage(this.imageDrawings);
        }

        private void ResetLayerItems()
        {
            int pixelWidth = GetPixelWidth();
            int pixelHeight = GetPixelHeight();

            GeometryGroup rectangles = new GeometryGroup();
            rectangles.Children.Add(new RectangleGeometry(new Rect(0, 0, pixelWidth, pixelHeight)));
            GeometryDrawing aGeometryDrawing = new GeometryDrawing();
            aGeometryDrawing.Geometry = rectangles;
            aGeometryDrawing.Brush = new SolidColorBrush(Colors.Transparent);
            this.imageDrawings = new DrawingGroup();
            this.imageDrawings.Children.Add(aGeometryDrawing);
            this.LayerItems = new DrawingImage(this.imageDrawings);
        }

        #endregion methods
    }
}
