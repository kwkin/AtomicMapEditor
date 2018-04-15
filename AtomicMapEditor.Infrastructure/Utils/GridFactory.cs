using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Utils
{

    public static class GridFactory
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        public static Brush Stroke { get; set; } = Brushes.Black;
        public static double StrokeThickness { get; set; } = 1;

        #endregion properties


        #region methods

        public static ObservableCollection<Visual> CreateGrid(GridModel gridParameters)
        {
            ObservableCollection<Visual> gridItems = new ObservableCollection<Visual>();
            if (gridParameters.offsetX == 0 && gridParameters.offsetY == 0)
            {
                if (gridParameters.paddingX == 0 && gridParameters.paddingY == 0)
                {
                    gridItems = CreateGridDefault(gridParameters);
                }
                else
                {
                    gridItems = CreateGridPadding(gridParameters);
                }
            }
            else if (gridParameters.paddingX == 0 && gridParameters.paddingY == 0)
            {
                gridItems = CreateGridOffset(gridParameters);
            }
            else
            {
                gridItems = CreateGridOffsetPadding(gridParameters);
            }
            return gridItems;
        }

        private static ObservableCollection<Visual> CreateGridDefault(GridModel gridParameters)
        {
            ObservableCollection<Visual> CanvasGridItems = new ObservableCollection<Visual>();

            int numTilesX = (int)(gridParameters.rows);
            int numTilesY = (int)(gridParameters.columns);
            int gridWidth = (int)(numTilesX * gridParameters.cellWidth);
            int gridHeight = (int)(numTilesY * gridParameters.cellHeight);

            Path rectPath = new Path();
            Rect tile = new Rect();
            RectangleGeometry tileGeometry = new RectangleGeometry();
            rectPath.Stroke = GridFactory.Stroke;
            rectPath.StrokeThickness = GridFactory.StrokeThickness;
            tile.Size = new Size(numTilesX * gridParameters.cellWidth, gridParameters.columns * gridParameters.cellHeight);
            tile.Location = new Point(0, 0);
            tileGeometry.Rect = tile;
            rectPath.Data = tileGeometry;

            CanvasGridItems.Add(rectPath);

            for (int index = 1; index < numTilesY; ++index)
            {
                Line horizontal = new Line();
                horizontal.Stroke = GridFactory.Stroke;
                horizontal.StrokeThickness = GridFactory.StrokeThickness;
                horizontal.X1 = 0;
                horizontal.Y1 = gridParameters.cellHeight * index;
                horizontal.X2 = gridWidth;
                horizontal.Y2 = gridParameters.cellHeight * index;
                CanvasGridItems.Add(horizontal);
            }

            for (int index = 1; index < numTilesX; ++index)
            {
                Line vertical = new Line();
                vertical.Stroke = GridFactory.Stroke;
                vertical.StrokeThickness = GridFactory.StrokeThickness;
                vertical.X1 = gridParameters.cellWidth * index;
                vertical.Y1 = 0;
                vertical.X2 = gridParameters.cellWidth * index;
                vertical.Y2 = gridHeight;
                CanvasGridItems.Add(vertical);
            }
            return CanvasGridItems;
        }

        private static ObservableCollection<Visual> CreateGridOffset(GridModel gridParameters)
        {
            ObservableCollection<Visual> CanvasGridItems = new ObservableCollection<Visual>();

            int numTilesX = (int)((gridParameters.rows * gridParameters.cellWidth - gridParameters.offsetX) / gridParameters.cellWidth);
            int numTilesY = (int)((gridParameters.columns * gridParameters.cellHeight - gridParameters.offsetY) / gridParameters.cellHeight);

            Path rectPath = new Path();
            Rect tile = new Rect();
            RectangleGeometry tileGeometry = new RectangleGeometry();
            rectPath.Stroke = GridFactory.Stroke;
            rectPath.StrokeThickness = GridFactory.StrokeThickness;
            tile.Size = new Size(numTilesX * gridParameters.cellWidth, numTilesY * gridParameters.cellHeight);
            tile.Location = new Point(gridParameters.offsetX, gridParameters.offsetY);
            tileGeometry.Rect = tile;
            rectPath.Data = tileGeometry;
            CanvasGridItems.Add(rectPath);

            for (int index = 1; index < numTilesY; ++index)
            {
                Line horizontal = new Line();
                horizontal.Stroke = GridFactory.Stroke;
                horizontal.StrokeThickness = GridFactory.StrokeThickness;
                horizontal.X1 = gridParameters.offsetX;
                horizontal.Y1 = gridParameters.offsetY + gridParameters.cellHeight * index;
                horizontal.X2 = numTilesX * gridParameters.cellWidth + gridParameters.offsetX;
                horizontal.Y2 = gridParameters.offsetY + gridParameters.cellHeight * index;
                CanvasGridItems.Add(horizontal);
            }

            for (int index = 1; index < numTilesX; ++index)
            {
                Line vertical = new Line();
                vertical.Stroke = GridFactory.Stroke;
                vertical.StrokeThickness = GridFactory.StrokeThickness;
                vertical.X1 = gridParameters.offsetX + gridParameters.cellWidth * index;
                vertical.Y1 = gridParameters.offsetY;
                vertical.X2 = gridParameters.offsetX + gridParameters.cellWidth * index;
                vertical.Y2 = numTilesY * gridParameters.cellHeight + gridParameters.offsetY;
                CanvasGridItems.Add(vertical);
            }
            return CanvasGridItems;
        }

        private static ObservableCollection<Visual> CreateGridPadding(GridModel gridParameters)
        {
            int numTilesX = (int)((gridParameters.rows * gridParameters.cellWidth) / (gridParameters.cellWidth + 2 * gridParameters.paddingX));
            int numTilesY = (int)((gridParameters.columns * gridParameters.cellHeight) / (gridParameters.cellHeight + 2 * gridParameters.paddingY));

            ObservableCollection<Visual> CanvasGridItems = new ObservableCollection<Visual>();

            Size tileSize = new Size(gridParameters.cellWidth, gridParameters.cellHeight);
            for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
            {
                for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                {
                    Path rectPath = new Path();
                    rectPath.Stroke = GridFactory.Stroke;
                    rectPath.StrokeThickness = GridFactory.StrokeThickness;

                    Rect tile = new Rect();
                    tile.Size = tileSize;
                    Point location = new Point();
                    location.X = xIndex * gridParameters.cellWidth + (2 * gridParameters.paddingX * xIndex + gridParameters.paddingX);
                    location.Y = yIndex * gridParameters.cellHeight + (2 * gridParameters.paddingY * yIndex + gridParameters.paddingY);
                    tile.Location = location;

                    RectangleGeometry tileGeometry = new RectangleGeometry();
                    tileGeometry.Rect = tile;

                    rectPath.Data = tileGeometry;

                    CanvasGridItems.Add(rectPath);
                }
            }
            return CanvasGridItems;
        }

        private static ObservableCollection<Visual> CreateGridOffsetPadding(GridModel gridParameters)
        {
            int numTilesX = (int)((gridParameters.rows * gridParameters.cellWidth - gridParameters.offsetX) / (gridParameters.cellWidth + 2 * gridParameters.paddingX));
            int numTilesY = (int)((gridParameters.columns * gridParameters.cellHeight - gridParameters.offsetY) / (gridParameters.cellHeight + 2 * gridParameters.paddingY));

            ObservableCollection<Visual> CanvasGridItems = new ObservableCollection<Visual>();

            Size tileSize = new Size(gridParameters.cellWidth, gridParameters.cellHeight);
            for (int xIndex = 0; xIndex < numTilesX; ++xIndex)
            {
                for (int yIndex = 0; yIndex < numTilesY; ++yIndex)
                {
                    Path rectPath = new Path();
                    rectPath.Stroke = GridFactory.Stroke;
                    rectPath.StrokeThickness = GridFactory.StrokeThickness;

                    Rect tile = new Rect();
                    tile.Size = tileSize;
                    Point location = new Point();
                    location.X = gridParameters.offsetX + xIndex * gridParameters.cellWidth + (2 * gridParameters.paddingX * xIndex + gridParameters.paddingX);
                    location.Y = gridParameters.offsetY + yIndex * gridParameters.cellHeight + (2 * gridParameters.paddingY * yIndex + gridParameters.paddingY);
                    tile.Location = location;

                    RectangleGeometry tileGeometry = new RectangleGeometry();
                    tileGeometry.Rect = tile;

                    rectPath.Data = tileGeometry;

                    CanvasGridItems.Add(rectPath);
                }
            }
            return CanvasGridItems;
        }

        #endregion methods
    }
}
