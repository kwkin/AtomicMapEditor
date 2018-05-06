using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Infrastructure.Messages
{
    public class OpenDockMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public OpenDockMessage(DockType type)
        {
            this.Type = type;
            this.Title = "";
        }

        public OpenDockMessage(DockType type, IUnityContainer container)
        {
            this.Type = type;
            this.Container = container;
            this.Title = "";
        }

        public OpenDockMessage(DockType type, String title)
        {
            this.Type = type;
            this.Title = title;
        }

        public OpenDockMessage(DockType type, IUnityContainer container, String title)
        {
            this.Type = type;
            this.Container = container;
            this.Title = title;
        }

        #endregion Constructor


        #region Properties

        public DockType Type { get; set; }
        public String Title { get; set; }
        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
