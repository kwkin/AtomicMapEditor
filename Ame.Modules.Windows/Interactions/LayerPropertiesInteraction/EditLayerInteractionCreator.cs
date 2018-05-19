using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerPropertiesInteraction
{
    public class EditLayerInteractionCreator : WindowInteractionCreatorTemplate
    {
        #region fields
        
        #endregion fields


        #region constructors

        public EditLayerInteractionCreator(AmeSession session)
        {
            this.Session = session;
        }

        public EditLayerInteractionCreator(AmeSession session, Action<INotification> callback)
        {
            this.Session = session;
            this.Callback = callback;
        }

        #endregion constructors


        #region properties

        public Layer Layer { get; set; }
        public AmeSession Session { get; set; }
        public Action<INotification> Callback { get; set; }

        #endregion properties


        #region methods

        public override IWindowInteraction CreateWindowInteraction()
        {
            return CreateWindowInteraction(this.Callback);
        }

        public override IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            Layer editLayer = null;
            if (this.Layer != null)
            {
                editLayer = this.Layer;
            }
            else
            {
                if (this.Session.CurrentMap != null)
                {
                    if (this.Session.CurrentMap.CurrentLayer != null)
                    {
                        editLayer = this.Session.CurrentMap.CurrentLayer as Layer;
                    }
                }
            }
            if (editLayer == null)
            {
                editLayer = new Layer(32, 32, 32, 32);
            }
            return new EditLayerInteraction(editLayer, callback);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(EditLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
