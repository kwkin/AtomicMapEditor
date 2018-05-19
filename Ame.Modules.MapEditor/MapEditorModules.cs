using Ame.Infrastructure.Core;
using Ame.Modules.MapEditor.Editor;
using Prism.Modularity;
using Prism.Regions;

namespace Ame.Modules.MapEditor
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
            this.regionViewRegistry.RegisterViewWithRegion(RegionNames.EditorRegion, typeof(MapEditorDocument));
        }
    }
}
