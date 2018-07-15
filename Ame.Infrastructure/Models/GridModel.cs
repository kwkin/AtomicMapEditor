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

        public GridModel(double rows, double columns, double cellWidth, double cellHeight)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
        }

        public GridModel(double rows, double columns, double cellWidth, double cellHeight, double offsetX, double offsetY)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public GridModel(double rows, double columns, double cellWidth, double cellHeight, double offsetX, double offsetY, double paddingX, double paddingY)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.PaddingX = paddingX;
            this.PaddingY = paddingY;
        }

        #endregion constructor


        #region properties

        public double Rows { get; set; } = 1;
        public double Columns { get; set; } = 1;
        public double CellWidth { get; set; } = 1;
        public double CellHeight { get; set; } = 1;
        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;
        public double PaddingX { get; set; } = 0;
        public double PaddingY { get; set; } = 0;
        public Brush DrawingBrush { get; set; } = Brushes.Transparent;
        public Pen DrawingPen { get; set; } = new Pen(Brushes.Black, 1);

        #endregion properties


        #region methods

        public static DrawingGroup CreateGrid(GridModel gridParameters)
        {
            DrawingGroup drawingGroup;
            if (gridParameters.OffsetX == 0 && gridParameters.OffsetY == 0)
            {
                if (gridParameters.PaddingX == 0 && gridParameters.PaddingY == 0)
                {
                    drawingGroup = CreateGridDefault(gridParameters);
                }
                else
                {
                    drawingGroup = CreateGridPadding(gridParameters);
                }
            }
            else if (gridParameters.PaddingX == 0 && gridParameters.PaddingY == 0)
            {
                drawingGroup = CreateGridOffset(gridParameters);
            }
            else
            {
                drawingGroup = CreateGridOffsetPadding(gridParameters);
            }
            return drawingGroup;
        }

        private static DrawingGroup CreateGridDefault(GridModel gridParameters)
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)(gridParameters.Rows);
            int numTilesY = (int)(gridParameters.Columns);
            int gridWidth = (int)(numTilesX * gridParameters.CellWidth);
            int gridHeight = (int)(numTilesY * gridParameters.CellHeight);
            double borderWidth = numTilesX * gridParameters.CellWidth;
            double borderHeight = gridParameters.Columns * gridParameters.CellHeight;

            Rect border = new Rect();
            border.Size = new Size(borderWidth, borderHeight);
            border.Location = new Point(0, 0);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(gridParameters.DrawingBrush, gridParameters.DrawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(0, gridParameters.CellHeight * index);
                    Point pointStop = new Point(gridWidth, gridParameters.CellHeight * index);
                    context.DrawLine(gridParameters.DrawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(gridParameters.CellWidth * index, 0);
                    Point pointStop = new Point(gridParameters.CellWidth * index, gridHeight);
                    context.DrawLine(gridParameters.DrawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private static DrawingGroup CreateGridOffset(GridModel gridParameters)
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((gridParameters.Rows * gridParameters.CellWidth - gridParameters.OffsetX) / gridParameters.CellWidth);
            int numTilesY = (int)((gridParameters.Columns * gridParameters.CellHeight - gridParameters.OffsetY) / gridParameters.CellHeight);
            double borderWidth = numTilesX * gridParameters.CellWidth;
            double borderHeight = gridParameters.Columns * gridParameters.CellHeight;


            Rect border = new Rect();
            border.Size = new Size(borderWidth, borderHeight);
            border.Location = new Point(gridParameters.OffsetX, gridParameters.OffsetY);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(gridParameters.DrawingBrush, gridParameters.DrawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(gridParameters.OffsetX, gridParameters.OffsetY + gridParameters.CellHeight * index);
                    Point pointStop = new Point(numTilesX * gridParameters.CellWidth + gridParameters.OffsetX, gridParameters.OffsetY + gridParameters.CellHeight * index);
                    context.DrawLine(gridParameters.DrawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(gridParameters.OffsetX + gridParameters.CellWidth * index, gridParameters.OffsetY);
                    Point pointStop = new Point(gridParameters.OffsetX + gridParameters.CellWidth * index, numTilesY * gridParameters.CellHeight + gridParameters.OffsetY);
                    context.DrawLine(gridParameters.DrawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private static DrawingGroup CreateGridPadding(GridModel gridParameters)
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((gridParameters.Rows * gridParameters.CellWidth) / (gridParameters.CellWidth + 2 * gridParameters.PaddingX));
            int numTilesY = (int)((gridParameters.Columns * gridParameters.CellHeight) / (gridParameters.CellHeight + 2 * gridParameters.PaddingY));
            Size tileSize = new Size(gridParameters.CellWidth, gridParameters.CellHeight);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = xIndex * gridParameters.CellWidth + (2 * gridParameters.PaddingX * xIndex + gridParameters.PaddingX);
                        double locationY = yIndex * gridParameters.CellHeight + (2 * gridParameters.PaddingY * yIndex + gridParameters.PaddingY);
                        Point location = new Point(locationX, locationY);
                        Rect tile = new Rect(location, tileSize);
                        context.DrawRectangle(gridParameters.DrawingBrush, gridParameters.DrawingPen, tile);
                    }
                }
            }
            return gridItems;
        }

        private static DrawingGroup CreateGridOffsetPadding(GridModel gridParameters)
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((gridParameters.Rows * gridParameters.CellWidth - gridParameters.OffsetX) / (gridParameters.CellWidth + 2 * gridParameters.PaddingX));
            int numTilesY = (int)((gridParameters.Columns * gridParameters.CellHeight - gridParameters.OffsetY) / (gridParameters.CellHeight + 2 * gridParameters.PaddingY));
            Size tileSize = new Size(gridParameters.CellWidth, gridParameters.CellHeight);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = gridParameters.OffsetX + xIndex * gridParameters.CellWidth + (2 * gridParameters.PaddingX * xIndex + gridParameters.PaddingX);
                        double locationY = gridParameters.OffsetY + yIndex * gridParameters.CellHeight + (2 * gridParameters.PaddingY * yIndex + gridParameters.PaddingY);
                        Point location = new Point(locationX, locationY);
                        Rect tile = new Rect(location, tileSize);
                        context.DrawRectangle(gridParameters.DrawingBrush, gridParameters.DrawingPen, tile);
                    }
                }
            }
            return gridItems;
        }

        #endregion methods
    }
}
