using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;

        #endregion fields


        #region constructors

        public LayerListCreator(IEventAggregator eventAggregator, IAmeSession session, IActionHandler actionHandler)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("handler is null");
            this.Session = session ?? throw new ArgumentNullException("session is null");
        }

        #endregion constructors


        #region properties

        public IAmeSession Session { get; set; }

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new LayerListViewModel(this.eventAggregator, this.Session, this.actionHandler);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(LayerListViewModel).Equals(type);
        }

        #endregion methods
    }
}
