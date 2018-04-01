using System;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Events
{
    public class OpenDockMessage
    {
        #region fields

        #endregion fields


        #region Constructor & destructer

        public OpenDockMessage(DockType dockType)
        {
            this.DockType = dockType;
            this.DockTitle = "";
        }

        public OpenDockMessage(DockType dockType, String dockTitle)
        {
            this.DockType = dockType;
            this.DockTitle = dockTitle;
        }

        #endregion Constructor & destructer


        #region Properties

        public DockType DockType { get; set; }
        public String DockTitle { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
