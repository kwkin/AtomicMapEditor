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
            this.Columns.Value = 0;
            this.Rows.Value = 0;
            this.TileWidth.Value = 1;
            this.TileHeight.Value = 1;
            this.Scale.Value = ScaleType.Tile;
        }

        public GridModel(int pixelWidth, int pixelHeight)
        {
            this.Columns.Value = pixelWidth;
            this.Rows.Value = pixelHeight;
            this.TileWidth.Value = 1;
            this.TileHeight.Value = 1;
            this.Scale.Value = ScaleType.Tile;
        }

        public GridModel(int columns, int rows, int tileWidth, int tileHeight)
        {
            this.Columns.Value = columns;
            this.Rows.Value = rows;
            this.TileWidth.Value = tileWidth;
            this.TileHeight.Value = tileHeight;
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property, "Columns")]
        public BindableProperty<int> Columns { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Rows")]
        public BindableProperty<int> Rows { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public BindableProperty<int> TileWidth { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public BindableProperty<int> TileHeight { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Scale")]
        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty.Prepare<ScaleType>();

        // TODO add a readonlyproperty for pixel width and height
        public int PixelWidth
        {
            get
            {
                return GetPixelWidth();
            }
        }
        public int PixelHeight
        {
            get
            {
                return GetPixelHeight();
            }
        }

        #endregion properties


        #region methods

        public virtual int GetPixelWidth()
        {
            return this.Columns.Value * this.TileWidth.Value;
        }

        public virtual int GetPixelHeight()
        {
            return this.Rows.Value * this.TileHeight.Value;
        }

        public Size GetPixelSize()
        {
            return new Size(GetPixelWidth(), GetPixelHeight());
        }

        public Size GetSize()
        {
            return new Size(this.Columns.Value, this.Rows.Value);
        }

        public void SetSize(Size size)
        {
            this.Columns.Value = (int)size.Width;
            this.Rows.Value = (int)size.Height;
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

        // TODO implement methods to calculate horizontal tile index from the pixel point
        public virtual void TileXFromPixel(int pixelPoint)
        {

        }

        public virtual void TileYFromPixel(int pixelPoint)
        {

        }

        public Point GetPoint(Point point)
        {
            Point boundPoint = GeometryUtils.CopyPoint(point);
            if (point.X < 0)
            {
                boundPoint.X = 0;
            }
            else if (point.X > this.PixelWidth)
            {
                boundPoint.X = this.PixelWidth - 1;
            }
            if (point.Y < 0)
            {
                boundPoint.Y = 0;
            }
            else if (point.Y > this.PixelHeight)
            {
                boundPoint.Y = this.PixelHeight - 1;
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
            else if (point.X > this.PixelWidth)
            {
                boundPoint.X = this.PixelWidth;
            }
            if (point.Y < 0)
            {
                boundPoint.Y = 0;
            }
            else if (point.Y > this.PixelHeight)
            {
                boundPoint.Y = this.PixelHeight;
            }
            return boundPoint;
        }

        // TODO factor in padding and offset
        public int GetID(int pixelX, int pixelY)
        {
            return (pixelY / this.TileHeight.Value) * this.Columns.Value + (pixelX / this.TileWidth.Value);

        }

        public int GetID(Point pixelPoint)
        {
            return ((int)pixelPoint.Y / this.TileHeight.Value) * this.Columns.Value + ((int)pixelPoint.X / this.TileWidth.Value);
        }

        public Point GetPointByID(int id)
        {
            int pointX = (id % this.Columns.Value) * this.TileWidth.Value;
            int pointY = (int)Math.Floor((double)(id / this.Columns.Value)) * this.TileHeight.Value;
            return new Point(pointX, pointY);
        }
    }

    #endregion methods
}
