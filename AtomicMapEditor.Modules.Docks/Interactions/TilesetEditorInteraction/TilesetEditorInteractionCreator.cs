using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.TilesetEditorInteraction
{
    public class TilesetEditorInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public TilesetEditorInteractionCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        public TilesetEditorInteractionCreator(IEventAggregator eventAggregator, Action<INotification> callback)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
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
            return Container.Resolve<TilesetEditorInteraction>();
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            IUnityContainer container = new UnityContainer();
            foreach (ContainerRegistration registration in this.Container.Registrations)
            {
                container.RegisterInstance<ContainerRegistration>(registration);
            }
            container.RegisterInstance<Action<INotification>>(callback);
            return container.Resolve<TilesetEditorInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(TilesetEditorInteraction).Equals(type);
        }

        #endregion methods
    }
}
