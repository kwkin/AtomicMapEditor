using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.Preferences
{
    public class PreferenceOptionsInteraction : IWindowInteraction
    {
        #region fields

        #endregion fields


        #region Constructor

        public PreferenceOptionsInteraction()
        {
        }

        public PreferenceOptionsInteraction(Action<INotification> callback)
        {
            this.Callback = callback;
        }

        #endregion Constructor


        #region Properties

        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }

        #endregion Properties


        #region methods

        public void UpdateMissingContent(AmeSession session)
        {
            this.Title = "Preferences";
        }

        public void RaiseNotification(DependencyObject parent)
        {
            Confirmation confirmation = new Confirmation();
            confirmation.Title = this.Title;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            InteractionRequest<INotification> interaction = new InteractionRequest<INotification>();
            trigger.SourceObject = interaction;
            trigger.Actions.Add(CreateAction());
            trigger.Attach(parent);
            interaction.Raise(confirmation, this.Callback);
        }

        private PopupWindowAction CreateAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new PreferencesMenu();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 620.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 380.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 620.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 380.0));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
