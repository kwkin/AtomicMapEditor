using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Messages
{
    public class EditLayerMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public EditLayerMessage(Layer layer)
        {
            this.Layer = layer;
        }

        #endregion Constructor


        #region Properties

        public Layer Layer { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
