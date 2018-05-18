using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapPropertiesInteraction
{
    public class EditMapInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        private AmeSession session;
        private Action<INotification> callback;

        #endregion fields


        #region constructors

        public EditMapInteractionCreator(AmeSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.session = session;
        }

        public EditMapInteractionCreator(AmeSession session, Action<INotification> callback)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.session = session;
            this.callback = callback;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return new EditMapInteraction(this.session, this.callback);
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            return new EditMapInteraction(this.session, callback);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditMapInteraction).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(AmeSession).Equals(type))
            {
                this.session = value as AmeSession;
            }
            else if (typeof(Action<INotification>).Equals(type))
            {
                this.callback = value as Action<INotification>;
            }
        }

        #endregion methods
    }
}
