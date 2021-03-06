﻿using Ame.Infrastructure.BaseTypes;
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
            : this(map, null)
        {
        }

        public EditMapInteraction(Action<INotification> callback)
            : this(null, callback)
        {
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
        public double Width { get; set; } = 420.0;
        public double Height { get; set; } = 400.0;

        #endregion Properties


        #region methods

        public void UpdateMissingContent(IAmeSession session)
        {
            this.Map = this.Map ?? session.CurrentMap.Value;
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
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, this.Height));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, this.Height));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
