using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapEditorInteraction
{
    public class NewMapInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public NewMapInteractionCreator(AmeSession session, IEventAggregator eventAggregator)
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
            this.Container.RegisterInstance<AmeSession>(session);
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        public NewMapInteractionCreator(AmeSession session, IEventAggregator eventAggregator, Action<INotification> callback)
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
            this.Container.RegisterInstance<AmeSession>(session);
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
            return this.Container.Resolve<NewMapInteraction>();
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            IUnityContainer container = new UnityContainer();
            foreach (ContainerRegistration registration in this.Container.Registrations)
            {
                container.RegisterInstance<ContainerRegistration>(registration);
            }
            container.RegisterInstance<Action<INotification>>(callback);
            return container.Resolve<EditMapInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(NewMapInteraction).Equals(type);
        }

        #endregion methods
    }
}
