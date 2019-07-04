using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ame.Infrastructure.Utils;

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

        public PaddedGridRenderable(int pixelWidth, int pixelHeight)
            : base(pixelWidth, pixelHeight)
        {
        }

        public PaddedGridRenderable(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {
        }

        public PaddedGridRenderable(int columns, int rows, int tileWidth, int tileHeight, int offsetX, int offsetY)
            : base(columns, rows, tileWidth, tileHeight, offsetX, offsetY)
        {
        }

        public PaddedGridRenderable(int columns, int rows, int tileWidth, int tileHeight, int offsetX, int offsetY, int paddingX, int paddingY)
            : base(columns, rows, tileWidth, tileHeight, offsetX, offsetY, paddingX, paddingY)
        {
        }

        public PaddedGridRenderable(GridModel grid)
            : base(grid.Columns.Value, grid.Rows.Value, grid.TileWidth.Value, grid.TileHeight.Value)
        {

        }

        public PaddedGridRenderable(PaddedGrid grid)
            : base(grid.Columns.Value, grid.Rows.Value, grid.TileWidth.Value, grid.TileHeight.Value, grid.OffsetX.Value, grid.OffsetY.Value, grid.PaddingX.Value, grid.PaddingY.Value)
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
            if (this.OffsetX.Value == 0 && this.OffsetY.Value == 0)
            {
                if (this.PaddingX.Value == 0 && this.PaddingY.Value == 0)
                {
                    drawingGroup = CreateGridDefault();
                }
                else
                {
                    drawingGroup = CreateGridPadding();
                }
            }
            else if (this.PaddingX.Value == 0 && this.PaddingY.Value == 0)
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

            int numTilesX = this.Columns.Value;
            int numTilesY = this.Rows.Value;

            Rect border = new Rect();
            border.Size = new Size(this.PixelWidth.Value, this.PixelHeight.Value);
            border.Location = new Point(0, 0);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(this.DrawingBrush, this.DrawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(0, this.TileHeight.Value * index);
                    Point pointStop = new Point(this.PixelWidth.Value, this.TileHeight.Value * index);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(this.TileWidth.Value * index, 0);
                    Point pointStop = new Point(this.TileWidth.Value * index, this.PixelHeight.Value);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private DrawingGroup CreateGridOffset()
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = this.Columns.Value;
            int numTilesY = this.Rows.Value;
            
            Rect border = new Rect();
            border.Size = new Size(this.TileHeight.Value * numTilesX, this.TileWidth.Value * numTilesY);
            border.Location = new Point(this.OffsetX.Value, this.OffsetY.Value);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(this.DrawingBrush, this.DrawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(this.OffsetX.Value, this.OffsetY.Value + this.TileHeight.Value * index);
                    Point pointStop = new Point(numTilesX * this.TileWidth.Value + this.OffsetX.Value, this.OffsetY.Value + this.TileHeight.Value * index);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(this.OffsetX.Value + this.TileWidth.Value * index, this.OffsetY.Value);
                    Point pointStop = new Point(this.OffsetX.Value + this.TileWidth.Value * index, numTilesY * this.TileHeight.Value + this.OffsetY.Value);
                    context.DrawLine(this.DrawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private DrawingGroup CreateGridPadding()
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = this.Columns.Value;
            int numTilesY = this.Rows.Value;
            Size tileSize = new Size(this.TileWidth.Value, this.TileHeight.Value);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = xIndex * this.TileWidth.Value + (2 * this.PaddingX.Value * xIndex + this.PaddingX.Value);
                        double locationY = yIndex * this.TileHeight.Value + (2 * this.PaddingY.Value * yIndex + this.PaddingY.Value);
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

            int numTilesX = this.Columns.Value;
            int numTilesY = this.Rows.Value;
            Size tileSize = new Size(this.TileWidth.Value, this.TileHeight.Value);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = this.OffsetX.Value + xIndex * this.TileWidth.Value + (2 * this.PaddingX.Value * xIndex + this.PaddingX.Value);
                        double locationY = this.OffsetY.Value + yIndex * this.TileHeight.Value + (2 * this.PaddingY.Value * yIndex + this.PaddingY.Value);
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
