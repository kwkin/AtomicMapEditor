using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Events
{
    public class EditLayerMessage
    {
        #region fields

        #endregion fields


        #region Constructor & destructer

        public EditLayerMessage(Layer layer)
        {
            this.Layer = layer;
        }

        #endregion Constructor & destructer


        #region Properties

        public Layer Layer { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
