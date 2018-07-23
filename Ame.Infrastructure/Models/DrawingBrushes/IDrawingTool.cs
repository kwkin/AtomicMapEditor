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
    public interface IDrawingTool
    {
        #region properties

        string ToolName { get; set; }
        BrushModel Brush { get; set; }

        #endregion


        #region methods

        void Apply(Map map, Point tilePosition);

        #endregion
    }
}
