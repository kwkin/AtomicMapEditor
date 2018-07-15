using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    // TODO combine grid model with tileset model
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
            this.rows = rows;
            this.columns = columns;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
        }

        public GridModel(double rows, double columns, double cellWidth, double cellHeight, double offsetX, double offsetY)
        {
            this.rows = rows;
            this.columns = columns;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
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

        #endregion constructor


        #region properties

        public double rows { get; set; } = 1;
        public double columns { get; set; } = 1;
        public double cellWidth { get; set; } = 1;
        public double cellHeight { get; set; } = 1;
        public double offsetX { get; set; } = 0;
        public double offsetY { get; set; } = 0;
        public double paddingX { get; set; } = 0;
        public double paddingY { get; set; } = 0;
        public Brush drawingBrush { get; set; } = Brushes.Transparent;
        public Pen drawingPen { get; set; } = new Pen(Brushes.Black, 1);

        #endregion properties


        #region methods

        public static DrawingGroup CreateGrid(GridModel gridParameters)
        {
            DrawingGroup drawingGroup;
            if (gridParameters.offsetX == 0 && gridParameters.offsetY == 0)
            {
                if (gridParameters.paddingX == 0 && gridParameters.paddingY == 0)
                {
                    drawingGroup = CreateGridDefault(gridParameters);
                }
                else
                {
                    drawingGroup = CreateGridPadding(gridParameters);
                }
            }
            else if (gridParameters.paddingX == 0 && gridParameters.paddingY == 0)
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

            int numTilesX = (int)(gridParameters.rows);
            int numTilesY = (int)(gridParameters.columns);
            int gridWidth = (int)(numTilesX * gridParameters.cellWidth);
            int gridHeight = (int)(numTilesY * gridParameters.cellHeight);
            double borderWidth = numTilesX * gridParameters.cellWidth;
            double borderHeight = gridParameters.columns * gridParameters.cellHeight;

            Rect border = new Rect();
            border.Size = new Size(borderWidth, borderHeight);
            border.Location = new Point(0, 0);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(gridParameters.drawingBrush, gridParameters.drawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(0, gridParameters.cellHeight * index);
                    Point pointStop = new Point(gridWidth, gridParameters.cellHeight * index);
                    context.DrawLine(gridParameters.drawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(gridParameters.cellWidth * index, 0);
                    Point pointStop = new Point(gridParameters.cellWidth * index, gridHeight);
                    context.DrawLine(gridParameters.drawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private static DrawingGroup CreateGridOffset(GridModel gridParameters)
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((gridParameters.rows * gridParameters.cellWidth - gridParameters.offsetX) / gridParameters.cellWidth);
            int numTilesY = (int)((gridParameters.columns * gridParameters.cellHeight - gridParameters.offsetY) / gridParameters.cellHeight);
            double borderWidth = numTilesX * gridParameters.cellWidth;
            double borderHeight = gridParameters.columns * gridParameters.cellHeight;


            Rect border = new Rect();
            border.Size = new Size(borderWidth, borderHeight);
            border.Location = new Point(gridParameters.offsetX, gridParameters.offsetY);
            using (DrawingContext context = gridItems.Open())
            {
                context.DrawRectangle(gridParameters.drawingBrush, gridParameters.drawingPen, border);
                for (int index = 1; index < numTilesY; ++index)
                {
                    Point pointStart = new Point(gridParameters.offsetX, gridParameters.offsetY + gridParameters.cellHeight * index);
                    Point pointStop = new Point(numTilesX * gridParameters.cellWidth + gridParameters.offsetX, gridParameters.offsetY + gridParameters.cellHeight * index);
                    context.DrawLine(gridParameters.drawingPen, pointStart, pointStop);
                }
                for (int index = 1; index < numTilesX; ++index)
                {
                    Point pointStart = new Point(gridParameters.offsetX + gridParameters.cellWidth * index, gridParameters.offsetY);
                    Point pointStop = new Point(gridParameters.offsetX + gridParameters.cellWidth * index, numTilesY * gridParameters.cellHeight + gridParameters.offsetY);
                    context.DrawLine(gridParameters.drawingPen, pointStart, pointStop);
                }
            }
            return gridItems;
        }

        private static DrawingGroup CreateGridPadding(GridModel gridParameters)
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((gridParameters.rows * gridParameters.cellWidth) / (gridParameters.cellWidth + 2 * gridParameters.paddingX));
            int numTilesY = (int)((gridParameters.columns * gridParameters.cellHeight) / (gridParameters.cellHeight + 2 * gridParameters.paddingY));
            Size tileSize = new Size(gridParameters.cellWidth, gridParameters.cellHeight);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = xIndex * gridParameters.cellWidth + (2 * gridParameters.paddingX * xIndex + gridParameters.paddingX);
                        double locationY = yIndex * gridParameters.cellHeight + (2 * gridParameters.paddingY * yIndex + gridParameters.paddingY);
                        Point location = new Point(locationX, locationY);
                        Rect tile = new Rect(location, tileSize);
                        context.DrawRectangle(gridParameters.drawingBrush, gridParameters.drawingPen, tile);
                    }
                }
            }
            return gridItems;
        }

        private static DrawingGroup CreateGridOffsetPadding(GridModel gridParameters)
        {
            DrawingGroup gridItems = new DrawingGroup();

            int numTilesX = (int)((gridParameters.rows * gridParameters.cellWidth - gridParameters.offsetX) / (gridParameters.cellWidth + 2 * gridParameters.paddingX));
            int numTilesY = (int)((gridParameters.columns * gridParameters.cellHeight - gridParameters.offsetY) / (gridParameters.cellHeight + 2 * gridParameters.paddingY));
            Size tileSize = new Size(gridParameters.cellWidth, gridParameters.cellHeight);

            using (DrawingContext context = gridItems.Open())
            {
                for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
                {
                    for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                    {
                        double locationX = gridParameters.offsetX + xIndex * gridParameters.cellWidth + (2 * gridParameters.paddingX * xIndex + gridParameters.paddingX);
                        double locationY = gridParameters.offsetY + yIndex * gridParameters.cellHeight + (2 * gridParameters.paddingY * yIndex + gridParameters.paddingY);
                        Point location = new Point(locationX, locationY);
                        Rect tile = new Rect(location, tileSize);
                        context.DrawRectangle(gridParameters.drawingBrush, gridParameters.drawingPen, tile);
                    }
                }
            }
            return gridItems;
        }

        #endregion methods
    }
}
