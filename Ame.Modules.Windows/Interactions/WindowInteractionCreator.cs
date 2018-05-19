using System;
using System.Linq;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions
{
    public class WindowInteractionCreator
    {
        #region fields

        private readonly WindowInteractionCreatorTemplate[] windowInteractionFactories;

        #endregion fields


        #region constructors

        public WindowInteractionCreator(WindowInteractionCreatorTemplate[] windowInteractionCreators)
        {
            this.windowInteractionFactories = windowInteractionCreators;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction(Type type)
        {
            var windowInteractionCreator = this.windowInteractionFactories.FirstOrDefault(factory => factory.AppliesTo(type));
            if (windowInteractionCreator == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            return windowInteractionCreator.CreateWindowInteraction();
        }

        public IWindowInteraction CreateWindowInteraction(Type type, Action<INotification> notification)
        {
            var windowInteractionCreator = this.windowInteractionFactories.FirstOrDefault(factory => factory.AppliesTo(type));
            if (windowInteractionCreator == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            return windowInteractionCreator.CreateWindowInteraction();
        }

        public void UpdateContainer(Type creatorType, Type valueType, object value)
        {
            var windowInteractionCreator = this.windowInteractionFactories.FirstOrDefault(factory => factory.AppliesTo(creatorType));
            if (windowInteractionCreator == null)
            {
                throw new Exception(string.Format("{0} type is not registered", creatorType));
            }
            windowInteractionCreator.UpdateContent(valueType, value);
        }

        #endregion methods
    }
}
