using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.DrawingTools
{
    public interface IDrawingTool
    {
        #region properties

        string ToolName { get; set; }
        BrushModel Brush { get; set; }

        #endregion properties


        #region methods

        void Apply(Map map, Point pixelPosition);

        #endregion methods
    }
}
