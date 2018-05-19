using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerPropertiesInteraction
{
    public class EditLayerInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        private ILayer layer;
        private AmeSession session;
        private Action<INotification> callback;

        #endregion fields


        #region constructors

        public EditLayerInteractionCreator(AmeSession session)
        {
            this.session = session;
        }

        public EditLayerInteractionCreator(AmeSession session, Action<INotification> callback)
        {
            this.session = session;
            this.callback = callback;
        }

        #endregion constructors


        #region properties
        
        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return CreateWindowInteraction(this.callback);
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            ILayer editLayer = null;
            if (this.layer != null)
            {
                editLayer = this.layer;
            }
            else
            {
                if (this.session.CurrentMap != null)
                {
                    if (this.session.CurrentMap.CurrentLayer != null)
                    {
                        editLayer = this.session.CurrentMap.CurrentLayer;
                    }
                }
            }
            if (editLayer == null)
            {
                editLayer = new Layer(32, 32, 32, 32);
            }
            IWindowInteraction interaction = new EditLayerInteraction(editLayer, callback);
            this.layer = null;
            return interaction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditLayerInteraction).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(Layer).Equals(type))
            {
                this.layer = value as Layer;
            }
            else if (typeof(Action<INotification>).Equals(type))
            {
                this.callback = value as Action<INotification>;
            }
        }

        #endregion methods
    }
}
