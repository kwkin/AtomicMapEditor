﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public PaddedGrid(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {
        }

        public PaddedGrid(int columns, int rows, int tileWidth, int tileHeight, int offsetX, int offsetY)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.SetWidthWithColumns(columns, tileWidth, offsetX);
            this.SetHeightWithRows(rows, tileHeight, offsetY);
        }

        public PaddedGrid(int columns, int rows, int tileWidth, int tileHeight, int offsetX, int offsetY, int paddingX, int paddingY)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.SetWidthWithColumns(columns, tileWidth, offsetX, paddingX);
            this.SetHeightWithRows(rows, tileHeight, offsetY, paddingY);
        }

        #endregion constructor


        #region properties

        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public int PaddingX { get; set; } = 0;
        public int PaddingY { get; set; } = 0;

        #endregion properties


        #region methods

        public override int ColumnCount()
        {
            return (this.PixelWidth - this.OffsetX) / (this.TileWidth + 2 * this.PaddingX);
        }

        public override int RowCount()
        {
            return (this.PixelHeight - this.OffsetY) / (this.TileHeight + 2 * this.PaddingY);
        }

        public override double PreciseColumnCount()
        {
            double offsetWidth = this.PixelWidth * this.TileWidth - this.OffsetX;
            double paddedTileWidth = this.TileWidth + 2 * this.PaddingX;
            return offsetWidth / paddedTileWidth;
        }

        public override double PreciseRowCount()
        {
            double offsetHeight = this.PixelHeight * this.TileHeight - this.OffsetY;
            double paddedTileHeight = this.TileHeight + 2 * this.PaddingY;
            return offsetHeight / paddedTileHeight;
        }

        public override void SetWidthWithColumns(int columns, int tileWidth)
        {
            this.TileWidth = tileWidth;
            this.PixelWidth = columns * (this.TileWidth + 2 * this.PaddingX) + this.OffsetX;
        }

        public override void SetHeightWithRows(int rows, int tileHeight)
        {
            this.TileHeight = tileHeight;
            this.PixelHeight = rows * (this.TileHeight + 2 * this.PaddingY) + this.OffsetY;
        }

        public virtual void SetWidthWithColumns(int columns, int tileWidth, int offsetX)
        {
            this.TileWidth = tileWidth;
            this.OffsetX = offsetX;
            this.PixelWidth = columns * (this.TileWidth + 2 * this.PaddingX) + this.OffsetX;
        }

        public virtual void SetHeightWithRows(int rows, int tileHeight, int offsetY)
        {
            this.TileHeight = tileHeight;
            this.OffsetY = offsetY;
            this.PixelHeight = rows * (this.TileHeight + 2 * this.PaddingY) + this.OffsetY;
        }

        public virtual void SetWidthWithColumns(int columns, int tileWidth, int offsetX, int paddingX)
        {
            this.TileWidth = tileWidth;
            this.OffsetX = offsetX;
            this.PaddingX = paddingX;
            this.PixelWidth = columns * (this.TileWidth + 2 * this.PaddingX) + this.OffsetX;
        }

        public virtual void SetHeightWithRows(int rows, int tileHeight, int offsetY, int paddingY)
        {
            this.TileHeight = tileHeight;
            this.OffsetY = offsetY;
            this.PaddingY = paddingY;
            this.PixelHeight = rows * (this.TileHeight + 2 * this.PaddingY) + this.OffsetY;
        }

        #endregion methods
    }
}