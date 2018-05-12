using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapEditorInteraction
{
    public class EditMapInteraction : IWindowInteraction
    {
        #region fields

        private AmeSession session;
        private InteractionRequest<INotification> interaction;
        private Action<INotification> callback;

        #endregion fields


        #region Constructor

        public EditMapInteraction(AmeSession session) : this(session, null)
        {
        }

        public EditMapInteraction(AmeSession session, Action<INotification> callback)
        {
            this.session = session;
            this.interaction = new InteractionRequest<INotification>();
            this.callback = callback;
        }

        #endregion Constructor


        #region Properties

        #endregion Properties


        #region methods

        public void RaiseNotification(DependencyObject parent)
        {
            RaiseNotification(parent, this.callback);
        }

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Content = this.session.CurrentMap;
            mapConfirmation.Title = string.Format("Edit Map - {0}", this.session.CurrentMap.Name);

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            trigger.SourceObject = this.interaction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(parent);
            this.interaction.Raise(mapConfirmation, callback);
        }

        private PopupWindowAction GetAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new MapEditorWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(Window.MinWidthProperty, 420.0));
            style.Setters.Add(new Setter(Window.MinHeightProperty, 380.0));
            style.Setters.Add(new Setter(Window.WidthProperty, 420.0));
            style.Setters.Add(new Setter(Window.HeightProperty, 380.0));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
