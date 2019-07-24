using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.SessionViewerDock
{
    public class SessionViewerCreator : DockCreatorTemplate
    {
        #region fields

        public IEventAggregator eventAggregator;
        public IAmeSession session;

        #endregion fields


        #region constructors

        public SessionViewerCreator(IEventAggregator eventAggregator, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new SessionViewerViewModel(this.eventAggregator, this.session);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(SessionViewerViewModel).Equals(type);
        }

        #endregion methods
    }
}
