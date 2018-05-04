using System;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class NewLayerFactory : IWindowInteractionFactory
    {
        #region fields

        #endregion fields


        #region constructors

        public NewLayerFactory(AmeSession session, IEventAggregator eventAggregator)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.Container = new UnityContainer();
            string newLayerName = string.Format("Layer #{0}", session.CurrentMap.LayerCount);
            this.Container.RegisterInstance<ILayer>(new Layer(newLayerName, 32, 32, 32, 32));
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return this.Container.Resolve(typeof(NewLayerInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(NewLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
