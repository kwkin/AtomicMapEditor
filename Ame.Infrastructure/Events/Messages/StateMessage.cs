using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ame.Infrastructure.Events.Messages
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
        public BitmapEncoder Encoder { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
