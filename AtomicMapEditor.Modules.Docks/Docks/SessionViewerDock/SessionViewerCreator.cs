using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.SessionViewerDock
{
    public class SessionViewerCreator : IDockCreator
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

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
            this.session = session;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            return new SessionViewerViewModel(this.eventAggregator, this.session);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(SessionViewerViewModel).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(IEventAggregator).Equals(type))
            {
                this.eventAggregator = value as IEventAggregator;
            }
            else if (typeof(AmeSession).Equals(type))
            {
                this.session = value as AmeSession;
            }
        }

        #endregion methods
    }
}
