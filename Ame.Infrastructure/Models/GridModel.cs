using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        public GridModel(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
        }

        public GridModel(int rows, int columns, int cellWidth, int cellHeight)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.TileWidth = cellWidth;
            this.TileHeight = cellHeight;
        }

        #endregion constructor


        #region properties

        public int Rows { get; set; } = 1;
        public int Columns { get; set; } = 1;
        public int TileWidth { get; set; } = 1;
        public int TileHeight { get; set; } = 1;
        public ScaleType Scale { get; set; }

        public int PixelWidth
        {
            get
            {
                int width = this.Columns;
                switch (this.Scale)
                {
                    case ScaleType.Tile:
                        width *= this.TileWidth;
                        break;

                    case ScaleType.Pixel:
                    default:
                        break;
                }
                return width;
            }
        }

        public int PixelHeight
        {
            get
            {
                int height = this.Rows;
                switch (this.Scale)
                {
                    case ScaleType.Tile:
                        height *= this.TileHeight;
                        break;

                    case ScaleType.Pixel:
                    default:
                        break;
                }
                return height;
            }
        }

        #endregion properties


        #region methods
        
        #endregion methods
    }
}
