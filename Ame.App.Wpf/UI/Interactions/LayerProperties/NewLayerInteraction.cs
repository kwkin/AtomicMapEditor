using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Interactions.LayerProperties
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
            this.layer = new Layer(newLayerName, currentMap.TileWidth, currentMap.TileHeight, currentMap.Rows, currentMap.Columns);
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
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 480.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 480.0));
            action.WindowStyle = style;
            return action;
        }

        private void OnNewLayerWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Layer layer = confirmation.Content as Layer;
                this.session.CurrentMap.LayerList.Add(layer);
            }
        }

        #endregion methods
    }
}
