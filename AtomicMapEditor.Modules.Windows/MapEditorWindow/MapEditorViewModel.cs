using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
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
        }

        #endregion constructor


        #region properties

        public ICommand SetMapPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }

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
                this.MapProperties = PropertyStatUtils.GetPropertyList(this.Map);
                this.MapProperties.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView MapProperties { get; set; }

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

        #endregion methods
    }
}
