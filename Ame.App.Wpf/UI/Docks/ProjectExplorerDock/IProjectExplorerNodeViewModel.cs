using Ame.Infrastructure.BaseTypes;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{

    public interface IProjectExplorerNodeViewModel
    {
        #region fields

        #endregion fields


        #region properties

        BindableProperty<bool> IsSelected { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
