using System;
using Ame.Infrastructure.Core;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Ame.Modules.Windows
{
    public class WindowModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry = null;
        
        public WindowModule(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }

        public void Initialize()
        {
            this.regionViewRegistry.RegisterViewWithRegion(RegionNames.DockRegion, typeof(WindowManager));
        }
    }
}
