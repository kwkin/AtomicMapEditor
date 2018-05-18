using System;
using Ame.Infrastructure.BaseTypes;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions
{
    public interface IWindowInteractionCreator
    {
        #region properties

        #endregion properties


        #region methods
        
        IWindowInteraction CreateWindowInteraction();
        IWindowInteraction CreateWindowInteraction(Action<INotification> callback);
        bool AppliesTo(Type type);
        void UpdateContent(Type type, object value);

        #endregion methods
    }
}
