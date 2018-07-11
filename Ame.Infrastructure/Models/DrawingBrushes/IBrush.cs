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
    public interface IBrush
    {
        void Draw(DrawingGroup drawingImage, Point point);
        void Cancel(DrawingGroup drawingImage, Point point);
    }
}
