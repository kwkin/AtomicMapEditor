using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Modules.Windows.PreferencesWindow;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.WindowInteractions
{
    // TODO move all factories and interactions to the corresponding window folder
    // Declare window core namespace containing interfaces and other common elements
    public class PreferenceOptionsInteraction : IWindowInteraction
    {
        #region fields
        
        private IEventAggregator eventAggregator;
        private InteractionRequest<INotification> interaction;

        #endregion fields


        #region Constructor

        public PreferenceOptionsInteraction(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.interaction = new InteractionRequest<INotification>();
        }

        #endregion Constructor


        #region Properties

        #endregion Properties


        #region methods

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            Confirmation confirmation = new Confirmation();
            confirmation.Title = "Preferences";

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            trigger.SourceObject = this.interaction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(parent);
            this.interaction.Raise(confirmation, callback);
        }

        public void OnWindowClosed(INotification notification)
        {

        }

        private PopupWindowAction GetAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new PreferencesMenu();

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
