using System.Windows;
using System.Windows.Controls;
using AtomicMapEditor.Infrastructure.BaseTypes;

namespace AtomicMapEditor.Modules.Docks.Core
{
    internal class DockStyleSelector : StyleSelector
    {
        #region properties
        
        public Style DocumentStyle { get; set; }

        #endregion properties


        #region methods

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is EditorViewModelTemplate)
            {
                return DocumentStyle;
            }
            return base.SelectStyle(item, container);
        }

        #endregion methods
    }
}