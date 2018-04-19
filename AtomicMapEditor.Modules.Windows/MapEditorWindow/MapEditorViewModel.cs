using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.MapEditorWindow
{
    public class MapEditorViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor

        public MapEditorViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.WindowTitle = "New Map";

            this.SetMapPropertiesCommand = new DelegateCommand(SetMapProperties);
            this.CloseWindowCommand = new DelegateCommand(CloseWindow);
            this.MovePropertyUpCommand = new DelegateCommand(MovePropertyUp);
            this.MovePropertyDownCommand = new DelegateCommand(MovePropertyDown);
        }

        #endregion constructor


        #region properties

        public ICommand SetMapPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand MovePropertyUpCommand { get; private set; }
        public ICommand MovePropertyDownCommand { get; private set; }

        public string WindowTitle { get; set; }
        public string Name { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public ScaleType Scale { get; set; }
        public int PixelScale { get; set; }
        public string Description { get; set; }

        private Map Map { get; set; }

        public IConfirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as IConfirmation;
                this.Map = this.notification.Content as Map;
                updateUIusingMap();
                UpdateMetadata();
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView MapMetadata { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }
        public MetadataProperty SelectedMetadata { get; set; }

        public Action FinishInteraction { get; set; }

        #endregion properties


        #region methods

        private void SetMapProperties()
        {
            UpdateMapProperties(this.Map);
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
            map.Name = this.Name;
            map.Scale = this.Scale;
            switch (this.Scale)
            {
                case ScaleType.Tile:
                    map.Columns = this.Columns;
                    map.Rows = this.Rows;
                    map.TileHeight = this.TileHeight;
                    map.TileWidth = this.TileWidth;
                    break;

                case ScaleType.Pixel:
                default:
                    map.Columns = this.Columns;
                    map.Rows = this.Rows;
                    break;
            }
            map.PixelScale = this.PixelScale;
            map.Description = this.Description;
        }

        private void updateUIusingMap()
        {
            updateUIusingMap(this.Map);
        }

        private void updateUIusingMap(Map map)
        {
            this.Name = map.Name;
            this.Columns = map.Columns;
            this.Rows = map.Rows;
            this.TileWidth = map.TileWidth;
            this.TileHeight = map.TileHeight;
            this.Scale = map.Scale;
            this.PixelScale = map.PixelScale;
            this.Description = map.Description;
        }

        private void UpdateMetadata()
        {
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.Map);
            this.MetadataList.Add(new MetadataProperty("Layer Count", this.Map.LayerList.Count, MetadataType.Statistic));
            this.MapMetadata = new ListCollectionView(this.MetadataList);
            this.MapMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
        }

        private void MovePropertyUp()
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

        private void MovePropertyDown()
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
