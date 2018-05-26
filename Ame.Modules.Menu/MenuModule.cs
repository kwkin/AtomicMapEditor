﻿using Ame.Infrastructure.Core;
using Ame.Modules.Menu.Options;
using Ame.Modules.Menu.Ribbon;
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
            this.regionViewRegistry.RegisterViewWithRegion(RegionNames.RibbonRegion, typeof(MapEditorRibbon));
        }
    }
}