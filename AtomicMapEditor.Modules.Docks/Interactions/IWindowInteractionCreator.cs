using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.Interactions
{
    public interface IWindowInteractionCreator
    {
        #region properties

        IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        IWindowInteraction CreateWindowInteraction();
        bool AppliesTo(Type type);

        #endregion methods
    }
}
