using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        public PaddedGrid(int rows, int columns)
            : base(rows, columns)
        {
        }

        public PaddedGrid(int rows, int columns, int tileWidth, int tileHeight)
            : base(rows, columns, tileWidth, tileHeight)
        {
        }

        public PaddedGrid(int rows, int columns, int tileWidth, int tileHeight, int offsetX, int offsetY)
            : base(rows, columns, tileWidth, tileHeight)
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public PaddedGrid(int rows, int columns, int tileWidth, int tileHeight, int offsetX, int offsetY, int paddingX, int paddingY)
            : base(rows, columns, tileWidth, tileHeight)
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.PaddingX = paddingX;
            this.PaddingY = paddingY;
        }

        #endregion constructor


        #region properties

        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public int PaddingX { get; set; } = 0;
        public int PaddingY { get; set; } = 0;

        #endregion properties


        #region methods

        #endregion methods
    }
}
