using Ame.Infrastructure.BaseTypes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        {
            this.Name.Value = layerGroupName;
            this.Layers = new ObservableCollection<ILayer>();
            this.Layers.CollectionChanged += LayersChanged;
        }

        public LayerGroup(string layerGroupName, ObservableCollection<ILayer> layers)
        {
            this.Name.Value = layerGroupName;
            this.Layers = layers;
            this.Layers.CollectionChanged += LayersChanged;
        }

        #endregion constructor


        #region properties

        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);

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

        // TODO fix
        public BindableProperty<int> OffsetX { get; set; } = BindableProperty.Prepare<int>();

        public BindableProperty<int> OffsetY { get; set; } = BindableProperty.Prepare<int>();

        #endregion properties


        #region methods

        public int GetPixelWidth()
        {
            int leftmost = this.OffsetX.Value;
            int rightmost = leftmost;
            foreach (ILayer layer in this.Layers)
            {
                int width = layer.GetPixelWidth();
                rightmost = Math.Max(rightmost, width + layer.OffsetX.Value);
            }
            int pixelWidth = rightmost - leftmost;
            return pixelWidth;
        }

        public int GetPixelHeight()
        {
            int topmost = this.OffsetY.Value;
            int bottommost = topmost;
            foreach (ILayer layer in this.Layers)
            {
                int height = layer.GetPixelHeight();
                bottommost = Math.Max(bottommost, height + layer.OffsetY.Value);
            }
            int pixelHeight = bottommost - topmost;
            return pixelHeight;
        }

        public void AddWith(ILayer layer)
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
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(ILayer layer in e.NewItems)
                    {
                        int index = e.NewStartingIndex;
                        this.Group.Children.Insert(index, layer.Group);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach(ILayer layer in e.OldItems)
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
