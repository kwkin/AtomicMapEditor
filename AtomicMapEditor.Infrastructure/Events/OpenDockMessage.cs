using System;

namespace Ame.Infrastructure.Events
{
    public enum DockType
    {
        Clipboard, ItemEditor, ItemList, LayerList, Minimap, SelectedBrush, Toolbox
    }

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
