using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Infrastructure.Messages.Interactions
{
    public class TilesetInteractionMessage : Confirmation
    {
        #region fields

        #endregion fields


        #region Constructor

        public TilesetInteractionMessage(TilesetModel tileset)
        {
            this.Tileset = tileset;
        }

        public TilesetInteractionMessage(TilesetModel tileset, bool isEditing)
        {
            this.Tileset = tileset;
            this.IsEditing = isEditing;
        }

        #endregion Constructor


        #region Properties

        public TilesetModel Tileset { get; set; }
        public bool IsEditing { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
