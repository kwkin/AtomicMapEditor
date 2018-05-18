using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapPropertiesInteraction
{
    public class NewMapInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        private AmeSession session;
        private IEventAggregator eventAggregator;
        private Action<INotification> callback;

        #endregion fields


        #region constructors

        public NewMapInteractionCreator(AmeSession session, IEventAggregator eventAggregator)
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

        public NewMapInteractionCreator(AmeSession session, IEventAggregator eventAggregator, Action<INotification> callback)
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
            return new NewMapInteraction(this.session, this.eventAggregator, this.callback);
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            return new NewMapInteraction(this.session, this.eventAggregator, callback);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(NewMapInteraction).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(AmeSession).Equals(type))
            {
                this.session = value as AmeSession;
            }
            else if (typeof(IEventAggregator).Equals(type))
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
