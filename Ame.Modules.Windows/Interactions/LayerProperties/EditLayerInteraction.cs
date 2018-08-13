using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Exceptions;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerProperties
{
    public class EditLayerInteraction : IWindowInteraction
    {
        #region fields

        #endregion fields


        #region Constructor

        public EditLayerInteraction()
        {
        }

        public EditLayerInteraction(ILayer layer)
        {
            this.Layer = layer;
        }

        public EditLayerInteraction(Action<INotification> callback)
        {
            this.Callback = callback;
        }

        public EditLayerInteraction(ILayer layer, Action<INotification> callback)
        {
            this.Layer = layer;
            this.Callback = callback;
        }

        #endregion Constructor


        #region Properties

        public ILayer Layer { get; set; }
        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }

        #endregion Properties


        #region methods

        public void UpdateMissingContent(AmeSession session)
        {
            this.Layer = this.Layer ?? session.CurrentLayer;
            this.Title = this.Title ?? string.Format("Edit Layer - {0}", this.Layer.LayerName);
        }

        public void RaiseNotification(DependencyObject parent)
        {
            if (this.Layer == null)
            {
                throw new InteractionConfigurationException("Layer is null");
            }
            Confirmation confirmation = new Confirmation();
            confirmation.Title = this.Title;
            confirmation.Content = this.Layer;

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
            action.WindowContent = new EditLayerWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 400.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 400.0));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
