using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.LayerEditorWindow
{
    public class NewLayerInteraction : IWindowInteraction
    {
        #region fields

        private ILayer layer;

        private IEventAggregator eventAggregator;
        private InteractionRequest<INotification> interaction;

        #endregion fields


        #region Constructor

        public NewLayerInteraction(ILayer layer, IEventAggregator eventAggregator)
        {
            this.layer = layer;
            this.eventAggregator = eventAggregator;
            this.interaction = new InteractionRequest<INotification>();
        }

        #endregion Constructor


        #region Properties

        #endregion Properties


        #region methods

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            Confirmation layerWindowConfirmation = new Confirmation();

            layerWindowConfirmation.Title = string.Format("Edit Layer - {0}", layer.LayerName);
            layerWindowConfirmation.Content = layer;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            trigger.SourceObject = this.interaction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(parent);
            this.interaction.Raise(layerWindowConfirmation, callback);
        }

        public void OnWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Layer layerModel = confirmation.Content as Layer;

                NewLayerMessage newLayerMessage = new NewLayerMessage(layerModel);
                this.eventAggregator.GetEvent<NewLayerEvent>().Publish(newLayerMessage);
            }
        }

        private PopupWindowAction GetAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new LayerEditor();

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
