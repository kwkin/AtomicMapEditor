using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.TilesetEditorInteraction
{
    public class EditTilesetInteractionCreator : WindowInteractionCreatorTemplate
    {
        #region fields
        
        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public EditTilesetInteractionCreator(AmeSession session, IEventAggregator eventAggregator)
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

        public EditTilesetInteractionCreator(AmeSession session, IEventAggregator eventAggregator, Action<INotification> callback)
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
            IWindowInteraction interaction;
            if (this.TilesetModel != null)
            {
                interaction = new EditTilesetInteraction(this.TilesetModel, this.eventAggregator, callback);
            }
            else
            {
                interaction = new EditTilesetInteraction(this.Session, this.eventAggregator, Callback);
            }
            return interaction;
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(EditTilesetInteraction).Equals(type);
        }

        #endregion methods
    }
}
