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
    public class BrushTool : IDrawingTool
    {
        #region fields

        #endregion fields


        #region constructor

        public BrushTool()
        {
        }

        public BrushTool(BrushModel brush)
        {
            this.Brush = brush;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Brush";
        public BrushModel Brush { get; set; }
        public CoordinateTransform Transform { get; set; }
        public int radius { get; set; } = 5;

        #endregion properties


        #region methods

        public void DrawPressed(Map map, Point pixelPosition)
        {
            //Stack<ImageDrawing> tiles = new Stack<ImageDrawing>();
            //int x0 = (int)pixelPosition.X;
            //int y0 = (int)pixelPosition.Y;
            //int x = this.radius;
            //int y = 0;
            //int err = 0;
            //Size tileSize = new Size(32, 32);
            //while (x >= y)
            //{
            //    Point point1 = new Point(x0 + x * 32, y0 + y * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point1, tileSize)));
            //    Point point2 = new Point(x0 + y * 32, y0 + x * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point2, tileSize)));
            //    Point point3 = new Point(x0 - y * 32, y0 + x * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point3, tileSize)));
            //    Point point4 = new Point(x0 - x * 32, y0 + y * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point4, tileSize)));
            //    Point point5 = new Point(x0 - x * 32, y0 - y * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point5, tileSize)));
            //    Point point6 = new Point(x0 - y * 32, y0 - x * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point6, tileSize)));
            //    Point point7 = new Point(x0 + y * 32, y0 - x * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point7, tileSize)));
            //    Point point8 = new Point(x0 + x * 32, y0 - y * 32);
            //    tiles.Push(new ImageDrawing(this.Brush.Image, new Rect(point8, tileSize)));

            //    y += 1;
            //    err += 1 + 2 * y;
            //    if (2 * (err - x) + 1 > 0)
            //    {
            //        x -= 1;
            //        err += 1 - 2 * x;
            //    }
            //}
            //Rect rect = new Rect(pixelPosition, this.Brush.TileSize);
            //BrushAction action = new BrushAction(this.ToolName, tiles);
            //map.Draw(action);
        }

        public void DrawReleased(Map map, Point pixelPosition)
        {
            return;
        }

        public void DrawHoverSample(DrawingGroup drawingArea, Rect drawingBounds, Point pixelPosition)
        {
            return;
        }

        public bool HasHoverSample()
        {
            return false;
        }

        #endregion methods
    }
}
