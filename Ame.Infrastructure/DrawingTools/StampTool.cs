using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;

namespace Ame.Infrastructure.DrawingTools
{
    public class StampTool : IDrawingTool, IEraserTool
    {
        #region fields

        private bool isDrawing;
        private Point pressPoint;

        #endregion fields


        #region constructor

        public StampTool()
        {

        }

        public StampTool(BrushModel brush, CoordinateTransform transform)
        {
            this.Brush = brush;
            this.Transform = transform;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Stamp";
        public BrushModel Brush { get; set; }
        public CoordinateTransform Transform { get; set; }
        public bool IsErasing { get; set; }

        #endregion properties


        #region methods

        public void DrawPressed(Map map, Point pixelPosition)
        {
            // TODO add erase function
            this.pressPoint = pixelPosition;
            this.isDrawing = true;
        }

        public void DrawReleased(Map map, Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();

            Point startTile = this.Transform.pixelToTile.Transform(this.pressPoint);
            Point stopTile = this.Transform.pixelToTile.Transform(pixelPosition);
            Vector tileDifference = startTile - stopTile;
            int horizontalCount = Math.Abs((int)tileDifference.X) + 1;
            int verticalCount = Math.Abs((int)tileDifference.Y) + 1;

            for (int hIndex = 0; hIndex < horizontalCount; ++hIndex)
            {
                for (int vIndex = 0; vIndex < verticalCount; ++vIndex)
                {
                    Point affectedPoint = startTile + new Vector(hIndex, vIndex);
                    Point affectedPixelPoint = this.Transform.pixelToTile.Inverse.Transform(affectedPoint);
                    Point adjustedPoint = new Point(affectedPixelPoint.X, affectedPixelPoint.Y);
                    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);

                    Tile drawing;
                    if (!this.IsErasing)
                    {
                        int hTile = hIndex % this.Brush.Columns();
                        int vTile = vIndex % this.Brush.Rows();
                        int tileIndex = vTile * this.Brush.Columns() + hTile;
                        drawing = this.Brush.Tiles[tileIndex];
                    }
                    else
                    {
                        drawing = Tile.emptyTile(affectedPixelPoint);
                    }
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    adjustedDrawing.ImageSource = drawing.Image.ImageSource;
                    adjustedDrawing.Rect = adjustedRect;

                    Tile adjustedTile = new Tile(adjustedDrawing, drawing.TilesetID, drawing.TileID);
                    tiles.Push(adjustedTile);
                }
            }
            DrawAction action = new DrawAction(this.ToolName, tiles);
            map.Draw(action);
            this.isDrawing = false;
        }

        // TODO use
        public void Erase(Map map, Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();
            
            Tile emptyTile = Tile.emptyTile(pixelPosition);
            tiles.Push(emptyTile);

            DrawAction action = new DrawAction(this.ToolName, tiles);
            map.Draw(action);
        }

        public void DrawHoverSample(DrawingGroup drawingArea, Point pixelPosition)
        {
            if (!this.isDrawing)
            {
                using (DrawingContext context = drawingArea.Open())
                {
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    if (!this.IsErasing)
                    {
                        Tile drawing = this.Brush.Tiles[0];
                        adjustedDrawing.ImageSource = drawing.Image.ImageSource;
                        Point adjustedPoint = new Point(pixelPosition.X, pixelPosition.Y);
                        Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);
                        adjustedDrawing.Rect = adjustedRect;
                    }
                    else
                    {
                        // TODO fix
                        Tile empty = Tile.emptyTile(pixelPosition);
                        adjustedDrawing.ImageSource = empty.Image.ImageSource;
                    }
                    context.DrawDrawing(adjustedDrawing);
                }
            }
            else
            {
                Stack<Tile> tiles = new Stack<Tile>();

                Point startTile = this.Transform.pixelToTile.Transform(this.pressPoint);
                Point stopTile = this.Transform.pixelToTile.Transform(pixelPosition);
                Vector tileDifference = startTile - stopTile;
                int horizontalCount = Math.Abs((int)tileDifference.X) + 1;
                int verticalCount = Math.Abs((int)tileDifference.Y) + 1;

                using (DrawingContext context = drawingArea.Open())
                {
                    for (int hIndex = 0; hIndex < horizontalCount; ++hIndex)
                    {
                        for (int vIndex = 0; vIndex < verticalCount; ++vIndex)
                        {
                            Point affectedPoint = startTile + new Vector(hIndex, vIndex);
                            Point affectedPixelPoint = this.Transform.pixelToTile.Inverse.Transform(affectedPoint);

                            Point adjustedPoint = new Point(affectedPixelPoint.X, affectedPixelPoint.Y);
                            Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);

                            Tile drawing;
                            if (!this.IsErasing)
                            {
                                int hTile = hIndex % this.Brush.Columns();
                                int vTile = vIndex % this.Brush.Rows();
                                int tileIndex = vTile * this.Brush.Columns() + hTile;
                                drawing = this.Brush.Tiles[tileIndex];
                            }
                            else
                            {
                                drawing = Tile.emptyTile(affectedPixelPoint);
                            }
                            ImageDrawing adjustedDrawing = new ImageDrawing();
                            adjustedDrawing.ImageSource = drawing.Image.ImageSource;
                            adjustedDrawing.Rect = adjustedRect;

                            context.DrawDrawing(adjustedDrawing);
                        }
                    }
                }
            }
        }

        public bool HasHoverSample()
        {
            return true;
        }

        #endregion methods
    }
}
