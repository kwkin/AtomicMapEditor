using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.LayerEditorInteraction
{
    public class EditLayerInteraction : IWindowInteraction
    {
        #region fields

        private ILayer layer;
        private InteractionRequest<INotification> interaction;

        #endregion fields


        #region Constructor

        public EditLayerInteraction(ILayer layer)
        {
            this.layer = layer;
            this.interaction = new InteractionRequest<INotification>();
        }

        #endregion Constructor


        #region Properties

        #endregion Properties


        #region methods

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            Confirmation confirmation = new Confirmation();

            confirmation.Title = string.Format("Edit Layer - {0}", layer.LayerName);
            confirmation.Content = layer;

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
