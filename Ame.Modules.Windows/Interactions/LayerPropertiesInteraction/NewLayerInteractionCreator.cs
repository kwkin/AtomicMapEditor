using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerPropertiesInteraction
{
    public class NewLayerInteractionCreator : WindowInteractionCreatorTemplate
    {
        #region fields

        public IEventAggregator eventAggregator;

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
            this.Session = session;
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
            this.Session = session;
            this.eventAggregator = eventAggregator;
            this.Callback = callback;
        }

        #endregion constructors


        #region properties

        public AmeSession Session { get; set; }
        public Action<INotification> Callback { get; set; }

        #endregion properties


        #region methods

        public override IWindowInteraction CreateWindowInteraction()
        {
            return CreateWindowInteraction(this.Callback);
        }

        public override IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            string newLayerName = string.Format("Layer #{0}", this.Session.CurrentMap.LayerCount);
            ILayer layer = new Layer(newLayerName, 32, 32, 32, 32);
            return new NewLayerInteraction(layer, this.eventAggregator, callback);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(NewLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
