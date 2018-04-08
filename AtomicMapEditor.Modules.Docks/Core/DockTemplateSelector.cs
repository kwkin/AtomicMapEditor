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

        DataTemplate CreateTemplate(Type viewModelType, Type viewType, object item)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            var context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            return template;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // TODO: add dock factory class for templates and styles.
            if (item is ClipboardViewModel)
            {
                return CreateTemplate(typeof(ClipboardViewModel), typeof(ClipboardDock.Clipboard), item);
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
