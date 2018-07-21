using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerPropertiesInteraction
{
    public class NewLayerInteraction : IWindowInteraction
    {
        #region fields

        private ILayer layer;

        private IEventAggregator eventAggregator;
        private InteractionRequest<INotification> interaction;
        private Action<INotification> callback;

        #endregion fields


        #region Constructor

        public NewLayerInteraction(ILayer layer, IEventAggregator eventAggregator)
        {
            this.layer = layer;
            this.eventAggregator = eventAggregator;
            this.interaction = new InteractionRequest<INotification>();
        }

        public NewLayerInteraction(ILayer layer, IEventAggregator eventAggregator, Action<INotification> callback)
        {
            this.layer = layer;
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
            string title = string.Format("New Layer - {0}", layer.LayerName);
            RaiseNotification(parent, this.callback, title);
        }

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            string title = string.Format("New Layer - {0}", layer.LayerName);
            RaiseNotification(parent, callback, title);
        }

        public void RaiseNotification(DependencyObject parent, string title)
        {
            RaiseNotification(parent, this.callback, title);
        }

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback, string title)
        {
            Confirmation layerWindowConfirmation = new Confirmation();
            layerWindowConfirmation.Title = string.Format("New Layer - {0}", layer.LayerName);
            layerWindowConfirmation.Content = layer;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            trigger.SourceObject = this.interaction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(parent);
            this.interaction.Raise(layerWindowConfirmation, callback);
        }

        private PopupWindowAction GetAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new LayerPropertiesWindow();

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
