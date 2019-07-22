using Ame.Infrastructure.BaseTypes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Events.Messages
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
