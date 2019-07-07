using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Interactions.MapProperties
{
    public class MapPropertiesViewModel : IInteractionRequestAware
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public MapPropertiesViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");

            this.WindowTitle.Value = "New Map";

            this.SetMapPropertiesCommand = new DelegateCommand(() => SetMapProperties());
            this.CloseWindowCommand = new DelegateCommand(() => CloseWindow());
        }

        #endregion constructor


        #region properties

        public ICommand SetMapPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }

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

        public MetadataHandler MetadataHandler { get; set; }

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
            ObservableCollection<MetadataProperty> properties = new ObservableCollection<MetadataProperty>();
            properties.Add(new MetadataProperty("Layer Count", this.Map.Value.Layers.Count, MetadataType.Statistic));

            this.MetadataHandler = new MetadataHandler(this.Map.Value, properties);
        }

        #endregion methods
    }
}
