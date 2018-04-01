using Ame.Infrastructure.Core;
using Ame.Modules.Menu.Options;
using Prism.Modularity;
using Prism.Regions;

namespace Ame.Modules.Menu
{
    public class MenuModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry = null;

        public MenuModule(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }

        public void Initialize()
        {
            this.regionViewRegistry.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(MenuOptions));
        }
    }
}
