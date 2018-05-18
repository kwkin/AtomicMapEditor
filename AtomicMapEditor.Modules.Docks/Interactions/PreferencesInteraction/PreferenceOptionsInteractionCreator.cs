using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.PreferencesInteraction
{
    public class PreferenceOptionsInteractionCreator : IWindowInteractionCreator
    {
        #region fields
        
        private IEventAggregator eventAggregator;
        private Action<INotification> callback;

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
            this.callback = callback;
        }

        #endregion Constructor


        #region Properties

        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return new PreferenceOptionsInteraction(this.eventAggregator, this.callback);
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            return new PreferenceOptionsInteraction(this.eventAggregator, callback);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(PreferenceOptionsInteraction).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(IEventAggregator).Equals(type))
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
