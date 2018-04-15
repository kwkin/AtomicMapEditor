using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Infrastructure.Events
{
    public class OpenDockMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public OpenDockMessage(DockType dockType)
        {
            this.DockType = dockType;
            this.DockTitle = "";
        }

        public OpenDockMessage(DockType dockType, IUnityContainer container)
        {
            this.DockType = dockType;
            this.Container = container;
            this.DockTitle = "";
        }

        public OpenDockMessage(DockType dockType, IUnityContainer container, String dockTitle)
        {
            this.DockType = dockType;
            this.Container = container;
            this.DockTitle = dockTitle;
        }

        #endregion Constructor


        #region Properties

        public DockType DockType { get; set; }
        public IUnityContainer Container { get; set; }
        public String DockTitle { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
