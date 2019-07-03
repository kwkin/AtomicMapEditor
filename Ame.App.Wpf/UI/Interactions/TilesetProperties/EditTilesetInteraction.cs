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

namespace Ame.App.Wpf.UI.Interactions.TilesetProperties
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
        public double Width { get; set; } = 720.0;
        public double Height { get; set; } = 480.0;

        #endregion Properties


        #region methods

        public void UpdateMissingContent(AmeSession session)
        {
            this.TilesetModel = this.TilesetModel ?? session.CurrentTileset;
            this.Title = this.Title ?? string.Format("Tileset Properties - {0}", this.TilesetModel.Name.Value);
        }

        public void RaiseNotification(DependencyObject parent)
        {
            if (this.TilesetModel == null)
            {
                throw new InteractionConfigurationException("TilesetModel is null");
            }
            Confirmation tilesetConfirmation = new Confirmation();
            tilesetConfirmation.Title = this.Title;
            tilesetConfirmation.Content = this.TilesetModel;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            InteractionRequest<INotification> interaction = new InteractionRequest<INotification>();
            trigger.SourceObject = interaction;
            trigger.Actions.Add(CreateAction());
            trigger.Attach(parent);
            interaction.Raise(tilesetConfirmation, this.Callback);
        }

        private PopupWindowAction CreateAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new EditTilesetWindow();

            // TODO get these properties via a static method in the view model
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
