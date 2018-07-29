using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Infrastructure.Messages.Interactions
{
    public class LayerInteractionMessage : Confirmation
    {
        #region fields

        #endregion fields


        #region Constructor

        public LayerInteractionMessage(Layer layer)
        {
            this.Layer = layer;
        }

        public LayerInteractionMessage(Layer layer, bool isEditing)
        {
            this.Layer = layer;
            this.IsEditing = isEditing;
        }

        #endregion Constructor


        #region Properties

        public Layer Layer { get; set; }
        public bool IsEditing { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
