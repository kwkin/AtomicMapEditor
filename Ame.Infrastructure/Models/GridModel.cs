using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Utils;
using Prism.Mvvm;

namespace Ame.Infrastructure.Models
{
    public class GridModel
    {
        #region fields

        #endregion fields


        #region constructor

        public GridModel()
        {
            this.PixelWidth.Value = 1;
            this.PixelHeight.Value = 1;
            this.TileWidth.Value = 1;
            this.TileHeight.Value = 1;
            this.Scale.Value = ScaleType.Tile;
        }

        public GridModel(int pixelWidth, int pixelHeight)
        {
            this.PixelWidth.Value = pixelWidth;
            this.PixelHeight.Value = pixelHeight;
            this.TileWidth.Value = 1;
            this.TileHeight.Value = 1;
            this.Scale.Value = ScaleType.Tile;
        }

        public GridModel(int columns, int rows, int tileWidth, int tileHeight)
        {
            SetHeightWithRows(rows, tileHeight);
            SetWidthWithColumns(columns, tileWidth);
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property, "Width")]
        public BindableProperty<int> PixelWidth { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Height")]
        public BindableProperty<int> PixelHeight { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public BindableProperty<int> TileWidth { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public BindableProperty<int> TileHeight { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Scale")]
        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty.Prepare<ScaleType>();

        #endregion properties


        #region methods

        public virtual int Columns()
        {
            return this.PixelWidth.Value / this.TileWidth.Value;
        }

        public virtual int Rows()
        {
            return this.PixelHeight.Value / this.TileHeight.Value;
        }

        public virtual double PreciseColumnCount()
        {
            return this.PixelWidth.Value / this.TileWidth.Value;
        }

        public virtual double PreciseRowCount()
        {
            return this.PixelHeight.Value / this.TileHeight.Value;
        }

        public virtual void SetWidthWithColumns(int columns, int tileWidth)
        {
            this.TileWidth.Value = tileWidth;
            this.PixelWidth.Value = this.TileWidth.Value * columns;
        }

        public virtual void SetHeightWithRows(int rows, int tileHeight)
        {
            this.TileHeight.Value = tileHeight;
            this.PixelHeight.Value = this.TileHeight.Value * rows;
        }

        public virtual void SetWidthWithColumns(int columns)
        {
            this.PixelWidth.Value = this.TileWidth.Value * columns;
        }

        public virtual void SetHeightWithRows(int rows)
        {
            this.PixelHeight.Value = this.TileHeight.Value * rows;
        }

        // TODO implement
        public virtual void TileXFromPixel(int pixelPoint)
        {

        }

        public virtual void TileYFromPixel(int pixelPoint)
        {

        }

        public Size GetPixelSize()
        {
            return new Size(this.PixelWidth.Value, this.PixelHeight.Value);
        }

        public void SetPixelSize(Size size)
        {
            this.PixelWidth.Value = (int)size.Width;
            this.PixelHeight.Value = (int)size.Height;
        }

        public Size GetTileSize()
        {
            return new Size(this.TileWidth.Value, this.TileHeight.Value);
        }

        public void SetTileSize(Size size)
        {
            this.TileWidth.Value = (int)size.Width;
            this.TileHeight.Value = (int)size.Height;
        }

        public Point GetPoint(Point point)
        {
            Point boundPoint = GeometryUtils.CopyPoint(point);
            if (point.X < 0)
            {
                boundPoint.X = 0;
            }
            else if (point.X > this.PixelWidth.Value)
            {
                boundPoint.X = this.PixelWidth.Value - 1;
            }
            if (point.Y < 0)
            {
                boundPoint.Y = 0;
            }
            else if (point.Y > this.PixelHeight.Value)
            {
                boundPoint.Y = this.PixelHeight.Value - 1;
            }
            return boundPoint;
        }

        public Point BindPointIncludeSize(Point point)
        {
            Point boundPoint = GeometryUtils.CopyPoint(point);
            if (point.X < 0)
            {
                boundPoint.X = 0;
            }
            else if (point.X > this.PixelWidth.Value)
            {
                boundPoint.X = this.PixelWidth.Value;
            }
            if (point.Y < 0)
            {
                boundPoint.Y = 0;
            }
            else if (point.Y > this.PixelHeight.Value)
            {
                boundPoint.Y = this.PixelHeight.Value;
            }
            return boundPoint;
        }

        // TODO factor in padding and offset
        public int GetID(int pixelX, int pixelY)
        {
            return (pixelY / this.TileHeight.Value) * Columns() + (pixelX / this.TileWidth.Value);

        }

        public int GetID(Point pixelPoint)
        {
            return ((int)pixelPoint.Y / this.TileHeight.Value) * Columns() + ((int)pixelPoint.X / this.TileWidth.Value);
        }

        public Point GetPointByID(int id)
        {
            int pointX = (id % Columns()) * this.TileWidth.Value;
            int pointY = (int)Math.Floor((double)(id / Columns())) * this.TileHeight.Value;
            return new Point(pointX, pointY);
        }
    }

    #endregion methods
}
