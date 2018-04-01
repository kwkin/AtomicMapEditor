using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Modules.Docks.ClipboardDock;
using Ame.Modules.Docks.ItemEditorDock;
using Ame.Modules.Docks.ItemListDock;
using Ame.Modules.Docks.LayerListDock;
using Ame.Modules.Docks.MinimapDock;
using Ame.Modules.Docks.SelectedBrushDock;
using Ame.Modules.Docks.ToolboxDock;
using Ame.Modules.MapEditor.Editor;
using AtomicMapEditor.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace AtomicMapEditor.Modules.Docks.Core
{
    public static class DockSelector
    {
        public static DockViewModelTemplate GetViewModel(DockType type, IUnityContainer container)
        {
            Type viewModelType = null;            
            switch (type)
            {
                case DockType.Clipboard:
                    viewModelType = typeof(ClipboardViewModel);
                    break;

                case DockType.ItemEditor:
                    viewModelType = typeof(ItemEditorViewModel);
                    break;

                case DockType.ItemList:
                    viewModelType = typeof(ItemListViewModel);
                    break;

                case DockType.LayerList:
                    viewModelType = typeof(LayerListViewModel);
                    break;

                case DockType.Minimap:
                    viewModelType = typeof(MinimapViewModel);
                    break;

                case DockType.SelectedBrush:
                    viewModelType = typeof(SelectedBrushViewModel);
                    break;

                case DockType.Toolbox:
                    viewModelType = typeof(ToolboxViewModel);
                    break;

                case DockType.MapEditor:
                    viewModelType = typeof(MainEditorViewModel);
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown DockType");
            }
            DockViewModelTemplate viewModel = container.Resolve(viewModelType, null) as DockViewModelTemplate;
            return viewModel;
        }
    }
}
