using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerPropertiesInteraction
{
    public class NewLayerInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region constructors

        // TODO use this constructor
        public NewLayerInteractionCreator(AmeSession session, IEventAggregator eventAggregator)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
            if (session.CurrentMap != null)
            {
                string newLayerName = string.Format("Layer #{0}", session.CurrentMap.LayerCount);
                this.Container.RegisterInstance<ILayer>(new Layer(newLayerName, 32, 32, 32, 32));
            }
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        public NewLayerInteractionCreator(AmeSession session, IEventAggregator eventAggregator, Action<INotification> callback)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
            if (session.CurrentMap != null)
            {
                string newLayerName = string.Format("Layer #{0}", session.CurrentMap.LayerCount);
                this.Container.RegisterInstance<ILayer>(new Layer(newLayerName, 32, 32, 32, 32));
            }
            else
            {
                this.Container.RegisterInstance<ILayer>(new Layer("Layer #0", 32, 32, 32, 32));
            }
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
            this.Container.RegisterInstance<Action<INotification>>(callback);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return this.Container.Resolve<NewLayerInteraction>();
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            IUnityContainer container = new UnityContainer();
            foreach (ContainerRegistration registration in this.Container.Registrations)
            {
                container.RegisterInstance<ContainerRegistration>(registration);
            }
            container.RegisterInstance<Action<INotification>>(callback);
            return container.Resolve<NewLayerInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(NewLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
