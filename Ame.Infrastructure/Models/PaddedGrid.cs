using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models
{
    public class PaddedGrid : GridModel
    {
        #region fields

        #endregion fields


        #region constructor

        public PaddedGrid()
            : base()
        {
        }

        public PaddedGrid(int pixelWidth, int pixelHeight)
            : base(pixelWidth, pixelHeight)
        {
            this.OffsetX.Value = 0;
            this.OffsetY.Value = 0;
            this.PaddingX.Value = 0;
            this.PaddingY.Value = 0;
        }

        public PaddedGrid(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.OffsetX.Value = 0;
            this.OffsetY.Value = 0;
            this.PaddingX.Value = 0;
            this.PaddingY.Value = 0;
        }

        public PaddedGrid(int columns, int rows, int tileWidth, int tileHeight, int offsetX, int offsetY)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.OffsetX.Value = 0;
            this.OffsetY.Value = 0;
            this.PaddingX.Value = 0;
            this.PaddingY.Value = 0;
            this.SetWidthWithColumns(columns, tileWidth, offsetX);
            this.SetHeightWithRows(rows, tileHeight, offsetY);
        }

        public PaddedGrid(int columns, int rows, int tileWidth, int tileHeight, int offsetX, int offsetY, int paddingX, int paddingY)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.OffsetX.Value = 0;
            this.OffsetY.Value = 0;
            this.PaddingX.Value = 0;
            this.PaddingY.Value = 0;
            this.SetWidthWithColumns(columns, tileWidth, offsetX, paddingX);
            this.SetHeightWithRows(rows, tileHeight, offsetY, paddingY);
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property, "Offset X")]
        public BindableProperty<int> OffsetX { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Offset Y")]
        public BindableProperty<int> OffsetY { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Padding X")]
        public BindableProperty<int> PaddingX { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Padding Y")]
        public BindableProperty<int> PaddingY { get; set; } = BindableProperty.Prepare<int>();

        #endregion properties


        #region methods

        public override int Columns()
        {
            return (this.PixelWidth.Value - this.OffsetX.Value) / (this.TileWidth.Value + 2 * this.PaddingX.Value);
        }

        public override int Rows()
        {
            return (this.PixelHeight.Value - this.OffsetY.Value) / (this.TileHeight.Value + 2 * this.PaddingY.Value);
        }

        public override double PreciseColumnCount()
        {
            double offsetWidth = this.PixelWidth.Value * this.TileWidth.Value - this.OffsetX.Value;
            double paddedTileWidth = this.TileWidth.Value + 2 * this.PaddingX.Value;
            return offsetWidth / paddedTileWidth;
        }

        public override double PreciseRowCount()
        {
            double offsetHeight = this.PixelHeight.Value * this.TileHeight.Value - this.OffsetY.Value;
            double paddedTileHeight = this.TileHeight.Value + 2 * this.PaddingY.Value;
            return offsetHeight / paddedTileHeight;
        }

        public override void SetWidthWithColumns(int columns, int tileWidth)
        {
            this.TileWidth.Value = tileWidth;
            this.PixelWidth.Value = columns * (this.TileWidth.Value + 2 * this.PaddingX.Value) + this.OffsetX.Value;
        }

        public override void SetHeightWithRows(int rows, int tileHeight)
        {
            this.TileHeight.Value = tileHeight;
            this.PixelHeight.Value = rows * (this.TileHeight.Value + 2 * this.PaddingY.Value) + this.OffsetY.Value;
        }

        public virtual void SetWidthWithColumns(int columns, int tileWidth, int offsetX)
        {
            this.TileWidth.Value = tileWidth;
            this.OffsetX.Value = offsetX;
            this.PixelWidth.Value = columns * (this.TileWidth.Value + 2 * this.PaddingX.Value) + this.OffsetX.Value;
        }

        public virtual void SetHeightWithRows(int rows, int tileHeight, int offsetY)
        {
            this.TileHeight.Value = tileHeight;
            this.OffsetY.Value = offsetY;
            this.PixelHeight.Value = rows * (this.TileHeight.Value + 2 * this.PaddingY.Value) + this.OffsetY.Value;
        }

        public virtual void SetWidthWithColumns(int columns, int tileWidth, int offsetX, int paddingX)
        {
            this.TileWidth.Value = tileWidth;
            this.OffsetX.Value = offsetX;
            this.PaddingX.Value = paddingX;
            this.PixelWidth.Value = columns * (this.TileWidth.Value + 2 * this.PaddingX.Value) + this.OffsetX.Value;
        }

        public virtual void SetHeightWithRows(int rows, int tileHeight, int offsetY, int paddingY)
        {
            this.TileHeight.Value = tileHeight;
            this.OffsetY.Value = offsetY;
            this.PaddingY.Value = paddingY;
            this.PixelHeight.Value = rows * (this.TileHeight.Value + 2 * this.PaddingY.Value) + this.OffsetY.Value;
        }

        #endregion methods
    }
}
