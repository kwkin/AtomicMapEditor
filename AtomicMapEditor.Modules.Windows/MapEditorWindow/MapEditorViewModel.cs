using System;
using System.Windows.Input;
using AtomicMapEditor.Infrastructure.Models;
using AtomicMapEditor.Infrastructure.Requests;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace AtomicMapEditor.Modules.Windows.MapEditorWindow
{
    public class MapEditorViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        private IEventAggregator ea;

        #endregion fields


        #region constructor & destructer

        public MapEditorViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            this.ea = eventAggregator;
            this.WindowTitle = "New Map";

            this.ApplyMapPropertiesCommand = new DelegateCommand(ApplyMapProperties);
            this.SetMapPropertiesCommand = new DelegateCommand(SetMapProperties);
            this.CloseWindowCommand = new DelegateCommand(CloseWindow);
        }

        #endregion constructor & destructer


        #region properties

        public ICommand ApplyMapPropertiesCommand { get; set; }
        public ICommand CancelCommand { get; set; }
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

        public MapWindowConfirmation _Notification { get; set; }
        public INotification Notification
        {
            get { return _Notification; }
            set
            {
                this._Notification = value as MapWindowConfirmation;
                this.Map = this._Notification.Map;
                updateUIusingMap();
                RaisePropertyChanged(nameof(this.Notification));
            }
        }
        public Action FinishInteraction { get; set; }

        private MapModel Map { get; set; }

        #endregion properties


        #region methods

        private void SetMapProperties()
        {
            UpdateMapProperties(this.Map);
            if (_Notification != null)
            {
                _Notification.Confirmed = true;
            }
            FinishInteraction();
        }

        private void ApplyMapProperties()
        {
            UpdateMapProperties(this.Map);
        }

        private void CloseWindow()
        {
            if (_Notification != null)
            {
                _Notification.Confirmed = false;
            }
            FinishInteraction();
        }

        private void UpdateMapProperties(MapModel map)
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
                    this.Map.setHeight(this.BaseHeight);
                    break;
            }
            this.Map.PixelScale = this.PixelScale;
            this.Map.Description = this.Description;

            // TODO send map message
        }

        private void updateUIusingMap()
        {
            this.Name = this.Map.Name;
            this.BaseWidth = this.Map.getTileWidth();
            this.BaseHeight = this.Map.getTileHeight();
            this.TileWidth = this.Map.TileWidth;
            this.TileHeight = this.Map.TileHeight;
            this.Scale = this.Map.Scale;
            this.PixelScale = this.Map.PixelScale;
            this.Description = this.Map.Description;
        }

        private void updateUIusingMap(MapModel map)
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
