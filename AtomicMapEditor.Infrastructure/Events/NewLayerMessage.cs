using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Events
{
    public class NewLayerMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public NewLayerMessage(Layer layer)
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
