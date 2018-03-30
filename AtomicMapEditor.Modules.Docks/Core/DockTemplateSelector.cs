using System.Windows;
using System.Windows.Controls;
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

        public DataTemplate MainEditorTemplate { get; set; }

        #endregion properties


        #region methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is MainEditorViewModel)
            {
                return MainEditorTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        #endregion methods
    }
}
