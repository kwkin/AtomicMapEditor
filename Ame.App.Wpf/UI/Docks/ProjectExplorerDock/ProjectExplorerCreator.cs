using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    public class ProjectExplorerCreator : DockCreatorTemplate
    {
        #region fields

        public IEventAggregator eventAggregator;
        public IActionHandler actionHandler;

        #endregion fields


        #region constructors

        public ProjectExplorerCreator(IEventAggregator eventAggregator, IActionHandler handler, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Session = session ?? throw new ArgumentNullException("session is null");
            this.actionHandler = handler ?? throw new ArgumentNullException("session is null");
        }

        #endregion constructors


        #region properties

        public IAmeSession Session { get; set; }

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new ProjectExplorerViewModel(this.eventAggregator, actionHandler, this.Session);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(ProjectExplorerViewModel).Equals(type);
        }

        #endregion methods
    }
}
