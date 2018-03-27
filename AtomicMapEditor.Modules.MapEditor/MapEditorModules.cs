using AtomicMapEditor.Infrastructure.Core;
using AtomicMapEditor.Modules.MapEditor.Editor;
using AtomicMapEditor.Modules.MapEditor.Ribbon;
using Prism.Modularity;
using Prism.Regions;

namespace AtomicMapEditor.Modules.MapEditor
{
    public class MapEditorModules : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry = null;

        public MapEditorModules(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }

        public void Initialize()
        {
            this.regionViewRegistry.RegisterViewWithRegion(RegionNames.EditorRegion, typeof(MainEditor));
            this.regionViewRegistry.RegisterViewWithRegion(RegionNames.RibbonRegion, typeof(MapEditorRibbon));
        }
    }
}
