using System;
using System.Windows.Input;
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


        #region constructor & destructer

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

        #endregion constructor & destructer


        #region properties

        public ICommand SetMapPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }

        public string WindowTitle { get; set; }
        public string Name { get; set; }
        public int BaseWidth { get; set; }
        public int BaseHeight { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public ScaleType Scale { get; set; }
        public int PixelScale { get; set; }
        public string Description { get; set; }

        public Confirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as Confirmation;
                this.Map = this.notification.Content as Map;
                updateUIusingMap();
                RaisePropertyChanged(nameof(this.Notification));
            }
        }
        public Action FinishInteraction { get; set; }

        private Map Map { get; set; }

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
                case ScaleType.Pixel:
                    map.setWidth(this.BaseWidth);
                    map.setHeight(this.BaseHeight);
                    break;

                case ScaleType.Tile:
                    map.setWidthTiles(this.BaseWidth, this.TileWidth);
                    map.setHeightTiles(this.BaseHeight, this.TileHeight);
                    break;

                default:
                    map.setWidth(this.BaseWidth);
                    map.setHeight(this.BaseHeight);
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
            this.BaseWidth = map.getTileWidth();
            this.BaseHeight = map.getTileHeight();
            this.TileWidth = map.TileWidth;
            this.TileHeight = map.TileHeight;
            this.Scale = map.Scale;
            this.PixelScale = map.PixelScale;
            this.Description = map.Description;
        }

        #endregion methods
    }
}
