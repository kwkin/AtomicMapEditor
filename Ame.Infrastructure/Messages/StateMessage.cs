using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Messages
{
    public class StateMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public StateMessage(string path)
        {
            this.Path = path;
        }

        #endregion Constructor


        #region Properties

        public string Path { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
