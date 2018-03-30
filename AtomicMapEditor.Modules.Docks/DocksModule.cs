using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtomicMapEditor.Infrastructure.Core;
using AtomicMapEditor.Modules.Docks.Core;
using Prism.Modularity;
using Prism.Regions;

namespace AtomicMapEditor.Modules.Docks
{
    public class DocksModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry = null;

        public DocksModule(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }

        public void Initialize()
        {
            this.regionViewRegistry.RegisterViewWithRegion(RegionNames.DockRegion, typeof(DockManager));
        }
    }
}
