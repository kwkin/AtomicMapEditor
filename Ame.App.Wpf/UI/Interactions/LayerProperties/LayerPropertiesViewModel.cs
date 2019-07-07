using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Interactions.LayerProperties
{
    public class LayerPropertiesViewModel : IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerPropertiesViewModel()
        {
            this.WindowTitle.Value = "Layer Editor";

            this.SetLayerPropertiesCommand = new DelegateCommand(() => SetLayerProperties());
            this.CloseWindowCommand = new DelegateCommand(() => CloseWindow());
        }

        #endregion constructor


        #region properties

        public ICommand SetLayerPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand ApplyLayerPropertiesCommand { get; private set; }

        public BindableProperty<string> WindowTitle { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<string> Name { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<int> Rows { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> Columns { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> TileWidth { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> TileHeight { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> OffsetX { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> OffsetY { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty<ScaleType>.Prepare();

        public BindableProperty<LayerPosition> Position { get; set; } = BindableProperty<LayerPosition>.Prepare();

        public BindableProperty<double> ScrollRate { get; set; } = BindableProperty<double>.Prepare();

        public BindableProperty<string> Description { get; set; } = BindableProperty<string>.Prepare();

        public Confirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as Confirmation;
                this.Layer.Value = this.notification.Content as Layer;
                UpdateUI();
                if (this.Layer.Value != null)
                {
                    UpdateMetadata();
                }
            }
        }

        public MetadataHandler MetadataHandler { get; set; }

        public Action FinishInteraction { get; set; }

        public BindableProperty<Layer> Layer { get; set; } = BindableProperty<Layer>.Prepare();

        #endregion properties


        #region methods

        private void SetLayerProperties()
        {
            UpdateLayerProperties(this.Layer.Value);
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
            layer.Name.Value = this.Name.Value;
            layer.Columns.Value = this.Columns.Value;
            layer.Rows.Value = this.Rows.Value;
            layer.OffsetX.Value = this.OffsetX.Value * this.TileWidth.Value;
            layer.OffsetY.Value = this.OffsetY.Value * this.TileHeight.Value;
            layer.TileWidth.Value = this.TileWidth.Value;
            layer.TileHeight.Value = this.TileHeight.Value;
            layer.Scale.Value = this.Scale.Value;
            layer.Position.Value = this.Position.Value;
            layer.ScrollRate.Value = this.ScrollRate.Value;
            layer.Description.Value = this.Description.Value;
        }

        private void UpdateUI()
        {
            Layer layer = this.Layer.Value;
            this.Name.Value = layer.Name.Value;
            this.Columns.Value = layer.Columns.Value;
            this.Rows.Value = layer.Rows.Value;
            this.OffsetX.Value = layer.OffsetX.Value / layer.TileWidth.Value;
            this.OffsetY.Value = layer.OffsetY.Value / layer.TileHeight.Value;
            this.TileWidth.Value = layer.TileWidth.Value;
            this.TileHeight.Value = layer.TileHeight.Value;
            this.Scale.Value = layer.Scale.Value;
            this.Position.Value = layer.Position.Value;
            this.ScrollRate.Value = layer.ScrollRate.Value;
            this.Description.Value = layer.Description.Value;
        }

        private void UpdateMetadata()
        {
            ObservableCollection<MetadataProperty> properties = new ObservableCollection<MetadataProperty>();
            properties.AddRange(MetadataPropertyUtils.GetPropertyList(this.Layer.Value));

            this.MetadataHandler = new MetadataHandler(this.Layer.Value, properties);
        }


        #endregion methods
    }
}
