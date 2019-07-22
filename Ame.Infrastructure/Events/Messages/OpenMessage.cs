using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Events.Messages
{
    public class OpenMapMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public OpenMapMessage(Map map)
        {
            this.Map = map;
        }

        #endregion Constructor


        #region Properties

        public Map Map { get; set; }

        #endregion Properties


        #region methods

        #endregion methods
    }
}
