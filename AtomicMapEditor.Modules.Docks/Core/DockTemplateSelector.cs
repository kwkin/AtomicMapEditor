using System.Windows;
using System.Windows.Controls;
using AtomicMapEditor.Modules.Docks.ClipboardDock;
using AtomicMapEditor.Modules.Docks.ItemEditorDock;
using AtomicMapEditor.Modules.Docks.ItemListDock;
using AtomicMapEditor.Modules.Docks.LayerListDock;
using AtomicMapEditor.Modules.Docks.MinimapDock;
using AtomicMapEditor.Modules.Docks.SelectedBrushDock;
using AtomicMapEditor.Modules.Docks.ToolboxDock;
using AtomicMapEditor.Modules.MapEditor.Editor;
using Xceed.Wpf.AvalonDock.Layout;

namespace AtomicMapEditor.Modules.Docks.Core
{
    internal class DockTemplateSelector : DataTemplateSelector
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public DockTemplateSelector()
        {
        }

        #endregion constructor & destructer


        #region properties

        public DataTemplate ClipboardDataTemplate { get; set; }
        public DataTemplate ItemEditorDataTemplate { get; set; }
        public DataTemplate ItemListDataTemplate { get; set; }
        public DataTemplate LayerListDataTemplate { get; set; }
        public DataTemplate MinimapDataTemplate { get; set; }
        public DataTemplate ToolboxDataTemplate { get; set; }
        public DataTemplate SelectedBrushDataTemplate { get; set; }

        public DataTemplate MainEditorTemplate { get; set; }

        #endregion properties


        #region methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // TODO: add dock factory class for templates and styles.
            var itemAsLayoutContent = item as LayoutContent;

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
            else if (item is MainEditorViewModel)
            {
                return MainEditorTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        #endregion methods
    }
}
