using System;
using System.Windows.Input;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.LayerEditorWindow
{
    public class LayerEditorViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public LayerEditorViewModel()
        {
            this.WindowTitle = "Layer Editor";

            this.SetLayerPropertiesCommand = new DelegateCommand(SetLayerProperties);
            this.CloseWindowCommand = new DelegateCommand(CloseWindow);
        }

        #endregion constructor & destructer


        #region properties

        public ICommand SetLayerPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand ApplyLayerPropertiesCommand { get; private set; }
        
        public string Name { get; set; }
        public int BaseWidth { get; set; }
        public int BaseHeight { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public ScaleType Scale { get; set; }
        public LayerPosition Position { get; set; }
        public int ScrollRateFrom { get; set; }
        public int ScrollRateTo { get; set; }
        public string Description { get; set; }

        public string WindowTitle { get; set; }

        public Confirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as Confirmation;
                this.Layer = this.notification.Content as Layer;
                updateUI();
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public Action FinishInteraction { get; set; }

        private Layer Layer { get; set; }

        #endregion properties


        #region methods

        private void SetLayerProperties()
        {
            UpdateLayerProperties(this.Layer);
            if (this.notification != null)
            {
                this.notification.Confirmed = true;
            }
            FinishInteraction();
        }

        private void CloseWindow()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = false;
            }
            FinishInteraction();
        }

        private void UpdateLayerProperties(Layer layer)
        {
            layer.LayerName = this.Name;

            // TODO fix the scaling
            // TODO look into using the datatemplate for displaying and modifying
            layer.Columns = this.BaseWidth;
            layer.Rows = this.BaseHeight;
            layer.OffsetX = this.OffsetX;
            layer.OffsetY = this.OffsetY;
            layer.TileWidth = this.TileWidth;
            layer.TileHeight = this.TileHeight;
            layer.Scale = this.Scale;
            layer.Position = this.Position;
            layer.ScrollRateFrom = this.ScrollRateFrom;
            layer.ScrollRateTo = this.ScrollRateTo;
            layer.Description = this.Description;
        }

        private void updateUI()
        {
            this.Name = this.Layer.LayerName;
            this.BaseWidth = this.Layer.Columns;
            this.BaseHeight = this.Layer.Rows;
            this.OffsetX = this.Layer.OffsetX;
            this.OffsetY = this.Layer.OffsetY;
            this.TileWidth = this.Layer.TileWidth;
            this.TileHeight = this.Layer.TileHeight;
            this.Scale = this.Layer.Scale;
            this.Position = this.Layer.Position;
            this.ScrollRateFrom = this.Layer.ScrollRateFrom;
            this.ScrollRateTo = this.Layer.ScrollRateTo;
            this.Description = this.Layer.Description;
        }

        #endregion methods
    }
}
