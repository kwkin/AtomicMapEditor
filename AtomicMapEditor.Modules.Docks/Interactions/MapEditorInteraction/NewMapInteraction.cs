using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapEditorInteraction
{
    public class NewMapInteraction : IWindowInteraction
    {
        #region fields

        private AmeSession session;
        private IEventAggregator eventAggregator;
        private InteractionRequest<INotification> interaction;
        private Action<INotification> callback;

        #endregion fields


        #region Constructor

        public NewMapInteraction(AmeSession session, IEventAggregator eventAggregator) : this(session, eventAggregator, null)
        {
        }

        public NewMapInteraction(AmeSession session, IEventAggregator eventAggregator, Action<INotification> callback)
        {
            this.session = session;
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
            RaiseNotification(parent, this.callback);
        }

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            Confirmation mapConfirmation = new Confirmation();
            int mapCount = this.session.MapList.Count;
            string newMapName = string.Format("Map #{0}", mapCount);
            mapConfirmation.Content = new Map(newMapName);
            mapConfirmation.Title = "New Map";

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
