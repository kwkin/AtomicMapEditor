using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerProperties
{
    public class NewLayerInteraction : IWindowInteraction
    {
        #region fields

        private Layer layer;
        private AmeSession session;

        #endregion fields


        #region Constructor

        public NewLayerInteraction()
        {
        }

        public NewLayerInteraction(Action<INotification> callback)
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
            Map currentMap = session.CurrentMap;
            this.Title = "New Layer";
            string newLayerName = string.Format("Layer #{0}", currentMap.LayerCount);
            this.layer = new Layer(newLayerName, currentMap.TileWidth, currentMap.TileHeight, currentMap.RowCount, currentMap.ColumnCount);
            this.Callback = this.Callback ?? OnNewLayerWindowClosed;
        }

        public void RaiseNotification(DependencyObject parent)
        {
            Confirmation layerWindowConfirmation = new Confirmation();
            layerWindowConfirmation.Title = this.Title;
            layerWindowConfirmation.Content = this.layer;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            InteractionRequest<INotification> interaction = new InteractionRequest<INotification>();
            trigger.SourceObject = interaction;
            trigger.Actions.Add(CreateAction());
            trigger.Attach(parent);
            interaction.Raise(layerWindowConfirmation, this.Callback);
        }

        private PopupWindowAction CreateAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new NewLayerWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 400.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 400.0));
            action.WindowStyle = style;
            return action;
        }

        private void OnNewLayerWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Layer layer = confirmation.Content as Layer;
                this.session.CurrentLayerList.Add(layer);
            }
        }

        #endregion methods
    }
}
