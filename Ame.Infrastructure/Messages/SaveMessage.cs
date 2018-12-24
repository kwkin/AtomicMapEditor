using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Messages
{
    public class SaveMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public SaveMessage(string path, Map map)
        {
            this.Path = path;
            this.Map = map;
        }

        #endregion Constructor


        #region Properties

        public string Path { get; set; }
        public Map Map { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
