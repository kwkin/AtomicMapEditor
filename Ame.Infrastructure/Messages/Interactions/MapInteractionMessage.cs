using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Infrastructure.Messages.Interactions
{
    public class MapInteractionMessage : Confirmation
    {
        #region fields

        #endregion fields


        #region Constructor

        public MapInteractionMessage(Map map)
        {
            this.Map = map;
        }

        public MapInteractionMessage(Map map, bool isEditing)
        {
            this.Map = map;
            this.IsEditing = isEditing;
        }

        #endregion Constructor


        #region Properties

        public Map Map { get; set; }
        public bool IsEditing { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
