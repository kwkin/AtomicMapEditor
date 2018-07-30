using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.DrawingTools
{
    public class TileCollisionTool : IDrawingTool
    {
        #region fields

        #endregion fields


        #region constructor

        public TileCollisionTool()
        {

        }

        public TileCollisionTool(BrushModel brush)
        {
            this.Brush = brush;
        }

        #endregion constructor


        #region properties

        public string ToolName { get; set; } = "Tile Collision";
        public BrushModel Brush { get; set; }
        public int radius { get; set; } = 5;

        #endregion properties


        #region methods

        public void Apply(Map map, Point pixelPosition)
        {
        }

        #endregion methods
    }
}
