using System;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Prism.Commands;

namespace Ame.Modules.Docks.ToolboxDock
{
    internal class ToolboxViewModel : DockToolViewModelTemplate
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public ToolboxViewModel()
        {
            this.Title = "Tools";
            this.ContentId = "Tools";

            this.ToolButtonCommand = new DelegateCommand<string>((s) => SetToolboxTitle(s));
        }

        #endregion constructor & destructer


        #region properties

        public ICommand ToolButtonCommand { get; private set; }

        public bool StampButtonValue { get; set; }
        public bool BrushButtonValue { get; set; }

        #endregion properties


        #region methods

        public void SetToolboxTitle(string brushTitle)
        {
            if (!String.IsNullOrEmpty(brushTitle))
            {
                this.Title = "Tools - " + brushTitle;
            }
            else
            {
                this.Title = "Tools";
            }
        }

        #endregion methods
    }
}
