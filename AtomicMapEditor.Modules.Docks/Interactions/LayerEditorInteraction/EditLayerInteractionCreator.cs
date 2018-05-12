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

        #endregion fields


        #region constructors

        public EditLayerInteractionCreator(AmeSession session, ILayer layer)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
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

        public EditLayerInteractionCreator(AmeSession session, ILayer layer, Action<INotification> callback)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
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
            return this.Container.Resolve<EditLayerInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
