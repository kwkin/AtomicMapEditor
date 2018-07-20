using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    [Serializable]
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

        public int PixelWidth { get; set; } = 1;
        public int PixelHeight { get; set; } = 1;
        public int TileWidth { get; set; } = 1;
        public int TileHeight { get; set; } = 1;
        public ScaleType Scale { get; set; } = ScaleType.Tile;

        #endregion properties


        #region methods

        public virtual int ColumnCount()
        {
            return this.PixelWidth / this.TileWidth;
        }

        public virtual int RowCount()
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

        #endregion methods
    }
}
