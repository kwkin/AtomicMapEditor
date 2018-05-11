using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.Modules.Windows
{
    public class SessionManager
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructor

        public SessionManager(IEventAggregator eventAggregator, AmeSession session)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.eventAggregator = eventAggregator;
            this.session = session;
            
            this.eventAggregator.GetEvent<NotificationEvent<Notification>>().Subscribe(
                NotificationReceived,
                ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        private void NotificationReceived(NotificationMessage<Notification> notification)
        {
            Map currentMap = this.session.CurrentMap;
            switch (notification.Content)
            {
                case Notification.MergeCurrentLayerDown:
                    currentMap.MergeCurrentLayerDown();
                    break;

                case Notification.MergeCurrentLayerUp:
                    currentMap.MergeCurrentLayerUp();
                    break;

                case Notification.MergeVisibleLayers:
                    currentMap.MergeVisibleLayers();
                    break;

                case Notification.DeleteCurrentLayer:
                    currentMap.DeleteCurrentLayer();
                    break;

                case Notification.DuplicateCurrentLayer:
                    currentMap.DuplicateCurrentLayer();
                    break;

                case Notification.NewLayerGroup:
                    currentMap.NewLayerGroup();
                    break;

                default:
                    break;
            }
        }

        #endregion methods
    }
}
