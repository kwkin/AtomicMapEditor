using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Infrastructure.Messages
{
    public class OpenWindowMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public OpenWindowMessage(Type type)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Title = "";
        }

        public OpenWindowMessage(Type type, IUnityContainer container)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Container = container;
            this.Title = "";
        }

        public OpenWindowMessage(Type type, String title)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Title = title;
        }

        public OpenWindowMessage(Type type, IUnityContainer container, String title)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Container = container;
            this.Title = "";
        }

        #endregion Constructor


        #region Properties

        public Type Type;
        public String Title { get; set; }
        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
