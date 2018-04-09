using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Ame.Modules.Docks.ClipboardDock;
using Ame.Modules.Docks.ItemEditorDock;
using Ame.Modules.Docks.ItemListDock;
using Ame.Modules.Docks.LayerListDock;
using Ame.Modules.Docks.MinimapDock;
using Ame.Modules.Docks.SelectedBrushDock;
using Ame.Modules.Docks.ToolboxDock;
using Ame.Modules.MapEditor.Editor;
using Prism.Mvvm;
using Xceed.Wpf.AvalonDock.Layout;

namespace Ame.Modules.Docks.Core
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
