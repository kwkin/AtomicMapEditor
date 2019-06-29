using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Core;
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
            this.AreaBrush = Brushes.Transparent;
            this.AreaPen = new Pen(Brushes.Yellow, 1);
        }

        public StampTool(BrushModel brush, CoordinateTransform transform)
        {
            this.Brush = brush;
            this.Transform = transform;

            this.AreaBrush = Brushes.Transparent;
            this.AreaPen = new Pen(Brushes.Yellow, 1);
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Stamp";
        public BrushModel Brush { get; set; }
        public CoordinateTransform Transform { get; set; }
        public bool IsErasing { get; set; }

        public Brush AreaBrush { get; set; }
        public Pen AreaPen { get; set; }

        #endregion properties


        #region methods

        public void DrawPressed(Map map, Point pixelPosition)
        {
            this.pressPoint = pixelPosition;
            this.isDrawing = true;
        }

        public void DrawReleased(Map map, Point pixelPosition)
        {
            Stack<Tile> tiles = DrawTiles(pixelPosition);
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
        public void DrawHoverSample(DrawingGroup drawingArea, Rect drawingBounds, double zoom, Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();
            if (!this.isDrawing)
            {
                if (!this.IsErasing)
                {
                    tiles = DrawBrushModel(pixelPosition);
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
                tiles = DrawTiles(pixelPosition);
            }

            using(DrawingContext context = drawingArea.Open())
            {
                if (this.isDrawing)
                {
                    double thickness = 4 / zoom;
                    this.AreaPen.Thickness = thickness > Global.maxGridThickness ? thickness : Global.maxGridThickness;
                    Console.WriteLine(this.AreaPen.Thickness);
                    Point updatedPosition = pixelPosition + new Vector(this.Brush.TileWidth.Value, this.Brush.TileHeight.Value);
                    Rect rect = new Rect(this.pressPoint, updatedPosition);
                    context.DrawRectangle(this.AreaBrush, this.AreaPen, rect);
                }

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

        private Stack<Tile> DrawTiles(Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();

            Point topLeft = GeometryUtils.TopLeftPoint(this.pressPoint, pixelPosition);
            Point bottomRight = GeometryUtils.BottomRightPoint(this.pressPoint, pixelPosition);
            Point startTile = this.Transform.pixelToTile.Transform(topLeft);
            Point stopTile = this.Transform.pixelToTile.Transform(bottomRight);
            Vector tileDifference = startTile - stopTile;
            int horizontalCount = Math.Abs((int)tileDifference.X) + 1;
            int verticalCount = Math.Abs((int)tileDifference.Y) + 1;
            Vector tileOffset = startTile - this.Transform.pixelToTile.Transform(this.pressPoint);
            
            for (int hIndex = 0; hIndex < horizontalCount; ++hIndex)
            {
                for (int vIndex = 0; vIndex < verticalCount; ++vIndex)
                {
                    Point affectedPoint = startTile + new Vector(hIndex, vIndex);
                    Point affectedPixelPoint = this.Transform.pixelToTile.Inverse.Transform(affectedPoint);
                    Point adjustedPoint = new Point(affectedPixelPoint.X, affectedPixelPoint.Y);
                    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.GetTileSize());

                    Tile drawing;
                    int hTile = (int)(hIndex - tileOffset.X) % this.Brush.Columns.Value;
                    int vTile = (int)(vIndex - tileOffset.Y) % this.Brush.Rows.Value;
                    int tileIndex = vTile * this.Brush.Columns.Value + hTile;
                    drawing = this.Brush.Tiles[tileIndex];
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    adjustedDrawing.ImageSource = drawing.Image.Value.ImageSource;
                    adjustedDrawing.Rect = adjustedRect;

                    Tile adjustedTile = new Tile(adjustedDrawing, drawing.TilesetID, drawing.TileID);
                    tiles.Push(adjustedTile);
                }
            }
            return tiles;
        }

        private Stack<Tile> DrawBrushModel(Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();
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
            return tiles;
        }

        #endregion methods
    }
}
