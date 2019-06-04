using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Exceptions;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ame.App.Wpf.UI.Interactions.MapProperties
{
    public class EditMapInteraction : IWindowInteraction
    {
        #region fields

        #endregion fields


        #region Constructor

        public EditMapInteraction()
        {
        }

        public EditMapInteraction(Map map)
        {
            this.Map = map;
        }

        public EditMapInteraction(Action<INotification> callback)
        {
            this.Callback = callback;
        }

        public EditMapInteraction(Map map, Action<INotification> callback)
        {
            this.Map = map;
            this.Callback = callback;
        }

        #endregion Constructor


        #region Properties

        public Map Map { get; set; }
        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }

        #endregion Properties


        #region methods

        public void UpdateMissingContent(AmeSession session)
        {
            this.Map = this.Map ?? session.CurrentMap;
            this.Title = string.Format("Edit Map - {0}", this.Map.Name);
        }

        public void RaiseNotification(DependencyObject parent)
        {
            if (this.Map == null)
            {
                throw new InteractionConfigurationException("Map is null");
            }
            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Content = this.Map;
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
            action.WindowContent = new EditMapWindow();

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
