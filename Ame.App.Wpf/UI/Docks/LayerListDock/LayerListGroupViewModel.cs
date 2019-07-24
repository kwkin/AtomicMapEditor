﻿using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListGroupViewModel : ILayerListEntryViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IAmeSession session;

        #endregion fields


        #region constructor

        public LayerListGroupViewModel(IEventAggregator eventAggregator, IAmeSession session, LayerGroup layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.layer = layer ?? throw new ArgumentNullException("layer");

            this.Layers = new ObservableCollection<ILayerListEntryViewModel>();

            this.layer.Layers.CollectionChanged += LayersChanged;

            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingGroup filled = new DrawingGroup();
            using (DrawingContext context = filled.Open())
            {
                Rect drawingRect = new Rect(0, 0, layer.GetPixelWidth(), layer.GetPixelHeight());
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
            drawingGroup.Children.Add(filled);
            drawingGroup.Children.Add(layer.Group);
            this.layerPreview = new DrawingImage(drawingGroup);
        }

        #endregion constructor


        #region properties

        public LayerGroup layer;
        public ILayer Layer
        {
            get
            {
                return this.layer;
            }
            set
            {
                if (typeof(LayerGroup).IsInstanceOfType(value))
                {
                    this.layer = value as LayerGroup;
                }
            }
        }

        private DrawingImage layerPreview;
        public DrawingImage LayerPreview
        {
            get
            {
                return this.layerPreview;
            }
        }

        public ObservableCollection<ILayerListEntryViewModel> Layers { get; private set; }

        #endregion properties


        #region methods

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in e.NewItems)
                    {
                        ILayerListEntryViewModel entry = LayerListEntryGenerator.Generate(this.eventAggregator, this.session, layer);
                        this.Layers.Add(entry);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ILayer layer in e.OldItems)
                    {
                        IEnumerable<ILayerListEntryViewModel> toRemove = new ObservableCollection<ILayerListEntryViewModel>(this.Layers.Where(entry => entry.Layer == layer));
                        foreach (ILayerListEntryViewModel entry in toRemove)
                        {
                            this.Layers.Remove(entry);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        ILayerListEntryViewModel entry = this.Layers[oldIndex];
                        this.Layers[oldIndex] = this.Layers[newIndex];
                        this.Layers[newIndex] = entry;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion methods
    }
}
