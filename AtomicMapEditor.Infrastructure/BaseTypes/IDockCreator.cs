using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows
{
    public interface IDockCreator
    {
        #region properties

        IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        DockViewModelTemplate CreateDock();

        bool AppliesTo(Type type);

        #endregion methods
    }
}
