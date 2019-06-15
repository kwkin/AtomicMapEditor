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
                    int hTile = hIndex % this.Brush.Columns();
                    int vTile = vIndex % this.Brush.Rows();
                    int tileIndex = vTile * this.Brush.Columns() + hTile;
                    Tile drawing = this.Brush.Tiles[tileIndex];

                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    adjustedDrawing.ImageSource = drawing.Image.ImageSource;
                    // TODO Check if Rect can be replaced with bounds
                    Point affectedPoint = startTile + new Vector(hIndex, vIndex);
                    Point affectedPixelPoint = this.Transform.pixelToTile.Inverse.Transform(affectedPoint);

                    Point adjustedPoint = new Point(affectedPixelPoint.X, affectedPixelPoint.Y);
                    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);
                    adjustedDrawing.Rect = adjustedRect;

                    Tile adjustedTile = new Tile(adjustedDrawing, drawing.TilesetID, drawing.TileID);
                    tiles.Push(adjustedTile);
                }
            }

            //foreach (Tile drawing in this.Brush.Tiles)
            //{
            //    ImageDrawing adjustedDrawing = new ImageDrawing();
            //    adjustedDrawing.ImageSource = drawing.Image.ImageSource;
            //    // Check if Rect can be replaced with bounds
            //    Point adjustedPoint = new Point(pixelPosition.X + drawing.Image.Rect.X, pixelPosition.Y + drawing.Image.Rect.Y);
            //    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);
            //    adjustedDrawing.Rect = adjustedRect;

            //    Tile adjustedTile = new Tile(adjustedDrawing, drawing.TilesetID, drawing.TileID);
            //    tiles.Push(adjustedTile);
            //}
            DrawAction action = new DrawAction(this.ToolName, tiles);
            map.Draw(action);
        }

        public void Erase(Map map, Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();
            
            Tile emptyTile = Tile.emptyTile(pixelPosition);
            tiles.Push(emptyTile);

            DrawAction action = new DrawAction(this.ToolName, tiles);
            map.Draw(action);
        }

        public void DrawHoverSample(DrawingGroup drawingArea, Point pixelPosition, Rect boundaries)
        {
            if (this.IsErasing)
            {
                return;
            }
            using (DrawingContext context = drawingArea.Open())
            {
                foreach (Tile drawing in this.Brush.Tiles)
                {
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    adjustedDrawing.ImageSource = drawing.Image.ImageSource;
                    // Check if Rect can be replaced with bounds
                    Point adjustedPoint = new Point(pixelPosition.X + drawing.Image.Rect.X, pixelPosition.Y + drawing.Image.Rect.Y);
                    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);
                    adjustedDrawing.Rect = adjustedRect;

                    if (adjustedDrawing.Bounds.X < boundaries.X
                        || adjustedDrawing.Bounds.Y < boundaries.Y
                        || adjustedDrawing.Bounds.X >= boundaries.Width
                        || adjustedDrawing.Bounds.Y >= boundaries.Height)
                    {
                        continue;
                    }
                    context.DrawDrawing(adjustedDrawing);
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
