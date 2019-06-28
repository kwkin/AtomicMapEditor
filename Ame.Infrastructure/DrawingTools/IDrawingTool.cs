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
    public interface IDrawingTool
    {
        #region properties

        string ToolName { get; set; }
        BrushModel Brush { get; set; }
        CoordinateTransform Transform { get; set; }

        #endregion properties


        #region methods

        void DrawPressed(Map map, Point pixelPosition);
        void DrawReleased(Map map, Point pixelPosition);
        void DrawHoverSample(DrawingGroup drawingArea, Rect drawingBounds, Point pixelPosition);
        bool HasHoverSample();

        #endregion methods
    }
}
