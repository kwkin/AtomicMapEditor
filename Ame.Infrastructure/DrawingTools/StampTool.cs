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
                    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.GetTileSize());

                    Tile drawing;
                    if (!this.IsErasing)
                    {
                        int hTile = hIndex % this.Brush.Columns.Value;
                        int vTile = vIndex % this.Brush.Rows.Value;
                        int tileIndex = vTile * this.Brush.Columns.Value + hTile;
                        drawing = this.Brush.Tiles[tileIndex];
                    }
                    else
                    {
                        drawing = Tile.emptyTile(affectedPixelPoint);
                    }
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    adjustedDrawing.ImageSource = drawing.Image.Value.ImageSource;
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

        // TODO add opacity to the hover sample
        // TODO ignore immediately when pixel position is out of the map bounds
        public void DrawHoverSample(DrawingGroup drawingArea, Rect drawingBounds, Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();
            if (!this.isDrawing)
            {
                if (!this.IsErasing)
                {
                    int horizontalCount = this.Brush.Columns.Value;
                    int verticalCount = this.Brush.Rows.Value;
                    for (int hIndex = 0; hIndex < horizontalCount; ++hIndex)
                    {
                        for (int vIndex = 0; vIndex < verticalCount; ++vIndex)
                        {
                            Tile drawing;
                            int hTile = hIndex % this.Brush.Columns.Value;
                            int vTile = vIndex % this.Brush.Rows.Value;
                            int tileIndex = vTile * this.Brush.Columns.Value + hTile;
                            drawing = this.Brush.Tiles[tileIndex];

                            Point adjustedPoint = new Point(pixelPosition.X + drawing.Image.Value.Rect.X, pixelPosition.Y + drawing.Image.Value.Rect.Y);
                            Rect adjustedRect = new Rect(adjustedPoint, this.Brush.GetTileSize());
                            
                            ImageDrawing adjustedDrawing = new ImageDrawing();
                            adjustedDrawing.ImageSource = drawing.Image.Value.ImageSource;
                            adjustedDrawing.Rect = adjustedRect;

                            Tile adjustedTile = new Tile(adjustedDrawing, drawing.TilesetID, drawing.TileID);
                            tiles.Push(adjustedTile);
                        }
                    }
                }
                else
                {
                    // TODO implement erasing
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    Tile empty = Tile.emptyTile(pixelPosition);
                    adjustedDrawing.ImageSource = empty.Image.Value.ImageSource;
                }
            }
            else
            {
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
                        Rect adjustedRect = new Rect(adjustedPoint, this.Brush.GetTileSize());

                        Tile drawing;
                        int hTile = hIndex % this.Brush.Columns.Value;
                        int vTile = vIndex % this.Brush.Rows.Value;
                        int tileIndex = vTile * this.Brush.Columns.Value + hTile;
                        drawing = this.Brush.Tiles[tileIndex];
                        ImageDrawing adjustedDrawing = new ImageDrawing();
                        adjustedDrawing.ImageSource = drawing.Image.Value.ImageSource;
                        adjustedDrawing.Rect = adjustedRect;

                        Tile adjustedTile = new Tile(adjustedDrawing, drawing.TilesetID, drawing.TileID);
                        tiles.Push(adjustedTile);
                    }
                }
            }
            using(DrawingContext context = drawingArea.Open())
            {
                foreach (Tile tile in tiles)
                {
                    if (tile.Bounds.IntersectsWith(drawingBounds))
                    {
                        context.DrawDrawing(tile.Image.Value);
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
