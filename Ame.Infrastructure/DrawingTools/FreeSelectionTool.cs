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
    public class FreeSelectionTool : IDrawingTool, ISelectionTool
    {
        #region fields

        #endregion fields


        #region constructor

        public FreeSelectionTool()
        {

        }

        public FreeSelectionTool(BrushModel brush)
        {
            this.Brush = brush;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Free Selection";
        public BrushModel Brush { get; set; }
        public CoordinateTransform Transform { get; set; }
        public int radius { get; set; } = 5;

        #endregion properties


        #region methods

        public void DrawPressed(Map map, Point pixelPosition)
        {
        }

        public void DrawReleased(Map map, Point pixelPosition)
        {
            return;
        }

        public void DeleteSelected()
        {
            throw new NotImplementedException();
        }

        public void CopySelected()
        {
            throw new NotImplementedException();
        }

        public void DrawHoverSample(Map map, DrawingGroup drawingArea, Rect drawingBounds, double zoom, Point pixelPosition)
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
