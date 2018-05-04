using System;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class NewMapFactory : IWindowInteractionFactory
    {
        #region fields

        private IUnityContainer container;

        #endregion fields


        #region constructors

        public NewMapFactory(AmeSession session, IEventAggregator eventAggregator)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.container = new UnityContainer();
            this.container.RegisterInstance<AmeSession>(session);
            this.container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return container.Resolve(typeof(NewMapInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(NewMapInteraction).Equals(type);
        }

        #endregion methods
    }
}
