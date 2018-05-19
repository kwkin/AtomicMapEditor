using System;
using Ame.Infrastructure.BaseTypes;
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

        public EditTilesetInteractionCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
        }

        public EditTilesetInteractionCreator(IEventAggregator eventAggregator, Action<INotification> callback)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
            this.Callback = callback;
        }

        #endregion constructors


        #region properties

        public Action<INotification> Callback { get; set; }

        #endregion properties


        #region methods

        public override IWindowInteraction CreateWindowInteraction()
        {
            return new EditTilesetInteraction(this.eventAggregator, this.Callback);
        }

        public override IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            return new EditTilesetInteraction(this.eventAggregator, callback);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(EditTilesetInteraction).Equals(type);
        }

        #endregion methods
    }
}
