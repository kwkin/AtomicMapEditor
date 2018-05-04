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

        public OpenWindowMessage(Type windowType)
        {
            this.WindowType = windowType;
            this.WindowTitle = "";
        }

        public OpenWindowMessage(Type windowType, String windowTitle)
        {
            this.WindowType = windowType;
            this.WindowTitle = windowTitle;
        }

        #endregion Constructor


        #region Properties

        public Type WindowType;
        public String WindowTitle { get; set; }
        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
