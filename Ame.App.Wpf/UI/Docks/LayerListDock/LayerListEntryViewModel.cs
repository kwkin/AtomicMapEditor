﻿using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
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
using System.Windows.Input;
using System.Windows.Media;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListEntryViewModel : ILayerListEntryViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IAmeSession session;

        DrawingGroup drawingGroup = new DrawingGroup();
        DrawingGroup filled = new DrawingGroup();

        #endregion fields


        #region constructor

        public LayerListEntryViewModel(IEventAggregator eventAggregator, IAmeSession session, Layer layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.Layer = layer ?? throw new ArgumentNullException("layer");

            drawingGroup = new DrawingGroup();
            filled = new DrawingGroup();

            drawingGroup.Children.Add(filled);
            drawingGroup.Children.Add(layer.Group);
            this.layerPreview = new DrawingImage(drawingGroup);

            RefreshPreview();

            layer.PixelHeight.PropertyChanged += LayerSizeChanged;
            layer.PixelWidth.PropertyChanged += LayerSizeChanged;

            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.MoveLayerDownCommand = new DelegateCommand(() => MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => MoveLayerUp());
            this.LayerToMapSizeCommand = new DelegateCommand(() => LayerToMapSize());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => RemoveLayer());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
        }

        #endregion constructor


        #region properties
        public ICommand EditPropertiesCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand LayerToMapSizeCommand { get; private set; }
        public ICommand MoveLayerDownCommand { get; private set; }
        public ICommand MoveLayerUpCommand { get; private set; }
        public ICommand DuplicateLayerCommand { get; private set; }
        public ICommand RemoveLayerCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }

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

        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);

        #endregion properties


        #region methods

        public void MoveLayerDown()
        {
            int currentLayerIndex = this.session.CurrentLayers.Value.IndexOf(this.Layer);
            if (currentLayerIndex < this.session.CurrentLayers.Value.Count - 1 && currentLayerIndex >= 0)
            {
                this.session.CurrentLayers.Value.Move(currentLayerIndex, currentLayerIndex + 1);
            }
        }

        public void MoveLayerUp()
        {
            int currentLayerIndex = this.session.CurrentLayers.Value.IndexOf(this.layer);
            if (currentLayerIndex > 0)
            {
                this.session.CurrentLayers.Value.Move(currentLayerIndex, currentLayerIndex - 1);
            }
        }

        public void DuplicateLayer()
        {
            ILayer copiedLayer = Utils.DeepClone<ILayer>(this.session.CurrentLayer.Value);
            this.Layer.AddSibling(copiedLayer);
        }

        public void RemoveLayer()
        {
            this.session.CurrentLayers.Value.Remove(this.Layer);
        }

        public void EditProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction(this.Layer);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void EditCollisions()
        {
            Console.WriteLine("Edit Collisions");
        }

        public void LayerToMapSize()
        {
            Console.WriteLine("Layer To Map Size");
        }

        public void RefreshPreview()
        {
            using (DrawingContext context = filled.Open())
            {
                Rect drawingRect = new Rect(0, 0, layer.PixelWidth.Value, layer.PixelHeight.Value);
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
        }

        private void LayerSizeChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshPreview();
        }

        private void EditTextbox()
        {
            this.IsEditingName.Value = true;
        }

        private void StopEditingTextbox()
        {
            this.IsEditingName.Value = false;
        }

        #endregion methods
    }
}
