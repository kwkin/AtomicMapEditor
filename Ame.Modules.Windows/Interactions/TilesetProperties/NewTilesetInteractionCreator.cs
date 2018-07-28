using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.TilesetProperties
{
    public class NewTilesetInteractionCreator : WindowInteractionCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public NewTilesetInteractionCreator(AmeSession session, IEventAggregator eventAggregator)
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

        public NewTilesetInteractionCreator(AmeSession session, IEventAggregator eventAggregator, Action<INotification> callback)
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
        public TilesetModel TilesetModel { get; set; }
        public Action<INotification> Callback { get; set; }

        #endregion properties


        #region methods

        public override IWindowInteraction CreateWindowInteraction()
        {
            return CreateWindowInteraction(this.Callback);
        }

        public override IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            return new NewTilesetInteraction(this.Session, this.eventAggregator, callback);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(NewTilesetInteraction).Equals(type);
        }

        #endregion methods
    }
}
