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
    public class LayerPropertiesViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerPropertiesViewModel()
        {
            this.WindowTitle = "Layer Editor";

            this.SelectedMetadata.PropertyChanged += SelectedMetadataChanged;

            this.SetLayerPropertiesCommand = new DelegateCommand(() => SetLayerProperties());
            this.CloseWindowCommand = new DelegateCommand(() => CloseWindow());
            this.AddCustomMetaDataCommand = new DelegateCommand(() => AddCustomProperty());
            this.RemoveCustomMetadataCommand = new DelegateCommand(() => SetLayerProperties());
            this.MoveMetadataUpCommand = new DelegateCommand(() => MoveMetadataUp());
            this.MoveMetadataDownCommand = new DelegateCommand(() => MoveMetadataDown());
        }

        #endregion constructor


        #region properties

        public ICommand SetLayerPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand ApplyLayerPropertiesCommand { get; private set; }
        public ICommand AddCustomMetaDataCommand { get; private set; }
        public ICommand RemoveCustomMetadataCommand { get; private set; }
        public ICommand MoveMetadataUpCommand { get; private set; }
        public ICommand MoveMetadataDownCommand { get; private set; }

        public string WindowTitle { get; set; }
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        private ScaleType scale;
        public ScaleType Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                SetProperty(ref this.scale, value);
            }
        }

        public LayerPosition Position { get; set; }
        public double ScrollRate { get; set; }
        public string Description { get; set; }

        public Confirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as Confirmation;
                this.Layer = this.notification.Content as Layer;
                UpdateUI();
                UpdateMetadata();
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public Action FinishInteraction { get; set; }

        private Layer Layer { get; set; }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView LayerMetadata { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }

        public BindableProperty<MetadataProperty> SelectedMetadata { get; set; } = BindableProperty<MetadataProperty>.Prepare();

        public bool isCustomSelected;
        public bool IsCustomSelected
        {
            get
            {
                return isCustomSelected;
            }
            set
            {
                SetProperty(ref this.isCustomSelected, value);
            }
        }

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
            layer.Name.Value = this.Name;
            layer.Columns.Value = this.Columns;
            layer.Rows.Value = this.Rows;
            layer.OffsetX = this.OffsetX * this.TileWidth;
            layer.OffsetY = this.OffsetY * this.TileHeight;
            layer.TileWidth.Value = this.TileWidth;
            layer.TileHeight.Value = this.TileHeight;
            layer.Scale.Value = this.Scale;
            layer.Position.Value = this.Position;
            layer.ScrollRate.Value = this.ScrollRate;
            layer.Description.Value = this.Description;
        }

        private void UpdateUI()
        {
            this.Name = this.Layer.Name.Value;
            this.Columns = this.Layer.Columns.Value;
            this.Rows = this.Layer.Rows.Value;
            this.OffsetX = this.Layer.OffsetX / this.Layer.TileWidth.Value;
            this.OffsetY = this.Layer.OffsetY / this.Layer.TileHeight.Value;
            this.TileWidth = this.Layer.TileWidth.Value;
            this.TileHeight = this.Layer.TileHeight.Value;
            this.Scale = this.Layer.Scale.Value;
            this.Position = this.Layer.Position.Value;
            this.ScrollRate = this.Layer.ScrollRate.Value;
            this.Description = this.Layer.Description.Value;

            RaisePropertyChanged(nameof(this.Name));
        }

        private void SelectedMetadataChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsCustomSelected = this.SelectedMetadata.Value.Type == MetadataType.Custom ? true : false;
        }

        private void UpdateMetadata()
        {
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.Layer);
            this.LayerMetadata = new ListCollectionView(this.MetadataList);
            this.LayerMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            foreach (MetadataProperty property in this.Layer.CustomProperties)
            {
                this.MetadataList.Add(property);
            }
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            string customName = string.Format("Custom #{0}", customCount);
            MetadataProperty property = new MetadataProperty(customName, "", MetadataType.Custom);
            this.Layer.CustomProperties.Add(property);
            this.MetadataList.Add(property);
        }

        private void MoveMetadataUp()
        {
            int currentIndex = this.LayerMetadata.CurrentPosition;
            MetadataProperty currentItem = this.LayerMetadata.CurrentItem as MetadataProperty;
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int lowestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    lowestIndex = propertyIndex;
                    break;

                case MetadataType.Statistic:
                    lowestIndex = statisticIndex;
                    break;

                case MetadataType.Custom:
                    lowestIndex = customIndex;
                    break;

                default:
                    break;
            }
            if (currentIndex > lowestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex - 1);
                this.LayerMetadata.Refresh();
            }
        }

        private void MoveMetadataDown()
        {
            int currentIndex = this.LayerMetadata.CurrentPosition;
            MetadataProperty currentItem = this.LayerMetadata.CurrentItem as MetadataProperty;
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int highestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    highestIndex = statisticIndex - 1;
                    break;

                case MetadataType.Statistic:
                    highestIndex = customIndex - 1;
                    break;

                case MetadataType.Custom:
                    highestIndex = this.MetadataList.Count - 1;
                    break;

                default:
                    break;
            }
            if (currentIndex < highestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex + 1);
                this.LayerMetadata.Refresh();
            }
        }


        #endregion methods
    }
}
