using System;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.PreferencesInteraction
{
    public class PreferenceOptionsInteractionCreator : WindowInteractionCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region Constructor

        public PreferenceOptionsInteractionCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
        }

        public PreferenceOptionsInteractionCreator(IEventAggregator eventAggregator, Action<INotification> callback)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
            this.Callback = callback;
        }

        #endregion Constructor


        #region Properties

        public Action<INotification> Callback { get; set; }

        #endregion Properties


        #region methods

        public override IWindowInteraction CreateWindowInteraction()
        {
            return CreateWindowInteraction(this.Callback);
        }

        public override IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            return new PreferenceOptionsInteraction(this.eventAggregator, callback);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(PreferenceOptionsInteraction).Equals(type);
        }

        #endregion methods
    }
}
