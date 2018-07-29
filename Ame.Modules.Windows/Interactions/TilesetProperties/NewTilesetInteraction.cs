﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Messages.Interactions;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.TilesetProperties
{
    public class NewTilesetInteraction : IWindowInteraction
    {
        #region fields

        private TilesetModel tilesetModel;
        private AmeSession session;

        #endregion fields


        #region Constructor

        public NewTilesetInteraction()
        {
        }

        public NewTilesetInteraction(Action<INotification> callback)
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
            this.Title = "New Tileset";
            string newTilesetName = string.Format("Tileset #{0}", session.CurrentTilesetCount);
            this.tilesetModel = new TilesetModel(newTilesetName);
            this.Callback = this.Callback ?? OnNewTilesetWindowClosed;
        }

        public void RaiseNotification(DependencyObject parent)
        {
            Confirmation tilesetConfirmation = new Confirmation();
            tilesetConfirmation.Title = this.Title;

            TilesetInteractionMessage message = new TilesetInteractionMessage(this.tilesetModel, false);
            tilesetConfirmation.Content = message;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            InteractionRequest<INotification> interaction = new InteractionRequest<INotification>();
            trigger.SourceObject = interaction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(parent);
            interaction.Raise(tilesetConfirmation, this.Callback);
        }

        private PopupWindowAction GetAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new TilesetPropertiesWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 720.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 450.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 720.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 450.0));
            action.WindowStyle = style;
            return action;
        }

        private void OnNewTilesetWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                TilesetInteractionMessage message = confirmation.Content as TilesetInteractionMessage;
                TilesetModel tilesetModel = message.Tileset as TilesetModel;
                this.session.CurrentTilesetList.Add(tilesetModel);
            }
        }

        #endregion methods
    }
}