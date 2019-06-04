using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ame.App.Wpf.UI.Docks
{
    internal class DockStyleSelector : StyleSelector
    {
        #region properties

        public Style DockWindowStyle { get; set; }
        public Style DocumentStyle { get; set; }

        #endregion properties


        #region methods

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is DockToolViewModelTemplate)
            {
                return DockWindowStyle;
            }
            else if (item is EditorViewModelTemplate)
            {
                return DocumentStyle;
            }
            return base.SelectStyle(item, container);
        }

        #endregion methods
    }
}
