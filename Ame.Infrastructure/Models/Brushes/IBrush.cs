using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models.Brushes
{
    public interface IBrush
    {
        void Draw(DrawingImage drawingImage, Point point);
        void Cancel(DrawingImage drawingImage, Point point);
    }
}
