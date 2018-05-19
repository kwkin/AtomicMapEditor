using System.Windows;
using System.Windows.Controls;
using Ame.Modules.Windows.Docks.ClipboardDock;
using Ame.Modules.Windows.Docks.ItemEditorDock;
using Ame.Modules.Windows.Docks.ItemListDock;
using Ame.Modules.Windows.Docks.LayerListDock;
using Ame.Modules.Windows.Docks.MinimapDock;
using Ame.Modules.Windows.Docks.SelectedBrushDock;
using Ame.Modules.Windows.Docks.SessionViewerDock;
using Ame.Modules.Windows.Docks.ToolboxDock;
using Ame.Modules.MapEditor.Editor;
using Ame.Modules.Windows.Docks.ProjectExplorerDock;

namespace Ame.Modules.Windows.Docks
{
    internal class DockTemplateSelector : DataTemplateSelector
    {
        #region fields

        #endregion fields


        #region constructor

        public DockTemplateSelector()
        {
        }

        #endregion constructor


        #region properties

        public DataTemplate ClipboardDataTemplate { get; set; }
        public DataTemplate ItemEditorDataTemplate { get; set; }
        public DataTemplate ItemListDataTemplate { get; set; }
        public DataTemplate LayerListDataTemplate { get; set; }
        public DataTemplate MinimapDataTemplate { get; set; }
        public DataTemplate ToolboxDataTemplate { get; set; }
        public DataTemplate ProjectExplorerDataTemplate { get; set; }
        public DataTemplate SelectedBrushDataTemplate { get; set; }
        public DataTemplate SessionViewDataTemplate { get; set; }

        public DataTemplate MapEditorTemplate { get; set; }

        #endregion properties


        #region methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = new DataTemplate(typeof(UserControl));
            if (item is ClipboardViewModel)
            {
                return ClipboardDataTemplate;
            }
            else if (item is ItemEditorViewModel)
            {
                return ItemEditorDataTemplate;
            }
            else if (item is ItemListViewModel)
            {
                return ItemListDataTemplate;
            }
            else if (item is LayerListViewModel)
            {
                return LayerListDataTemplate;
            }
            else if (item is MinimapViewModel)
            {
                return MinimapDataTemplate;
            }
            else if (item is ToolboxViewModel)
            {
                return ToolboxDataTemplate;
            }
            else if (item is SelectedBrushViewModel)
            {
                return SelectedBrushDataTemplate;
            }
            else if (item is ProjectExplorerViewModel)
            {
                return ProjectExplorerDataTemplate;
            }
            else if (item is SessionViewerViewModel)
            {
                return SessionViewDataTemplate;
            }
            else if (item is MapEditorViewModel)
            {
                return MapEditorTemplate;
            }
            return base.SelectTemplate(item, container);
        }

        #endregion methods
    }
}
