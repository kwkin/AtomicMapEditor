using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.Modules.Docks
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

            this.eventAggregator.GetEvent<NotificationEvent<WindowType>>().Subscribe(
                CreateNewLayerGroup,
                ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<Notification>>().Subscribe(
                NotificationReceived,
                ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods
        
        private void CreateNewLayerGroup(NotificationMessage<WindowType> layerGroup)
        {
            if (layerGroup.Content == WindowType.NewLayerGroup)
            {
                int layerGroupCount = GetLayerGroupCount();
                String newLayerGroupName = String.Format("Layer Group #{0}", layerGroupCount);
                ILayer newLayerGroup = new LayerGroup(newLayerGroupName);
                this.session.CurrentMap.LayerList.Add(newLayerGroup);
            }
        }

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

                default:
                    break;
            }
        }

        private int GetLayerGroupCount()
        {
            int layerGroupCount = 0;
            foreach (ILayer layer in this.session.CurrentMap.LayerList)
            {
                if (layer is LayerGroup)
                {
                    layerGroupCount++;
                }
            }
            return layerGroupCount;
        }

        private int GetLayerCount()
        {
            int layerCount = 0;
            foreach (ILayer layer in this.session.CurrentMap.LayerList)
            {
                if (layer is Layer)
                {
                    layerCount++;
                }
            }
            return layerCount;
        }

        #endregion methods
    }
}
