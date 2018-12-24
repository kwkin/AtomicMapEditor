using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.DrawingTools
{
    public class StampTool : IDrawingTool, IEraserTool
    {
        #region fields

        #endregion fields


        #region constructor

        public StampTool()
        {

        }

        public StampTool(BrushModel brush)
        {
            this.Brush = brush;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Stamp";
        public BrushModel Brush { get; set; }
        public bool IsErasing { get; set; }

        #endregion properties


        #region methods

        public void Apply(Map map, Point pixelPosition)
        {
            if (this.IsErasing)
            {
                Erase(map, pixelPosition);
            }
            else
            {
                Stack<Tile> tiles = new Stack<Tile>();

                foreach (Tile drawing in this.Brush.Tiles)
                {
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    adjustedDrawing.ImageSource = drawing.Image.ImageSource;
                    // Check if Rect can be replaced with bounds
                    Point adjustedPoint = new Point(pixelPosition.X + drawing.Image.Rect.X, pixelPosition.Y + drawing.Image.Rect.Y);
                    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);
                    adjustedDrawing.Rect = adjustedRect;

                    Tile adjustedTile = new Tile(adjustedDrawing, drawing.TilesetID, drawing.TileID);
                    tiles.Push(adjustedTile);
                }
                DrawAction action = new DrawAction(this.ToolName, tiles);
                map.Draw(action);
            }
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
