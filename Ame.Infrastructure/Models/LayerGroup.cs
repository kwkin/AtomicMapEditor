using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class LayerGroup : ILayer
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerGroup(string layerGroupName)
            : this(layerGroupName, new ObservableCollection<ILayer>())
        {
        }

        public LayerGroup(string layerGroupName, ObservableCollection<ILayer> layers)
        {
            this.Name.Value = layerGroupName;
            this.Layers = layers;

            this.Rows = BindableProperty.Prepare<int>();
            this.Columns = BindableProperty.Prepare<int>();
            this.OffsetX = BindableProperty.Prepare<int>();
            this.OffsetY = BindableProperty.Prepare<int>();

            this.Layers.CollectionChanged += LayersChanged;

            this.pixelWidth.Value = GetPixelWidth();
            this.pixelHeight.Value = GetPixelHeight();
            this.Columns.PropertyChanged += UpdatePixelWidth;
            this.Rows.PropertyChanged += UpdatePixelHeight;
        }

        #endregion constructor


        #region properties

        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>("wassai");

        public BindableProperty<bool> IsImmutable { get; set; } = BindableProperty.Prepare<bool>();

        public BindableProperty<bool> IsVisible { get; set; } = BindableProperty.Prepare<bool>();

        public ObservableCollection<ILayer> Layers { get; set; }

        private DrawingGroup group;
        public DrawingGroup Group
        {
            get
            {
                if (group == null)
                {
                    group = new DrawingGroup();
                }
                return group;
            }
            set
            {
                this.Group = value;
            }
        }

        public LayerGroup Parent { get; set; }

        public BindableProperty<int> columns;
        public BindableProperty<int> Columns
        {
            get
            {
                this.columns.Value = GetColumns();
                return this.columns;
            }
            set
            {
                this.columns = value;
            }
        }

        public BindableProperty<int> rows;
        public BindableProperty<int> Rows
        {
            get
            {
                this.rows.Value = GetRows();
                return this.rows;
            }
            set
            {
                this.rows = value;
            }
        }

        public BindableProperty<int> offsetX;
        public BindableProperty<int> OffsetX
        {
            get
            {
                this.offsetX.Value = GetOffsetX();
                return this.offsetX;
            }
            set
            {
                this.offsetX = value;
            }
        }

        public BindableProperty<int> offsetY;
        public BindableProperty<int> OffsetY
        {
            get
            {
                this.offsetY.Value = GetOffsetY();
                return this.offsetY;
            }
            set
            {
                this.offsetY = value;
            }
        }

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

        #endregion properties


        #region methods

        public virtual int GetPixelWidth()
        {
            int leftmost = this.OffsetX.Value;
            int rightmost = leftmost;
            foreach (ILayer layer in this.Layers)
            {
                int width = layer.PixelWidth.Value;
                rightmost = Math.Max(rightmost, width + layer.OffsetX.Value);
            }
            int pixelWidth = rightmost - leftmost;
            return pixelWidth;
        }

        public virtual int GetPixelHeight()
        {
            int topmost = this.OffsetY.Value;
            int bottommost = topmost;
            foreach (ILayer layer in this.Layers)
            {
                int height = layer.PixelHeight.Value;
                bottommost = Math.Max(bottommost, height + layer.OffsetY.Value);
            }
            int pixelHeight = bottommost - topmost;
            return pixelHeight;
        }

        public void AddSibling(ILayer layer)
        {
            this.Layers.Add(layer);
            layer.Parent = this;
        }

        public int GetLayerCount()
        {
            int totalLayers = 0;
            IEnumerable<ILayer> layers = this.Layers.Where(layer => typeof(Layer).IsInstanceOfType(layer));
            totalLayers += layers.Count();

            IEnumerable<ILayer> groups = this.Layers.Where(layer => typeof(LayerGroup).IsInstanceOfType(layer));
            foreach (LayerGroup group in groups)
            {
                totalLayers += group.GetLayerCount();
            }
            return totalLayers;
        }

        public int GetLayerGroupCount()
        {
            int totalGroups = 0;

            IEnumerable<ILayer> groups = this.Layers.Where(layer => typeof(LayerGroup).IsInstanceOfType(layer));
            totalGroups += groups.Count();
            foreach (LayerGroup group in groups)
            {
                totalGroups += group.GetLayerGroupCount();
            }
            return totalGroups;
        }

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in e.NewItems)
                    {
                        int index = e.NewStartingIndex;
                        this.Group.Children.Insert(index, layer.Group);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ILayer layer in e.OldItems)
                    {
                        this.Group.Children.Remove(layer.Group);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        Drawing entry = this.Group.Children[oldIndex];
                        this.Group.Children[oldIndex] = this.Group.Children[newIndex];
                        this.Group.Children[newIndex] = entry;
                    }
                    break;

                default:
                    break;
            }
            UpdatePixelWidth();
            UpdatePixelHeight();
        }

        protected void UpdatePixelWidth(object sender, PropertyChangedEventArgs e)
        {
            this.pixelWidth.Value = GetPixelWidth();
        }

        protected void UpdatePixelWidth()
        {
            this.pixelWidth.Value = GetPixelWidth();
        }

        protected void UpdatePixelHeight(object sender, PropertyChangedEventArgs e)
        {
            this.pixelHeight.Value = GetPixelHeight();
        }

        protected void UpdatePixelHeight()
        {
            this.pixelHeight.Value = GetPixelHeight();
        }

        private int GetColumns()
        {
            return GetPixelHeight();
        }

        private int GetRows()
        {
            return GetPixelWidth();
        }

        private int GetOffsetX()
        {
            int offsetX;
            if (this.Layers.Count > 0)
            {
                offsetX = this.Layers[0].OffsetX.Value;
                for (int index = 1; index < this.Layers.Count; ++index)
                {
                    ILayer layer = this.Layers[index];
                    offsetX = Math.Min(offsetX, layer.OffsetX.Value);
                }
            }
            else
            {
                offsetX = 0;
            }
            return offsetX;
        }

        private int GetOffsetY()
        {
            int offsetY;
            if (this.Layers.Count > 0)
            {
                offsetY = this.Layers[0].OffsetY.Value;
                for (int index = 1; index < this.Layers.Count; ++index)
                {
                    ILayer layer = this.Layers[index];
                    offsetY = Math.Min(offsetY, layer.OffsetY.Value);
                }
            }
            else
            {
                offsetY = 0;
            }
            return offsetY;
        }

        #endregion methods
    }
}
