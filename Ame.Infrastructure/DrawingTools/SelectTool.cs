﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.DrawingTools
{
    public class SelectTool : IDrawingTool, ISelectionTool
    {
        #region fields

        #endregion fields


        #region constructor

        public SelectTool()
        {

        }

        public SelectTool(BrushModel brush)
        {
            this.Brush = brush;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Select";
        public BrushModel Brush { get; set; }
        public int radius { get; set; } = 5;

        #endregion properties


        #region methods

        public void Apply(Map map, Point pixelPosition)
        {
        }

        public void DeleteSelected()
        {
            throw new NotImplementedException();
        }

        public void CopySelected()
        {
            throw new NotImplementedException();
        }

        #endregion methods
    }
}
