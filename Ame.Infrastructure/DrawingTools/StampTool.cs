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
                Stack<ImageDrawing> tiles = new Stack<ImageDrawing>();

                foreach (ImageDrawing drawing in this.Brush.Tiles)
                {
                    ImageDrawing adjustedDrawing = new ImageDrawing();
                    adjustedDrawing.ImageSource = drawing.ImageSource;
                    Point adjustedPoint = new Point(pixelPosition.X + drawing.Rect.X, pixelPosition.Y + drawing.Rect.Y);
                    Rect adjustedRect = new Rect(adjustedPoint, this.Brush.TileSize);
                    adjustedDrawing.Rect = adjustedRect;
                    tiles.Push(adjustedDrawing);
                }
                DrawAction action = new DrawAction(this.ToolName, tiles);
                map.Draw(action);
            }
        }
        
        public void Erase(Map map, Point pixelPosition)
        {
            Stack<ImageDrawing> tiles = new Stack<ImageDrawing>();

            Rect rect = new Rect(pixelPosition, new Size(32, 32));
            ImageDrawing emptyTile = new ImageDrawing(new DrawingImage(), rect);
            tiles.Push(emptyTile);

            DrawAction action = new DrawAction(this.ToolName, tiles);
            map.Draw(action);
        }

        #endregion methods
    }
}
