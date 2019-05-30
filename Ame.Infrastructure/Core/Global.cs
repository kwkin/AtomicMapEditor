using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Core
{
    public static class Global
    {
        public static readonly int version = 1;
        public static readonly string applicationName = "AtomicMapEditor";

        public static string SessionFileName = "Session.config";
        public static string LayoutFileName = "Layout.config";

        public static readonly double maxGridThickness = 0.5;
        public static readonly long defaultUpdatePositionLabelDelay = 30;
        public static readonly long defaultDrawSelectLineDelay = 100;
    }
}
