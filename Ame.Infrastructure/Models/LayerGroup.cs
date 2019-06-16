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
    public class LayerGroup : BindableBase, ILayer
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerGroup(string layerGroupName)
        {
            this.Name = layerGroupName;
            this.Layers = new ObservableCollection<ILayer>();
            this.Layers.CollectionChanged += LayersChanged;
        }

        public LayerGroup(string layerGroupName, ObservableCollection<ILayer> layers)
        {
            this.Name = layerGroupName;
            this.Layers = layers;
            this.Layers.CollectionChanged += LayersChanged;
        }

        #endregion constructor


        #region properties

        public string Name { get; set; }
        public bool IsImmutable { get; set; }
        public bool IsVisible { get; set; }
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

        #endregion properties


        #region methods

        public int GetPixelWidth()
        {
            int pixelWidth = 0;
            foreach (ILayer layer in this.Layers)
            {
                // TODO incorporate offset
                pixelWidth = Math.Max(pixelWidth, layer.GetPixelWidth());
            }
            return pixelWidth;
        }

        public int GetPixelHeight()
        {
            int pixelHeight = 0;
            foreach (ILayer layer in this.Layers)
            {
                // TODO incorporate offset
                pixelHeight = Math.Max(pixelHeight, layer.GetPixelHeight());
            }
            return pixelHeight;
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
        
        #endregion methods
    }
}
