using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Utils;
using System;
using System.ComponentModel;
using System.Windows;

namespace Ame.Infrastructure.Models
{
    public class GridModel
    {
        #region fields

        #endregion fields


        #region constructor

        public GridModel()
            : this(0, 0, 1, 1)
        {
        }

        public GridModel(int pixelWidth, int pixelHeight)
            : this(pixelWidth, pixelHeight, 1, 1)
        {
        }

        public GridModel(int columns, int rows, int tileWidth, int tileHeight)
        {
            this.Columns.Value = columns;
            this.Rows.Value = rows;
            this.TileWidth.Value = tileWidth;
            this.TileHeight.Value = tileHeight;

            this.pixelWidth.Value = GetPixelWidth();
            this.pixelHeight.Value = GetPixelHeight();
            this.Columns.PropertyChanged += UpdatePixelWidth;
            this.Rows.PropertyChanged += UpdatePixelHeight;
            this.TileWidth.PropertyChanged += UpdatePixelWidth;
            this.TileHeight.PropertyChanged += UpdatePixelHeight;
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

        private BindableProperty<int> pixelWidth = BindableProperty.Prepare<int>();
        private ReadOnlyBindableProperty<int> pixelWidthReadOnly;
        public ReadOnlyBindableProperty<int> PixelWidth
        {
            get
            {
                this.pixelWidthReadOnly = this.pixelWidthReadOnly ?? this.pixelWidth.ReadOnlyProperty();
                return this.pixelWidthReadOnly;
            }
        }

        private BindableProperty<int> pixelHeight = BindableProperty.Prepare<int>();
        private ReadOnlyBindableProperty<int> pixelHeightReadOnly;
        public ReadOnlyBindableProperty<int> PixelHeight
        {
            get
            {
                this.pixelHeightReadOnly = this.pixelHeightReadOnly ?? this.pixelHeight.ReadOnlyProperty();
                return this.pixelHeightReadOnly;
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

        public virtual int TileXFromPixel(int pixelPoint)
        {
            return pixelPoint / this.TileWidth.Value;
        }

        public virtual int TileYFromPixel(int pixelPoint)
        {
            return pixelPoint / this.TileHeight.Value;
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

        protected void UpdatePixelWidth(object sender, PropertyChangedEventArgs e)
        {
            this.pixelWidth.Value = GetPixelWidth();
        }

        protected void UpdatePixelWidth()
        {
            this.pixelWidth.Value = GetPixelWidth();
        }

        protected void UpdatePixelHeight(object sender, PropertyChangedEventArgs e)
        {
            this.pixelHeight.Value = GetPixelHeight();
        }

        protected void UpdatePixelHeight()
        {
            this.pixelHeight.Value = GetPixelHeight();
        }
    }

    #endregion methods
}
