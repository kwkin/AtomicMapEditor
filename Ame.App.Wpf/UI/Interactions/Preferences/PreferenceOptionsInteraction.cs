using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ame.App.Wpf.UI.Interactions.Preferences
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
        public double Width { get; set; } = 620.0;
        public double Height { get; set; } = 380.0;

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
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, this.Height));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, this.Height));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
