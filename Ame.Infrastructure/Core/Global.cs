using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Core
{
    public static class Global
    {
        public static readonly string version = @"0.0.1";
        public static readonly string applicationName = @"AtomicMapEditor";

        public static string sessionFileName = @"Session.config";
        public static string layoutFileName = @"Layout.config";

        public static readonly double maxGridThickness = 0.5;
        public static readonly long defaultUpdatePositionLabelDelay = 30;
        public static readonly long defaultDrawSelectLineDelay = 100;
    }
}
