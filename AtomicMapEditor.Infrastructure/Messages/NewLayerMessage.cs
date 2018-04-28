using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Messages
{
    public class NewLayerMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public NewLayerMessage(ILayer layer)
        {
            this.Layer = layer;
        }

        #endregion Constructor


        #region Properties

        public ILayer Layer { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
