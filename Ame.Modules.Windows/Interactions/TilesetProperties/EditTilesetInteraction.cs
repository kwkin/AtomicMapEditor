﻿using System;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Exceptions;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.TilesetProperties
{
    public class EditTilesetInteraction : IWindowInteraction
    {
        #region fields

        #endregion fields


        #region Constructor

        public EditTilesetInteraction()
        {
        }

        public EditTilesetInteraction(TilesetModel tileset)
        {
            this.TilesetModel = tileset;
        }

        public EditTilesetInteraction(Action<INotification> callback)
        {
            this.Callback = callback;
        }

        public EditTilesetInteraction(TilesetModel tileset, Action<INotification> callback)
        {
            this.TilesetModel = tileset;
            this.Callback = callback;
        }

        #endregion Constructor


        #region Properties

        public TilesetModel TilesetModel { get; set; }
        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }

        #endregion Properties


        #region methods

        public void UpdateMissingContent(AmeSession session)
        {
            this.TilesetModel = this.TilesetModel ?? session.CurrentTileset;
            this.Title = this.Title ?? string.Format("Tileset Properties - {0}", this.TilesetModel.Name);
        }

        public void RaiseNotification(DependencyObject parent)
        {
            if (this.TilesetModel == null)
            {
                throw new InteractionConfigurationException("TilesetModel is null");
            }
            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Title = this.Title;
            mapConfirmation.Content = this.TilesetModel;

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
            action.WindowContent = new TilesetPropertiesWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 380.0));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, 420.0));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 380.0));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
