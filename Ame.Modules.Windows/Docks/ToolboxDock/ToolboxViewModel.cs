using System;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ToolboxDock
{
    public class ToolboxViewModel : DockToolViewModelTemplate
    {
        #region fields

        public IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public ToolboxViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.Title = "Tools";

            this.ToolButtonCommand = new DelegateCommand<string>((s) => SetToolboxTitle(s));
        }

        #endregion constructor


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
