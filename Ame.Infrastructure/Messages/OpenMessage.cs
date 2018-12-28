using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Messages
{
    public class OpenMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public OpenMessage(string path)
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
