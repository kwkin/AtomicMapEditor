using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class PaddedGridRenderable : PaddedGrid
    {
        #region fields

        #endregion fields


        #region constructor

        public PaddedGridRenderable()
            : base()
        {
        }

        public PaddedGridRenderable(int rows, int columns)
            : base(rows, columns)
        {
        }

        public PaddedGridRenderable(int rows, int columns, int tileWidth, int tileHeight)
            : base(rows, columns, tileWidth, tileHeight)
        {
        }

        public PaddedGridRenderable(int rows, int columns, int tileWidth, int tileHeight, int offsetX, int offsetY)
            : base(rows, columns, tileWidth, tileHeight, offsetX, offsetY)
        {
        }

        public PaddedGridRenderable(int rows, int columns, int tileWidth, int tileHeight, int offsetX, int offsetY, int paddingX, int paddingY)
            : base(rows, columns, tileWidth, tileHeight, offsetX, offsetY, paddingX, paddingY)
        {
        }

        #endregion constructor


        #region properties
        
        public Brush DrawingBrush { get; set; } = Brushes.Transparent;
        public Pen DrawingPen { get; set; } = new Pen(Brushes.Black, 1);

        #endregion properties


        #region methods

        public DrawingGroup CreateGrid()
        {
            DrawingGroup drawingGroup;
            if (this.OffsetX == 0 && this.OffsetY == 0)
            {
                if (this.PaddingX == 0 && this.PaddingY == 0)
                {
                    drawingGroup = CreateGridDefault();
                }
                else
                {
                    drawingGroup = CreateGridPadding();
                }
            }
            else if (this.PaddingX == 0 && this.PaddingY == 0)
            {
                drawingGroup = CreateGridOffset();
            }
            else
            {
                drawingGroup = CreateGridOffsetPadding();
            }
            return drawingGroup;
        }

        private DrawingGroup CreateGridDefault()
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)(this.Rows);
            int numTilesY = (int)(this.Columns);
            int gridWidth = (int)(numTilesX * this.TileWidth);
            int gridHeight = (int)(numTilesY * this.TileHeight);
            double borderWidth = numTilesX * this.TileWidth;
            double borderHeight = this.Columns * this.TileHeight;

            Rect border = new Rect();
            border.Size = new Size(borderWidth, borderHeight);
            border.Location = new Point(0, 0);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(this.DrawingBrush, this.DrawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(0, this.TileHeight * index);
                    Point pointStop = new Point(gridWidth, this.TileHeight * index);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(this.TileWidth * index, 0);
                    Point pointStop = new Point(this.TileWidth * index, gridHeight);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private DrawingGroup CreateGridOffset()
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((this.Rows * this.TileWidth - this.OffsetX) / this.TileWidth);
            int numTilesY = (int)((this.Columns * this.TileHeight - this.OffsetY) / this.TileHeight);
            double borderWidth = numTilesX * this.TileWidth;
            double borderHeight = this.Columns * this.TileHeight;


            Rect border = new Rect();
            border.Size = new Size(borderWidth, borderHeight);
            border.Location = new Point(this.OffsetX, this.OffsetY);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(this.DrawingBrush, this.DrawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(this.OffsetX, this.OffsetY + this.TileHeight * index);
                    Point pointStop = new Point(numTilesX * this.TileWidth + this.OffsetX, this.OffsetY + this.TileHeight * index);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(this.OffsetX + this.TileWidth * index, this.OffsetY);
                    Point pointStop = new Point(this.OffsetX + this.TileWidth * index, numTilesY * this.TileHeight + this.OffsetY);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private DrawingGroup CreateGridPadding()
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((this.Rows * this.TileWidth) / (this.TileWidth + 2 * this.PaddingX));
            int numTilesY = (int)((this.Columns * this.TileHeight) / (this.TileHeight + 2 * this.PaddingY));
            Size tileSize = new Size(this.TileWidth, this.TileHeight);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = xIndex * this.TileWidth + (2 * this.PaddingX * xIndex + this.PaddingX);
                        double locationY = yIndex * this.TileHeight + (2 * this.PaddingY * yIndex + this.PaddingY);
                        Point location = new Point(locationX, locationY);
                        Rect tile = new Rect(location, tileSize);
                        context.DrawRectangle(this.DrawingBrush, this.DrawingPen, tile);
                    }
                }
            }
            return gridItems;
        }

        private DrawingGroup CreateGridOffsetPadding()
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((this.Rows * this.TileWidth - this.OffsetX) / (this.TileWidth + 2 * this.PaddingX));
            int numTilesY = (int)((this.Columns * this.TileHeight - this.OffsetY) / (this.TileHeight + 2 * this.PaddingY));
            Size tileSize = new Size(this.TileWidth, this.TileHeight);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = this.OffsetX + xIndex * this.TileWidth + (2 * this.PaddingX * xIndex + this.PaddingX);
                        double locationY = this.OffsetY + yIndex * this.TileHeight + (2 * this.PaddingY * yIndex + this.PaddingY);
                        Point location = new Point(locationX, locationY);
                        Rect tile = new Rect(location, tileSize);
                        context.DrawRectangle(this.DrawingBrush, this.DrawingPen, tile);
                    }
                }
            }
            return gridItems;
        }

        #endregion methods
    }
}
