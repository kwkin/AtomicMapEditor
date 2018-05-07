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

        // TODO check the type
        public OpenDockMessage(Type type)
        {
            this.Type = type;
            this.Title = "";
        }

        public OpenDockMessage(Type type, IUnityContainer container)
        {
            this.Type = type;
            this.Container = container;
            this.Title = "";
        }

        public OpenDockMessage(Type type, String title)
        {
            this.Type = type;
            this.Title = title;
        }

        public OpenDockMessage(Type type, IUnityContainer container, String title)
        {
            this.Type = type;
            this.Container = container;
            this.Title = title;
        }

        #endregion Constructor


        #region Properties

        public Type Type { get; set; }
        public String Title { get; set; }
        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
