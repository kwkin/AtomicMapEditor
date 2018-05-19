using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows
{
    public interface IDockCreator
    {
        #region properties

        #endregion properties


        #region methods

        DockViewModelTemplate CreateDock();
        bool AppliesTo(Type type);
        void UpdateContent(Type type, object value);

        #endregion methods
    }
}
