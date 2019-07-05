using Ame.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ame.App.Wpf.UILogic.Actions
{
    public class LayerBoundariesRenderable
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerBoundariesRenderable(ILayer layer)
        {
            this.Layer = layer;

            this.DrawingBrush = Brushes.Transparent;
            this.DrawingPen = new Pen(Brushes.Black, 1);
            this.DrawingPenDashed = new Pen(Brushes.Yellow, 1);
            double[] dashPattern = new double[] { 0, 2 };
            this.DrawingPenDashed.DashStyle = new DashStyle(dashPattern, 0);
        }

        #endregion constructor


        #region properties

        public ILayer Layer { get; set; }
        public Brush DrawingBrush { get; set; }
        public Pen DrawingPen { get; set; }
        public Pen DrawingPenDashed { get; set; }

        #endregion properties


        #region methods

        public DrawingGroup CreateBoundaries()
        {
            Brush drawingBrush = Brushes.Transparent;
            Pen drawingPen = new Pen(Brushes.Black, 1);

            Pen drawingPenDashed = new Pen(Brushes.Yellow, 1);
            double[] dashPattern = new double[] { 2, 4 };
            drawingPenDashed.DashStyle = new DashStyle(dashPattern, 0);

            DrawingGroup drawingGroup = new DrawingGroup();
            using (DrawingContext context = drawingGroup.Open())
            {
                Point location = new Point(this.Layer.OffsetX.Value, this.Layer.OffsetY.Value);
                Size size = new Size(this.Layer.PixelWidth.Value, this.Layer.PixelHeight.Value);
                Rect boundaries = new Rect(location, size);
                context.DrawRectangle(drawingBrush, drawingPen, boundaries);
                context.DrawRectangle(drawingBrush, drawingPenDashed, boundaries);
            }
            return drawingGroup;
        }

        #endregion methods
    }
}
