using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Core;
using Ame.Modules.Docks.Core;
using Prism.Modularity;
using Prism.Regions;

namespace Ame.Modules.Docks
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
