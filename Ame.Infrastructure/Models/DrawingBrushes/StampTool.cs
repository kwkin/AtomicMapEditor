using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models.DrawingBrushes
{
    public class StampTool : IDrawingTool
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
        public int radius { get; set; } = 5;

        #endregion properties


        #region methods

        public void Apply(Map map, Point pixelPosition)
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
            BrushAction action = new BrushAction(this.ToolName, tiles);
            map.Draw(action);
        }



        #endregion methods
    }
}
