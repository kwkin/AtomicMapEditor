using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Messages
{
    public class CloseDockMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public CloseDockMessage(DockViewModelTemplate dock)
        {
            this.Dock = dock;
        }

        #endregion Constructor


        #region Properties

        public DockViewModelTemplate Dock { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
