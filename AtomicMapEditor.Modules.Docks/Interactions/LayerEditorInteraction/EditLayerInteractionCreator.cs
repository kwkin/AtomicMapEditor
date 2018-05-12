using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerEditorInteraction
{
    public class EditLayerInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        private Action<INotification> callback;

        #endregion fields


        #region constructors

        public EditLayerInteractionCreator(ILayer layer)
        {
            this.Container = new UnityContainer();
            if (layer != null)
            {
                this.Container.RegisterInstance<ILayer>(layer);
            }
            else
            {
                this.Container.RegisterInstance<ILayer>(new Layer(32, 32, 32, 32));
            }
        }

        public EditLayerInteractionCreator( ILayer layer, Action<INotification> callback)
        {
            this.callback = callback;
            this.Container = new UnityContainer();
            if (layer != null)
            {
                this.Container.RegisterInstance<ILayer>(layer);
            }
            else
            {
                this.Container.RegisterInstance<ILayer>(new Layer(32, 32, 32, 32));
            }
            this.Container.RegisterInstance<Action<INotification>>(callback);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            if (!this.Container.IsRegistered<Action<INotification>>())
            {
                this.Container.RegisterInstance<Action<INotification>>(callback);
            }
            return this.Container.Resolve<EditLayerInteraction>();
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            IUnityContainer container = new UnityContainer();
            foreach (ContainerRegistration registration in this.Container.Registrations)
            {
                container.RegisterInstance<ContainerRegistration>(registration);
            }
            container.RegisterInstance<Action<INotification>>(callback);
            return container.Resolve<EditLayerInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
