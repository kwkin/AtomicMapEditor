using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Models.DrawingBrushes;

namespace Ame.Infrastructure.Models.DrawingBrushes
{
    public class BrushCommand : IBrushCommand
    {
        #region fields

        #endregion fields


        #region constructor

        public BrushCommand(DrawingGroup drawingGroup, IBrush brush, Point point)
        {
            this.DrawingGroup = drawingGroup;
            this.Brush = brush;
            this.Point = point;
        }

        #endregion constructor


        #region properties

        // TODO rename
        private DrawingGroup DrawingGroup { get; set; }
        private IBrush Brush { get; set; }
        private Point Point { get; set; }

        #endregion properties


        #region methods

        public void Execute()
        {
            this.Brush.Draw(this.DrawingGroup, this.Point);
        }

        public void UnExecute()
        {
            this.Brush.Draw(this.DrawingGroup, this.Point);
        }

        #endregion methods
    }
}
