using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models
{
    [Serializable]
    public class Layer : ILayer, INotifyPropertyChanged
    {
        #region fields
        
        private List<Tile> occupiedTiles;

        [NonSerialized]
        private DrawingGroup imageDrawings;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

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

        #endregion constructor


        #region properties
        
        private string layerName { get; set; }

        [MetadataProperty(MetadataType.Property, "Name")]
        public string LayerName
        {
            get
            {
                return this.layerName;
            }
            set
            {
                if (this.layerName != value)
                {
                    this.layerName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public int Columns { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public int Rows { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public int TileWidth { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Offset X")]
        public int OffsetX { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Offset Y")]
        public int OffsetY { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public LayerPosition Position { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public ScaleType Scale { get; set; }

        [MetadataProperty(MetadataType.Property, "Scroll Rate")]
        public double ScrollRate { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public string Description { get; set; }
        public bool IsImmutable { get; set; }
        public bool IsVisible { get; set; }

        [NonSerialized]
        private DrawingImage layerItems;

        [IgnoreNodeBuilder]
        public DrawingImage LayerItems
        {
            get
            {
                return this.layerItems;
            }
            set
            {
                this.layerItems = value;
            }
        }

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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion methods
    }
}
