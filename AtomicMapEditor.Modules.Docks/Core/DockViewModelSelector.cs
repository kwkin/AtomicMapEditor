using System;
using System.ComponentModel;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Modules.Docks.ClipboardDock;
using Ame.Modules.Docks.ItemEditorDock;
using Ame.Modules.Docks.ItemListDock;
using Ame.Modules.Docks.LayerListDock;
using Ame.Modules.Docks.MinimapDock;
using Ame.Modules.Docks.SelectedBrushDock;
using Ame.Modules.Docks.ToolboxDock;
using Ame.Modules.MapEditor.Editor;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Docks.Core
{
    public static class DockViewModelSelector
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
                    if (!container.IsRegistered(typeof(Map)))
                    {
                        container.RegisterInstance<Map>(new Map("Map #1"));
                    }
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
