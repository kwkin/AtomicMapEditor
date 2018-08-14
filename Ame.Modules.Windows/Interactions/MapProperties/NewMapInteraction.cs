using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Modules.MapEditor.Editor;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.MapProperties
{
    public class NewMapInteraction : IWindowInteraction
    {
        #region fields

        private Map map;
        private AmeSession session;

        #endregion fields


        #region Constructor

        public NewMapInteraction()
        {
        }

        public NewMapInteraction(Action<INotification> callback)
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
            this.session = session;
            this.Title = "New Map";
            string newMapeName = string.Format("Map #{0}", session.MapCount);
            this.map = new Map(newMapeName);
            this.Callback = this.Callback ?? OnNewMapWindowClosed;
        }

        public void RaiseNotification(DependencyObject parent)
        {
            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Content = this.map;
            mapConfirmation.Title = this.Title;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            InteractionRequest<INotification> interaction = new InteractionRequest<INotification>();
            trigger.SourceObject = interaction;
            trigger.Actions.Add(CreateAction());
            trigger.Attach(parent);
            interaction.Raise(mapConfirmation, this.Callback);
        }

        private PopupWindowAction CreateAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new NewMapWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 475.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 475.0));
            action.WindowStyle = style;
            return action;
        }

        private void OnNewMapWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Map mapModel = confirmation.Content as Map;
                OpenDockMessage openEditorMessage = new OpenDockMessage(typeof(MapEditorViewModel), mapModel);
                this.EventAggregator.GetEvent<OpenDockEvent>().Publish(openEditorMessage);
            }
        }

        #endregion methods
    }
}
