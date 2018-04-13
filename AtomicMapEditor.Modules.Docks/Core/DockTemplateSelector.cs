using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Ame.Modules.Docks.ClipboardDock;
using Ame.Modules.Docks.ItemEditorDock;
using Ame.Modules.Docks.ItemListDock;
using Ame.Modules.Docks.LayerListDock;
using Ame.Modules.Docks.MinimapDock;
using Ame.Modules.Docks.SelectedBrushDock;
using Ame.Modules.Docks.ToolboxDock;
using Ame.Modules.MapEditor.Editor;

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
        
        #endregion properties


        #region methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = new DataTemplate(typeof(UserControl));
            FrameworkElementFactory frameworkElementFactory;
            if (item is ClipboardViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(ClipboardDock.Clipboard));
            }
            else if (item is ItemEditorViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(ItemEditor));
            }
            else if (item is ItemListViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(ItemList));
            }
            else if (item is LayerListViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(LayerList));
            }
            else if (item is MinimapViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(Minimap));
            }
            else if (item is ToolboxViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(Toolbox));
            }
            else if (item is SelectedBrushViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(SelectedBrush));
            }
            else if (item is MainEditorViewModel)
            {
                frameworkElementFactory = new FrameworkElementFactory(typeof(MainEditor));
            }
            else
            {
                return base.SelectTemplate(item, container);
            }
            frameworkElementFactory.SetValue(UserControl.DataContextProperty, item);
            template.VisualTree = frameworkElementFactory;
            return template;
        }

        #endregion methods
    }
}
