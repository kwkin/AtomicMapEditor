using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Utils
{
    public static class StopwatchExtension
    {
        public static long TotalTime(this Stopwatch sw, Action action, int iterations)
        {
            //long elepasedTime = s.TotalTime(() => brushModel.Image = ImageUtils.MatToImageDrawing(croppedImage), 1000);
            sw.Reset();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                action();
            }
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }


        public static long AverageTime(this Stopwatch sw, Action action, int iterations)
        {
            sw.Reset();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                action();
            }
            sw.Stop();

            return sw.ElapsedMilliseconds / iterations;
        }
    }
}
