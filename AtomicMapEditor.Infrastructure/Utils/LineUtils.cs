using System.Windows;
using System.Windows.Shapes;

namespace AtomicMapEditor.Infrastructure.Utils
{
    public static class LineUtils
    {
        public static Line CreateLine(Point p1, Point p2)
        {
            Line newLine = new Line();
            newLine.X1 = p1.X;
            newLine.Y1 = p1.Y;
            newLine.X2 = p2.X;
            newLine.Y2 = p2.Y;
            return newLine;
        }

        public static Line CreateLine(double p1X, double p1Y, double p2X, double p2Y)
        {
            Line newLine = new Line();
            newLine.X1 = p1X;
            newLine.Y1 = p1Y;
            newLine.X2 = p2X;
            newLine.Y2 = p2Y;
            return newLine;
        }
    }
}
