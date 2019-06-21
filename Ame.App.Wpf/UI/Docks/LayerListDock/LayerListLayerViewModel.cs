﻿using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListLayerViewModel : ILayerListEntryViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public LayerListLayerViewModel(IEventAggregator eventAggregator, ILayer layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.Layer = layer ?? throw new ArgumentNullException("layer");

            // TODO bind sizes
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

        public ILayer layer;
        public ILayer Layer
        {
            get
            {
                return this.layer;
            }
            set
            {
                this.layer = value;
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


        #endregion properties


        #region methods

        #endregion methods
    }
}
