using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions
{
    public interface IWindowInteractionCreator
    {
        #region properties

        IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        IWindowInteraction CreateWindowInteraction();
        IWindowInteraction CreateWindowInteraction(Action<INotification> callback);
        bool AppliesTo(Type type);

        #endregion methods
    }
}
