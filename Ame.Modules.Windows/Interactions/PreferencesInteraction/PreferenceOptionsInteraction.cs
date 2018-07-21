using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.PreferencesInteraction
{
    public class PreferenceOptionsInteraction : IWindowInteraction
    {
        #region fields

        private IEventAggregator eventAggregator;
        private InteractionRequest<INotification> interaction;
        private Action<INotification> callback;

        #endregion fields


        #region Constructor

        public PreferenceOptionsInteraction(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.interaction = new InteractionRequest<INotification>();
        }

        public PreferenceOptionsInteraction(IEventAggregator eventAggregator, Action<INotification> callback)
        {
            this.eventAggregator = eventAggregator;
            this.interaction = new InteractionRequest<INotification>();
            this.callback = callback;
        }

        #endregion Constructor


        #region Properties

        #endregion Properties


        #region methods

        public void RaiseNotification(DependencyObject parent)
        {
            string title = "Preferences";
            RaiseNotification(parent, this.callback, title);
        }

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            string title = "Preferences";
            RaiseNotification(parent, callback, title);
        }

        public void RaiseNotification(DependencyObject parent, string title)
        {
            RaiseNotification(parent, this.callback, title);
        }

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback, string title)
        {
            Confirmation confirmation = new Confirmation();
            confirmation.Title = title;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            trigger.SourceObject = this.interaction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(parent);
            this.interaction.Raise(confirmation, callback);
        }

        private PopupWindowAction GetAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new PreferencesMenu();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(Window.MinWidthProperty, 620.0));
            style.Setters.Add(new Setter(Window.MinHeightProperty, 380.0));
            style.Setters.Add(new Setter(Window.WidthProperty, 620.0));
            style.Setters.Add(new Setter(Window.HeightProperty, 380.0));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
