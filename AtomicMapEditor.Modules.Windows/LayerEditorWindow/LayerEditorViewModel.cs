using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
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


        #region constructor

        public LayerEditorViewModel()
        {
            this.WindowTitle = "Layer Editor";

            this.SetLayerPropertiesCommand = new DelegateCommand(SetLayerProperties);
            this.CloseWindowCommand = new DelegateCommand(CloseWindow);
        }

        #endregion constructor


        #region properties

        public ICommand SetLayerPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand ApplyLayerPropertiesCommand { get; private set; }
        
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public ScaleType Scale { get; set; }
        public LayerPosition Position { get; set; }
        public double ScrollRate { get; set; }
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
                UpdateMetadata();
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public Action FinishInteraction { get; set; }

        private Layer Layer { get; set; }
        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView LayerMetadata { get; set; }

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
            layer.Columns = this.Columns;
            layer.Rows = this.Rows;
            layer.OffsetX = this.OffsetX;
            layer.OffsetY = this.OffsetY;
            layer.TileWidth = this.TileWidth;
            layer.TileHeight = this.TileHeight;
            layer.Scale = this.Scale;
            layer.Position = this.Position;
            layer.ScrollRate = this.ScrollRate;
            layer.Description = this.Description;
        }

        private void updateUI()
        {
            this.Name = this.Layer.LayerName;
            this.Columns = this.Layer.Columns;
            this.Rows = this.Layer.Rows;
            this.OffsetX = this.Layer.OffsetX;
            this.OffsetY = this.Layer.OffsetY;
            this.TileWidth = this.Layer.TileWidth;
            this.TileHeight = this.Layer.TileHeight;
            this.Scale = this.Layer.Scale;
            this.Position = this.Layer.Position;
            this.ScrollRate = this.Layer.ScrollRate;
            this.Description = this.Layer.Description;
        }

        private void UpdateMetadata()
        {
            IList metadataList = MetadataPropertyUtils.GetPropertyList(this.Layer);
            this.LayerMetadata = new ListCollectionView(metadataList);
            this.LayerMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
        }


        #endregion methods
    }
}
