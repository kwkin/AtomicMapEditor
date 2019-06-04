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

        #endregion fields


        #region constructors

        public SessionViewerCreator(IEventAggregator eventAggregator, AmeSession session)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.eventAggregator = eventAggregator;
            this.Session = session;
        }

        #endregion constructors


        #region properties
        public AmeSession Session { get; set; }

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new SessionViewerViewModel(this.eventAggregator, this.Session);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(SessionViewerViewModel).Equals(type);
        }

        #endregion methods
    }
}
