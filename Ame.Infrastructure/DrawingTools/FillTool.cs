﻿using System;
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
    public class FillTool : IDrawingTool
    {
        #region fields

        #endregion fields


        #region constructor

        public FillTool()
        {

        }

        public FillTool(BrushModel brush)
        {
            this.Brush = brush;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Fill";
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

        public void DrawHoverSample(Map map, Point pixelPosition)
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
