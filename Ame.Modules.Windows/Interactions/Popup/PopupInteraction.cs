﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.Popup
{
    public class PopupInteraction : IWindowInteraction
    {
        #region fields
        
        #endregion fields


        #region Constructor

        public PopupInteraction(FrameworkElement window)
        {
            this.Window = window;
        }

        public PopupInteraction(FrameworkElement window, Action<INotification> callback)
        {
            this.Window = window;
            this.Callback = callback;
        }


        #endregion Constructor


        #region Properties

        public string Title { get; set; }
        public string Message { get; set; }
        public FrameworkElement Window { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }

        #endregion Properties


        #region methods

        public void UpdateMissingContent(AmeSession session)
        {
        }

        public void RaiseNotification(DependencyObject parent)
        {
            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Content = this.Message;
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
            action.WindowContent = this.Window;

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 400.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 170.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 400.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 170.0));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
