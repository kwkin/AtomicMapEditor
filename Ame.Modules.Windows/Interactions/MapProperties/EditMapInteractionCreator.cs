using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapProperties
{
    public class EditMapInteractionCreator : WindowInteractionCreatorTemplate
    {
        #region fields

        #endregion fields


        #region constructors

        public EditMapInteractionCreator(AmeSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.Session = session;
        }

        public EditMapInteractionCreator(AmeSession session, Action<INotification> callback)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.Session = session;
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
            return new EditMapInteraction(this.Session, callback);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(EditMapInteraction).Equals(type);
        }

        #endregion methods
    }
}
