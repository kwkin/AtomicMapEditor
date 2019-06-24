using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
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

namespace Ame.App.Wpf.UI.Interactions.MapProperties
{
    public class MapPropertiesViewModel : IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor

        public MapPropertiesViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.WindowTitle.Value = "New Map";

            this.SelectedMetadata.PropertyChanged += SelectedMetadataChanged;

            this.SetMapPropertiesCommand = new DelegateCommand(() => SetMapProperties());
            this.CloseWindowCommand = new DelegateCommand(() => CloseWindow());
            this.AddCustomMetaDataCommand = new DelegateCommand(() => AddCustomProperty());
            this.RemoveCustomMetadataCommand = new DelegateCommand(() => RemoveCustomProperty());
            this.MoveMetadataUpCommand = new DelegateCommand(() => MoveMetadataUp());
            this.MoveMetadataDownCommand = new DelegateCommand(() => MoveMetadataDown());
        }

        #endregion constructor


        #region properties

        public ICommand SetMapPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand AddCustomMetaDataCommand { get; private set; }
        public ICommand RemoveCustomMetadataCommand { get; private set; }
        public ICommand MoveMetadataUpCommand { get; private set; }
        public ICommand MoveMetadataDownCommand { get; private set; }

        public BindableProperty<string> WindowTitle { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<string> Name { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<int> Rows { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> Columns { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> TileWidth { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> TileHeight { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty<ScaleType>.Prepare();

        public BindableProperty<int> PixelRatio { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<string> Description { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<Map> Map { get; set; } = BindableProperty<Map>.Prepare();

        public IConfirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as IConfirmation;
                this.Map.Value = this.notification.Content as Map;
                updateUIusingMap();
                if (this.Map.Value != null)
                {
                    UpdateMetadata();
                }
            }
        }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView MapMetadata { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }

        public BindableProperty<MetadataProperty> SelectedMetadata { get; set; } = BindableProperty<MetadataProperty>.Prepare();

        public BindableProperty<bool> IsCustomSelected { get; set; } = BindableProperty<bool>.Prepare();

        public Action FinishInteraction { get; set; }

        #endregion properties


        #region methods

        private void SetMapProperties()
        {
            UpdateMapProperties(this.Map.Value);
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

        private void UpdateMapProperties(Map map)
        {
            map.Name.Value = this.Name.Value;
            map.Scale.Value = this.Scale.Value;
            switch (this.Scale.Value)
            {
                case ScaleType.Tile:
                    map.Columns.Value = this.Columns.Value;
                    map.Rows.Value = this.Rows.Value;
                    map.TileHeight.Value = this.TileHeight.Value;
                    map.TileWidth.Value = this.TileWidth.Value;
                    break;

                case ScaleType.Pixel:
                default:
                    map.Columns.Value = this.Columns.Value;
                    map.Rows.Value = this.Rows.Value;
                    break;
            }
            map.PixelScale.Value = this.PixelRatio.Value;
            map.Description.Value = this.Description.Value;
        }

        private void updateUIusingMap()
        {
            updateUIusingMap(this.Map.Value);
        }

        private void updateUIusingMap(Map map)
        {
            this.Name.Value = map.Name.Value;
            this.Columns.Value = map.Columns.Value;
            this.Rows.Value = map.Rows.Value;
            this.TileWidth.Value = map.TileWidth.Value;
            this.TileHeight.Value = map.TileHeight.Value;
            this.Scale.Value = map.Scale.Value;
            this.PixelRatio.Value = map.PixelScale.Value;
            this.Description.Value = map.Description.Value;
        }

        private void UpdateMetadata()
        {
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.Map.Value);
            this.MetadataList.Add(new MetadataProperty("Layer Count", this.Map.Value.LayerList.Count, MetadataType.Statistic));
            this.MapMetadata = new ListCollectionView(this.MetadataList);
            this.MapMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            foreach (MetadataProperty property in this.Map.Value.CustomProperties)
            {
                this.MetadataList.Add(property);
            }
        }

        private void SelectedMetadataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.SelectedMetadata.Value != null)
            {
                this.IsCustomSelected.Value = this.SelectedMetadata.Value.Type == MetadataType.Custom ? true : false;
            }
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            string customName = string.Format("Custom #{0}", customCount);
            MetadataProperty property = new MetadataProperty(customName, "", MetadataType.Custom);
            this.Map.Value.CustomProperties.Add(property);
            this.MetadataList.Add(property);
        }

        private void RemoveCustomProperty()
        {
            if (this.SelectedMetadata.Value.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(this.SelectedMetadata.Value);
            }
        }

        private void MoveMetadataUp()
        {
            int currentIndex = this.MapMetadata.CurrentPosition;
            MetadataProperty currentItem = this.MapMetadata.CurrentItem as MetadataProperty;
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
                this.MapMetadata.Refresh();
            }
        }

        private void MoveMetadataDown()
        {
            int currentIndex = this.MapMetadata.CurrentPosition;
            MetadataProperty currentItem = this.MapMetadata.CurrentItem as MetadataProperty;
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
                this.MapMetadata.Refresh();
            }
        }

        #endregion methods
    }
}
