using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.Interactions.LayerEditorInteraction
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
            this.AddCustomMetaDataCommand = new DelegateCommand(AddCustomProperty);
            this.RemoveCustomMetadataCommand = new DelegateCommand(RemoveCustomProperty);
            this.MoveMetadataUpCommand = new DelegateCommand(MoveMetadataUp);
            this.MoveMetadataDownCommand = new DelegateCommand(MoveMetadataDown);
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
        public ScaleType Scale { get; set; }
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
                updateUI();
                UpdateMetadata();
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public Action FinishInteraction { get; set; }

        private Layer Layer { get; set; }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView LayerMetadata { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }

        public MetadataProperty selectedMetadata;
        public MetadataProperty SelectedMetadata
        {
            get
            {
                return selectedMetadata;
            }
            set
            {
                this.IsCustomSelected = value.Type == MetadataType.Custom ? true : false;
                SetProperty(ref this.selectedMetadata, value);
            }
        }

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
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.Layer);
            this.LayerMetadata = new ListCollectionView(this.MetadataList);
            this.LayerMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            String customName = String.Format("Custom #{0}", customCount);
            this.MetadataList.Add(new MetadataProperty(customName, "", MetadataType.Custom));
        }

        private void RemoveCustomProperty()
        {
            if (this.SelectedMetadata.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(this.SelectedMetadata);
            }
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
