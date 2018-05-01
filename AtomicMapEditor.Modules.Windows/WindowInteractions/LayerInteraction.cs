using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.LayerEditorWindow;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.WindowInteractions
{
    public class LayerInteraction : IWindowInteraction
    {
        #region fields

        private ILayer layer;
        private InteractionRequest<INotification> mapWindowInteraction;

        #endregion fields


        #region Constructor

        public LayerInteraction(ILayer layer)
        {
            this.layer = layer;
            this.mapWindowInteraction = new InteractionRequest<INotification>();
        }

        #endregion Constructor


        #region Properties

        #endregion Properties


        #region methods

        public void RaiseNotification(DependencyObject test, Action<INotification> callback)
        {
            Confirmation layerWindowConfirmation = new Confirmation();

            layerWindowConfirmation.Title = string.Format("Edit Layer - {0}", layer.LayerName);
            layerWindowConfirmation.Content = layer;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            trigger.SourceObject = this.mapWindowInteraction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(test);
            this.mapWindowInteraction.Raise(layerWindowConfirmation, callback);
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
