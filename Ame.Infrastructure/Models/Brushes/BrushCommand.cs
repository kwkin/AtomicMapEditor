using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Models.Brushes;

namespace Ame.Infrastructure.Models.Brushes
{
    public class BrushCommand : IBrushCommand
    {
        #region fields

        #endregion fields


        #region constructor

        public BrushCommand(DrawingImage drawingImage, IBrush brush, Point point)
        {
            this.DrawingImage = drawingImage;
            this.Brush = brush;
            this.Point = point;
        }

        #endregion constructor


        #region properties

        private DrawingImage DrawingImage { get; set; }
        private IBrush Brush { get; set; }
        private Point Point { get; set; }

        #endregion properties


        #region methods

        public void Execute()
        {
            this.Brush.Draw(this.DrawingImage, this.Point);
        }

        public void UnExecute()
        {
            this.Brush.Draw(this.DrawingImage, this.Point);
        }

        #endregion methods
    }
}
