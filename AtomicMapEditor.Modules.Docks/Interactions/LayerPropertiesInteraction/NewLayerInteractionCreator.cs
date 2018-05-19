using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerPropertiesInteraction
{
    public class NewLayerInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        private AmeSession session;
        private IEventAggregator eventAggregator;
        private Action<INotification> callback;

        #endregion fields


        #region constructors
        
        public NewLayerInteractionCreator(AmeSession session, IEventAggregator eventAggregator)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.session = session;
            this.eventAggregator = eventAggregator;
        }

        public NewLayerInteractionCreator(AmeSession session, IEventAggregator eventAggregator, Action<INotification> callback)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.session = session;
            this.eventAggregator = eventAggregator;
            this.callback = callback;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            string newLayerName = string.Format("Layer #{0}", session.CurrentMap.LayerCount);
            ILayer layer = new Layer(newLayerName, 32, 32, 32, 32);
            return new NewLayerInteraction(layer, this.eventAggregator, this.callback);
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            string newLayerName = string.Format("Layer #{0}", session.CurrentMap.LayerCount);
            ILayer layer = new Layer(newLayerName, 32, 32, 32, 32);
            return new NewLayerInteraction(layer, this.eventAggregator, callback);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(NewLayerInteraction).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(AmeSession).Equals(type))
            {
                this.session = value as AmeSession;
            }
            else if (typeof(Action<INotification>).Equals(type))
            {
                this.eventAggregator = value as IEventAggregator;
            }
            else if (typeof(Action<INotification>).Equals(type))
            {
                this.callback = value as Action<INotification>;
            }
        }

        #endregion methods
    }
}
