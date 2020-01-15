using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class Layer : ILayer, IContainsMetadata
    {
        #region fields

        #endregion fields


        #region constructor

        public Layer(Map map)
            : this(map, "", 32, 32, 32, 32)
        {
        }

        public Layer(string name)
            : this(null, name, 32, 32, 32, 32)
        {
        }

        public Layer(Map map, string name)
            : this(map, name, 32, 32, 32, 32)
        {
        }

        public Layer(Map map, int tileWidth, int tileHeight, int rows, int columns)
            : this(map, "", tileWidth, tileHeight, rows, columns)
        {
        }

        public Layer(string layerName, int tileWidth, int tileHeight, int rows, int columns)
            : this(null, layerName, tileWidth, tileHeight, rows, columns)
        {
        }

        public Layer(Map map, string layerName, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Map.Value = map;
            this.Name.Value = layerName;
            this.TileWidth.Value = tileWidth;
            this.TileHeight.Value = tileHeight;
            this.Rows.Value = rows;
            this.Columns.Value = columns;

            this.Parent = map;
            this.IsVisible.Value = true;
            this.Position.Value = LayerPosition.Base;
            this.Scale.Value = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
            this.CustomProperties = new ObservableCollection<MetadataProperty>();

            this.pixelWidth.Value = GetPixelWidth();
            this.pixelHeight.Value = GetPixelHeight();
            this.TileWidth.PropertyChanged += UpdatePixelWidth;
            this.TileHeight.PropertyChanged += UpdatePixelHeight;
            this.Columns.PropertyChanged += LayerSizeChanged;
            this.Rows.PropertyChanged += LayerSizeChanged;
            this.OffsetX.PropertyChanged += LayerPositionChanged;
            this.OffsetY.PropertyChanged += LayerPositionChanged;
        }

        #endregion constructor


        #region properties

        public int ID { get; set; } = -1;

        public BindableProperty<Map> Map { get; set; } = BindableProperty.Prepare<Map>();

        [MetadataProperty(MetadataType.Property, "Name")]
        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<int> Columns { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<int> Rows { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public BindableProperty<int> TileWidth { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public BindableProperty<int> TileHeight { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Pixel Offset X")]
        public BindableProperty<int> OffsetX { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Pixel Offset Y")]
        public BindableProperty<int> OffsetY { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<LayerPosition> Position { get; set; } = BindableProperty.Prepare<LayerPosition>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty.Prepare<ScaleType>();

        [MetadataProperty(MetadataType.Property, "Scroll Rate")]
        public BindableProperty<double> ScrollRate { get; set; } = BindableProperty.Prepare<double>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<string> Description { get; set; } = BindableProperty.Prepare<string>();

        public BindableProperty<bool> IsImmutable { get; set; } = BindableProperty.Prepare<bool>();

        public BindableProperty<bool> IsVisible { get; set; } = BindableProperty.Prepare<bool>();

        private BindableProperty<int> pixelWidth = BindableProperty.Prepare<int>();
        private ReadOnlyBindableProperty<int> pixelWidthReadOnly;
        public ReadOnlyBindableProperty<int> PixelWidth
        {
            get
            {
                this.pixelWidthReadOnly = this.pixelWidthReadOnly ?? this.pixelWidth.ReadOnlyProperty();
                return this.pixelWidthReadOnly;
            }
        }

        private BindableProperty<int> pixelHeight = BindableProperty.Prepare<int>();
        private ReadOnlyBindableProperty<int> pixelHeightReadOnly;
        public ReadOnlyBindableProperty<int> PixelHeight
        {
            get
            {
                this.pixelHeightReadOnly = this.pixelHeightReadOnly ?? this.pixelHeight.ReadOnlyProperty();
                return this.pixelHeightReadOnly;
            }
        }

        [IgnoreNodeBuilder]
        public DrawingGroup Group
        {
            get
            {
                return this.TileIDs.Group;
            }

            set
            {
                this.TileIDs.Group = value;
            }
        }

        [IgnoreNodeBuilder]
        public DrawingCollection LayerItems
        {
            get
            {
                DrawingCollection children = null;
                if (this.TileIDs.Group != null)
                {
                    children = this.TileIDs.Group.Children;
                }
                return children;
            }
        }

        public TileCollection TileIDs { get; set; }

        public ILayerParent Parent { get; set; }

        public ObservableCollection<MetadataProperty> CustomProperties { get; set; }

        #endregion properties


        #region methods

        public int GetPixelWidth()
        {
            return this.TileWidth.Value * this.Columns.Value;
        }

        public int GetPixelHeight()
        {
            return this.TileHeight.Value * this.Rows.Value;
        }

        public Rect GetBounds()
        {
            return new Rect(this.OffsetX.Value, this.OffsetY.Value, this.GetPixelWidth(), this.GetPixelHeight());
        }

        public Rect GetBoundsExclusive()
        {
            Rect test = new Rect(this.OffsetX.Value + 1, this.OffsetY.Value + 1, this.GetPixelWidth() - 2, this.GetPixelHeight() - 2);
            return test;
        }

        public void Clear()
        {
            this.TileIDs.Clear();
        }

        public Point GetPointFromIndex(int id)
        {
            int pointX = (id % this.Columns.Value) * this.TileWidth.Value;
            int pointY = (int)Math.Floor((double)(id / this.Rows.Value)) * this.TileHeight.Value;
            return new Point(pointX, pointY);
        }

        public void AddLayerAbove(ILayer layer)
        {
            layer.Parent.Layers.Remove(layer);
            layer.Parent = this.Parent;

            int thisIndex = this.Parent.Layers.IndexOf(this);
            this.Parent.Layers.Insert(thisIndex, layer);
        }

        public void AddLayerOnto(ILayer layer)
        {
            AddLayerAbove(layer);
        }

        public void AddLayerBelow(ILayer layer)
        {
            layer.Parent.Layers.Remove(layer);
            layer.Parent = this.Parent;

            int thisIndex = this.Parent.Layers.IndexOf(this);
            int insertIndex = thisIndex + 1;
            if (insertIndex > this.Parent.Layers.Count - 1)
            {
                this.Parent.Layers.Add(layer);
            }
            else
            {
                this.Parent.Layers.Insert(insertIndex, layer);
            }
        }

        private void LayerSizeChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TileIDs.Resize(0, 0);
            this.pixelHeight.Value = GetPixelHeight();
            this.pixelWidth.Value = GetPixelWidth();
        }

        private void LayerPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TileIDs.Move(this.OffsetX.Value, this.OffsetY.Value);
        }

        protected void UpdatePixelWidth(object sender, PropertyChangedEventArgs e)
        {
            this.pixelWidth.Value = GetPixelWidth();
        }

        protected void UpdatePixelHeight(object sender, PropertyChangedEventArgs e)
        {
            this.pixelHeight.Value = GetPixelHeight();
        }

        #endregion methods
    }
}
