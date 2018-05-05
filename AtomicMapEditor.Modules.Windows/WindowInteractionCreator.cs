using System;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class WindowInteractionCreator
    {
        #region fields

        private readonly IWindowInteractionCreator[] windowInteractionFactories;

        #endregion fields


        #region constructors

        public WindowInteractionCreator(IWindowInteractionCreator[] windowInteractionFactories)
        {
            this.windowInteractionFactories = windowInteractionFactories;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction(Type type)
        {
            var windowInteractionFactory = this.windowInteractionFactories.FirstOrDefault(factory => factory.AppliesTo(type));
            if (windowInteractionFactory == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            return windowInteractionFactory.CreateWindowInteraction();
        }

        public IWindowInteraction CreateWindowInteraction(Type type, IUnityContainer container)
        {
            var windowInteractionFactory = this.windowInteractionFactories.FirstOrDefault(factory => factory.AppliesTo(type));
            windowInteractionFactory.Container = container;
            if (windowInteractionFactory == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            return windowInteractionFactory.CreateWindowInteraction();
        }

        #endregion methods
    }
}
