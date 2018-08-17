﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.DrawingTools
{
    public class EraserTool : IDrawingTool
    {
        #region fields

        #endregion fields


        #region constructor

        public EraserTool()
        {

        }

        public EraserTool(BrushModel brush)
        {
            this.Brush = brush;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Eraser";
        public BrushModel Brush { get; set; }
        public int radius { get; set; } = 5;

        #endregion properties


        #region methods

        public void Apply(Map map, Point pixelPosition)
        {
        }

        public void DrawHoverSample(DrawingGroup drawingArea, Point pixelPosition, Rect boundaries)
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
