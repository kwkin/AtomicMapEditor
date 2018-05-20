using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public struct GridModel
    {
        public double rows;
        public double columns;
        public double cellWidth;
        public double cellHeight;
        public double offsetX;
        public double offsetY;
        public double paddingX;
        public double paddingY;

        public GridModel(double rows, double columns, double cellWidth, double cellHeight)
        {
            this.rows = rows;
            this.columns = columns;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.offsetX = 0;
            this.offsetY = 0;
            this.paddingX = 0;
            this.paddingY = 0;
        }

        public GridModel(double rows, double columns, double cellWidth, double cellHeight, double offsetX, double offsetY)
        {
            this.rows = rows;
            this.columns = columns;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.paddingX = 0;
            this.paddingY = 0;
        }

        public GridModel(double rows, double columns, double cellWidth, double cellHeight, double offsetX, double offsetY, double paddingX, double paddingY)
        {
            this.rows = rows;
            this.columns = columns;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.paddingX = paddingX;
            this.paddingY = paddingY;
        }
    }
}
