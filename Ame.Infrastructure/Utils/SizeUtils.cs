using System.Windows;

namespace Ame.Infrastructure.Utils
{
    public static class SizeUtils
    {
        public static System.Drawing.Size WindowsToDrawingPoint(Size size)
        {
            return new System.Drawing.Size((int)size.Width, (int)size.Height);
        }

        public static Size DrawingToWindowPoint(System.Drawing.Size size)
        {
            return new Size(size.Width, size.Height);
        }
    }
}
