using System;
using System.Windows;

namespace AtomicMapEditor.Infrastructure.Events
{
    public enum WindowType
    {
        Map
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

        #endregion Properties


        #region methods


        #endregion methods
    }
}
