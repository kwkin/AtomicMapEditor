using System;
using Ame.Infrastructure.Core;
using Microsoft.Practices.Unity;
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
