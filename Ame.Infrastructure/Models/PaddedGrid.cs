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
            this.OffsetX.Value = offsetX;
            this.OffsetY.Value = offsetY;
            this.PaddingX.Value = 0;
            this.PaddingY.Value = 0;
        }

        public PaddedGrid(int columns, int rows, int tileWidth, int tileHeight, int offsetX, int offsetY, int paddingX, int paddingY)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.OffsetX.Value = offsetX;
            this.OffsetY.Value = offsetY;
            this.PaddingX.Value = paddingX;
            this.PaddingY.Value = paddingY;
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

        public override int GetPixelWidth()
        {
            return this.Columns.Value * (this.TileWidth.Value + 2 * this.PaddingX.Value) + this.OffsetX.Value;
        }

        public override int GetPixelHeight()
        {
            return this.Rows.Value * (this.TileHeight.Value + 2 * this.PaddingY.Value) + this.OffsetY.Value;
        }

        #endregion methods
    }
}
