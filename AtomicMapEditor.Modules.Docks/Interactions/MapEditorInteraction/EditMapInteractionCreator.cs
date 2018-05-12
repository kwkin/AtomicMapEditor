using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapEditorInteraction
{
    public class EditMapInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public EditMapInteractionCreator(AmeSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<AmeSession>(session);
        }

        public EditMapInteractionCreator(AmeSession session, Action<INotification> callback)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<AmeSession>(session);
            this.Container.RegisterInstance<Action<INotification>>(callback);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return Container.Resolve<EditMapInteraction>();
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
            return typeof(EditMapInteraction).Equals(type);
        }

        #endregion methods
    }
}
