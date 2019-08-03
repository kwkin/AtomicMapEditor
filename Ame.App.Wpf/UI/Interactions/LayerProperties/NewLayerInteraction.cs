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
        private IAmeSession session;

        #endregion fields


        #region Constructor

        public NewLayerInteraction()
        {
        }

        public NewLayerInteraction(Map map)
            : this(map, null)
        {
        }

        public NewLayerInteraction(Action<INotification> callback)
            : this(null, callback)
        {
        }

        public NewLayerInteraction(Map map, Action<INotification> callback)
        {
            this.Map = map;
            this.Callback = callback;
        }

        #endregion Constructor



        #region Properties

        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }
        public double Width { get; set; } = 420.0;
        public double Height { get; set; } = 480.0;
        public Map Map { get; set; }

        #endregion Properties


        #region methods

        public void UpdateMissingContent(IAmeSession session)
        {
            this.session = session;
            this.Map = this.Map ?? session.CurrentMap.Value;
            this.Title = "New Layer";
            string newLayerName = string.Format("Layer #{0}", this.Map.GetLayerCount());
            this.layer = new Layer(this.Map, newLayerName, this.Map.TileWidth.Value, this.Map.TileHeight.Value, this.Map.Rows.Value, this.Map.Columns.Value);
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
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, this.Height));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, this.Height));
            action.WindowStyle = style;
            return action;
        }

        private void OnNewLayerWindowClosed(INotification notification)
        {
            // TODO change current layer on the layer list
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Layer layer = confirmation.Content as Layer;
                this.Map.CurrentLayer.Value.AddLayerOnto(layer);

                this.Map.CurrentLayer.Value = layer;
            }
        }

        #endregion methods
    }
}
