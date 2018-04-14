using System;
using System.Windows;
using Microsoft.Practices.Unity;

namespace Ame.Infrastructure.Events
{
    public enum WindowType
    {
        Map, Layer
    }

    public class OpenWindowMessage
    {
        #region fields

        #endregion fields


        #region Constructor & destructer

        public OpenWindowMessage(WindowType windowType)
        {
            this.WindowType = windowType;
            this.WindowTitle = "";
        }

        public OpenWindowMessage(WindowType windowType, String windowTitle)
        {
            this.WindowType = windowType;
            this.WindowTitle = windowTitle;
        }

        #endregion Constructor & destructer


        #region Properties

        public Window Parent { get; set; }
        public WindowType WindowType { get; set; }
        public String WindowTitle { get; set; }
        public object Content { get; set; }
        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
