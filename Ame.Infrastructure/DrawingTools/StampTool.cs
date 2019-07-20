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
        private double maxGridThickness;

        #endregion fields


        #region constructor

        public StampTool()
        {
            this.AreaBrush = Brushes.Transparent;
            this.AreaPen = new Pen(Brushes.Yellow, 1);

            this.maxGridThickness = 0.5;
        }

        public StampTool(BrushModel brush, CoordinateTransform transform)
        {
            this.Brush = brush;
            this.Transform = transform;

            this.AreaBrush = Brushes.Transparent;
            this.AreaPen = new Pen(Brushes.Yellow, 1);

            this.maxGridThickness = 0.5;
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
            if (!this.isDrawing)
            {
                this.pressPoint = pixelPosition;
            }
            else
            {
                Point topLeft = GeometryUtils.TopLeftPoint(this.pressPoint, pixelPosition);
                Point bottomRight = GeometryUtils.BottomRightPoint(this.pressPoint, pixelPosition);
                Stack<Tile> tiles = DrawTiles(topLeft, bottomRight);
                DrawAction action = new DrawAction(this.ToolName, tiles);
                map.Draw(action);
            }
            this.isDrawing = !this.isDrawing;
        }

        public void DrawReleased(Map map, Point pixelPosition)
        {
            if (!ImageUtils.Intersects(map.CurrentLayer.Group, pixelPosition))
            {
                pixelPosition = ImageUtils.BindPoint(map.CurrentLayer.Group, pixelPosition);
            }
            Point topLeft = GeometryUtils.TopLeftPoint(this.pressPoint, pixelPosition);
            Point bottomRight = GeometryUtils.BottomRightPoint(this.pressPoint, pixelPosition);
            Stack<Tile> tiles = DrawTiles(topLeft, bottomRight);
            DrawAction action = new DrawAction(this.ToolName, tiles);
            map.Draw(action);
            this.isDrawing = false;
        }

        public void DrawHoverSample(Map map, DrawingGroup drawingArea, double zoom, Point pixelPosition)
        {
            bool isIntersecting = ImageUtils.Intersects(map.CurrentLayer.Group, pixelPosition);
            if (!isIntersecting && !this.isDrawing)
            {
                return;
            }
            Point topLeft = GeometryUtils.TopLeftPoint(this.pressPoint, pixelPosition);
            Point bottomRight = GeometryUtils.BottomRightPoint(this.pressPoint, pixelPosition);
            topLeft = ImageUtils.BindPoint(map.CurrentLayer.Group, topLeft);
            bottomRight = ImageUtils.BindPoint(map.CurrentLayer.Group, bottomRight);

            Stack<Tile> tiles;
            if (!this.isDrawing)
            {
                tiles = DrawBrushModel(pixelPosition);
            }
            else
            {
                tiles = DrawTiles(topLeft, bottomRight);
            }

            if (this.IsErasing)
            {
                DrawAction action = new DrawAction(this.ToolName, tiles);
                tiles = map.DrawSample(action).Tiles;
            }

            using (DrawingContext context = drawingArea.Open())
            {
                if (this.isDrawing)
                {
                    if (isIntersecting)
                    {
                        bottomRight += new Vector(this.Brush.TileWidth.Value, this.Brush.TileHeight.Value);
                    }

                    double thickness = 4 / zoom;
                    this.AreaPen.Thickness = thickness > this.maxGridThickness ? thickness : this.maxGridThickness;
                    Rect rect = new Rect(topLeft, bottomRight);
                    context.DrawRectangle(this.AreaBrush, this.AreaPen, rect);
                }
                foreach (Tile tile in tiles)
                {
                    if (tile.Bounds.IntersectsWith(map.CurrentLayer.GetBoundsExclusive()))
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

        private Stack<Tile> DrawTiles(Point topLeft, Point bottomRight)
        {
            Stack<Tile> tiles = new Stack<Tile>();

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

                    Tile tile;
                    if (!this.IsErasing)
                    {
                        int hTile = Utils.Utils.Mod((int)(hIndex + tileOffset.X), this.Brush.Columns.Value);
                        int vTile = Utils.Utils.Mod((int)(vIndex + tileOffset.Y), this.Brush.Rows.Value);
                        int tileIndex = vTile * this.Brush.Columns.Value + hTile;
                        tile = this.Brush.Tiles[tileIndex];
                        ImageDrawing adjustedDrawing = new ImageDrawing();
                        adjustedDrawing.ImageSource = tile.Image.Value.ImageSource;
                        adjustedDrawing.Rect = adjustedRect;
                        tile = new Tile(adjustedDrawing, tile.TilesetID, tile.TileID);
                    }
                    else
                    {
                        tile = Tile.EmptyTile(adjustedPoint);
                    }
                    tiles.Push(tile);
                }
            }
            return tiles;
        }

        private Stack<Tile> DrawBrushModel(Point pixelPosition)
        {
            Stack<Tile> tiles = new Stack<Tile>();
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
                Tile emptyTile = Tile.EmptyTile(pixelPosition);
                tiles.Push(emptyTile);
            }
            return tiles;
        }

        #endregion methods
    }
}
