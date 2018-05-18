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
        private Action<INotification> callback;

        #endregion fields


        #region constructors

        public EditLayerInteractionCreator(ILayer layer)
        {
            this.layer = layer ?? new Layer(32, 32, 32, 32);
        }

        public EditLayerInteractionCreator(ILayer layer, Action<INotification> callback)
        {
            this.layer = layer ?? new Layer(32, 32, 32, 32);
            this.callback = callback;
        }

        #endregion constructors


        #region properties
        
        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return new EditLayerInteraction(this.layer, this.callback);
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            return new EditLayerInteraction(this.layer, callback);
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
