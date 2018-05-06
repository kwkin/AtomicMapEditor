using System;
using System.Windows;
using Microsoft.Practices.Unity;

namespace Ame.Infrastructure.Messages
{
    public enum WindowType
    {
        NewLayerGroup,
        ImageEditor
    }

    public class OpenWindowMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public OpenWindowMessage(Type type)
        {
            this.Type = type;
            this.Title = "";
        }

        public OpenWindowMessage(Type type, IUnityContainer container)
        {
            this.Type = type;
            this.Container = container;
            this.Title = "";
        }

        public OpenWindowMessage(Type type, String title)
        {
            this.Type = type;
            this.Title = title;
        }

        public OpenWindowMessage(Type type, IUnityContainer container, String title)
        {
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
