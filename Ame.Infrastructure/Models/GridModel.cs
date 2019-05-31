using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Utils;

namespace Ame.Infrastructure.Models
{
    public class GridModel
    {
        #region fields

        #endregion fields


        #region constructor

        public GridModel()
        {
        }

        public GridModel(int pixelWidth, int pixelHeight)
        {
            this.PixelWidth = pixelWidth;
            this.PixelHeight = pixelHeight;
        }

        public GridModel(int columns, int rows, int tileWidth, int tileHeight)
        {
            SetHeightWithRows(rows, tileHeight);
            SetWidthWithColumns(columns, tileWidth);
        }

        #endregion constructor


        #region properties
        
        [MetadataProperty(MetadataType.Property, "Width")]
        public int PixelWidth { get; set; } = 1;
        
        [MetadataProperty(MetadataType.Property, "Height")]
        public int PixelHeight { get; set; } = 1;

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public int TileWidth { get; set; } = 1;

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight { get; set; } = 1;

        public ScaleType Scale { get; set; } = ScaleType.Tile;
        
        public Size PixelSize
        {
            get
            {
                return new Size(this.PixelWidth, this.PixelHeight);
            }
            set
            {
                this.PixelWidth = (int)value.Width;
                this.PixelHeight = (int)value.Height;
            }
        }
        
        public Size TileSize
        {
            get
            {
                return new Size(this.TileWidth, this.TileHeight);
            }
            set
            {
                this.TileWidth = (int)value.Width;
                this.TileHeight = (int)value.Height;
            }
        }

        #endregion properties


        #region methods

        public virtual int Columns()
        {
            return this.PixelWidth / this.TileWidth;
        }

        public virtual int Rows()
        {
            return this.PixelHeight / this.TileHeight;
        }

        public virtual double PreciseColumnCount()
        {
            return this.PixelWidth / this.TileWidth;
        }

        public virtual double PreciseRowCount()
        {
            return this.PixelHeight / this.TileHeight;
        }

        public virtual void SetWidthWithColumns(int columns, int tileWidth)
        {
            this.TileWidth = tileWidth;
            this.PixelWidth = this.TileWidth * columns;
        }

        public virtual void SetHeightWithRows(int rows, int tileHeight)
        {
            this.TileHeight = tileHeight;
            this.PixelHeight = this.TileHeight * rows;
        }

        public virtual void SetWidthWithColumns(int columns)
        {
            this.PixelWidth = this.TileWidth * columns;
        }

        public virtual void SetHeightWithRows(int rows)
        {
            this.PixelHeight = this.TileHeight * rows;
        }

        // TODO implement
        public virtual void TileXFromPixel(int pixelPoint)
        {

        }

        public virtual void TileYFromPixel(int pixelPoint)
        {

        }

        public Point BindPoint(Point point)
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
            return (pixelY / this.TileHeight) * Columns() + (pixelX / this.TileWidth);

        }

        public int GetID(Point pixelPoint)
        {
            return ((int)pixelPoint.Y / this.TileHeight) * Columns() + ((int)pixelPoint.X / this.TileWidth);
        }

        public Point GetPointByID(int id)
        {
            int pointX = (id % Columns()) * this.TileWidth;
            int pointY = (int)Math.Floor((double)(id / Columns())) * this.TileHeight;
            return new Point(pointX, pointY);
        }
    }

    #endregion methods
}
