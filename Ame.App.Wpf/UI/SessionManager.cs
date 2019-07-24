using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.App.Wpf.UI
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
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session");

            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Subscribe(
                NotificationReceived,
                ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        private void NotificationReceived(NotificationMessage<LayerNotification> notification)
        {
            Map currentMap = this.session.CurrentMap.Value;
            switch (notification.Content)
            {
                case LayerNotification.MergeCurrentLayerDown:
                    currentMap.MergeCurrentLayerDown();
                    break;

                case LayerNotification.MergeCurrentLayerUp:
                    currentMap.MergeCurrentLayerUp();
                    break;

                case LayerNotification.MergeVisibleLayers:
                    currentMap.MergeVisibleLayers();
                    break;

                case LayerNotification.DeleteCurrentLayer:
                    currentMap.DeleteCurrentLayer();
                    break;

                case LayerNotification.DuplicateCurrentLayer:
                    currentMap.DuplicateCurrentLayer();
                    break;

                case LayerNotification.NewLayerGroup:
                    currentMap.NewLayerGroup();
                    break;

                default:
                    break;
            }
        }

        #endregion methods
    }
}
