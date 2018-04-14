﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Docks.LayerListDock
{
    public class LayerListViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor & destructer

        // TODO add layer editing properties
        public LayerListViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.Title = "Layer List";
            this.eventAggregator = eventAggregator;

            this.NewLayerCommand = new DelegateCommand(() => NewTilesetLayer());
            this.NewLayerGroupCommand = new DelegateCommand(() => NewLayerGroup());
            this.MergeLayerDownCommand = new DelegateCommand(() => MergeLayerDown());
            this.MergeLayerUpCommand = new DelegateCommand(() => MergeLayerUp());
            this.MoveLayerDownCommand = new DelegateCommand(() => MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => MoveLayerUp());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => RemoveLayer());
            this.ShowLayerCommand = new DelegateCommand(() => ShowLayer());
            this.LockLayerCommand = new DelegateCommand(() => LockLayer());
            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.LayerToMapSizeCommand = new DelegateCommand(() => LayerToMapSize());
            this.CurrentLayerChangedCommand = new DelegateCommand<object>((currentLayer) => CurrentLayerChanged((Layer)currentLayer));

            this.LayerList = new ObservableCollection<ILayer>();

            this.LayerList.Add(new Layer("Layer #1", 32, 32, 32, 32));
            this.LayerList.Add(new Layer("Layer #2", 32, 32, 32, 32));

            //ObservableCollection<ILayer> layerGroupLayers = new ObservableCollection<ILayer>();
            //layerGroupLayers.Add(new Layer("Layer #3", 32, 32, 32, 32));
            //ObservableCollection<ILayer> layerGroupLayers2 = new ObservableCollection<ILayer>();
            //layerGroupLayers2.Add(new Layer("Layer #4", 32, 32, 32, 32));
            //layerGroupLayers2.Add(new Layer("Layer #5", 32, 32, 32, 32));
            //layerGroupLayers.Add(new LayerGroup("Layer Group #2", layerGroupLayers2));
            //layerGroupLayers.Add(new Layer("Layer #6", 32, 32, 32, 32));
            //this.LayerList.Add(new LayerGroup("Layer Group #1", layerGroupLayers));
            //this.LayerList.Add(new Layer("Layer #7", 32, 32, 32, 32));
            
            //this.LayerList.Add(new Layer("Layer #3", 32, 32, 32, 32));
            //this.LayerList.Add(new Layer("Layer #4", 32, 32, 32, 32));
            //this.LayerList.Add(new Layer("Layer #5", 32, 32, 32, 32));
            //this.LayerList.Add(new Layer("Layer #6", 32, 32, 32, 32));

            this.eventAggregator.GetEvent<NewLayerEvent>().Subscribe(AddTilesetLayerMessage);
            this.eventAggregator.GetEvent<EditLayerEvent>().Subscribe(EditLayerMessageRecieved);
        }

        #endregion constructor & destructer


        #region properties

        public ICommand NewLayerCommand { get; private set; }
        public ICommand NewLayerGroupCommand { get; private set; }
        public ICommand MergeLayerDownCommand { get; private set; }
        public ICommand MergeLayerUpCommand { get; private set; }
        public ICommand MoveLayerDownCommand { get; private set; }
        public ICommand MoveLayerUpCommand { get; private set; }
        public ICommand DuplicateLayerCommand { get; private set; }
        public ICommand RemoveLayerCommand { get; private set; }
        public ICommand ShowLayerCommand { get; private set; }
        public ICommand LockLayerCommand { get; private set; }
        public ICommand EditPropertiesCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand LayerToMapSizeCommand { get; private set; }
        public ICommand CurrentLayerChangedCommand { get; private set; }

        public override DockType DockType
        {
            get
            {
                return DockType.LayerList;
            }
        }

        public ObservableCollection<ILayer> LayerList { get; set; }
        public Layer CurrentLayer { get; set; }

        #endregion properties


        #region methods

        public void AddTilesetLayerMessage(NewLayerMessage message)
        {
            AddTilesetLayer(message.Layer);
        }

        public void EditLayerMessageRecieved(EditLayerMessage message)
        {
            Layer editedLayer = message.Layer;
            int currentLayerIndex = this.LayerList.IndexOf(this.CurrentLayer);
            this.LayerList.RemoveAt(currentLayerIndex);
            this.LayerList.Insert(currentLayerIndex, editedLayer);
        }

        public void AddTilesetLayer(Layer layer)
        {
            this.LayerList.Add(layer);
            RaisePropertyChanged(nameof(this.LayerList));
        }

        public void NewTilesetLayer()
        {
            // TODO pass layer count
            OpenWindowMessage openWindowMessage = new OpenWindowMessage(WindowType.Layer);

            openWindowMessage.WindowTitle = "New Layer";
            int layerCount = GetLayerCount() + 1;
            String newLayerName = String.Format("Layer #{0}", layerCount);

            openWindowMessage.Content = new Layer(newLayerName, 32, 32, 32, 32);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(openWindowMessage);
        }

        public void NewLayerGroup()
        {
            int layerGroupCount = GetLayerGroupCount() + 1;
            String newLayerGroupName = String.Format("Layer Group #{0}", layerGroupCount);
            this.LayerList.Add(new LayerGroup(newLayerGroupName));
        }

        public void MergeLayerDown()
        {
            Console.WriteLine("Merge layer down");
        }

        public void MergeLayerUp()
        {
            Console.WriteLine("Merge layer up");
        }

        public void MoveLayerDown()
        {
            int currentLayerIndex = this.LayerList.IndexOf(this.CurrentLayer);
            if (currentLayerIndex < this.LayerList.Count - 1)
            {
                this.LayerList.Move(currentLayerIndex, currentLayerIndex + 1);
            }
        }

        public void MoveLayerUp()
        {
            int currentLayerIndex = this.LayerList.IndexOf(this.CurrentLayer);
            if (currentLayerIndex > 0)
            {
                this.LayerList.Move(currentLayerIndex, currentLayerIndex - 1);
            }
        }

        public void DuplicateLayer()
        {
            int currentLayerIndex = this.LayerList.IndexOf(this.CurrentLayer);
            Layer layerCopy = Infrastructure.Utils.Utils.DeepClone<Layer>(this.CurrentLayer);
            this.LayerList.Insert(currentLayerIndex + 1, layerCopy);
        }

        public void RemoveLayer()
        {
            this.LayerList.Remove(this.CurrentLayer);
        }

        public void EditProperties()
        {
            OpenWindowMessage openWindowMessage = new OpenWindowMessage(WindowType.Layer);
            openWindowMessage.WindowTitle = "Edit Layer";
            openWindowMessage.Content = this.CurrentLayer;
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(openWindowMessage);
        }

        public void ShowLayer()
        {
            Console.WriteLine("Show layer");
        }

        public void LockLayer()
        {
            Console.WriteLine("Lock layer");
        }

        public void EditCollisions()
        {
            Console.WriteLine("Edit Collisions");
        }

        public void LayerToMapSize()
        {
            Console.WriteLine("Layer To Map Size");
        }
        
        public void CurrentLayerChanged(Layer layer)
        {
            this.CurrentLayer = layer;
        }

        private int GetLayerGroupCount()
        {
            int layerGroupCount = 0;
            foreach (ILayer layer in this.LayerList)
            {
                if (layer is LayerGroup)
                {
                    layerGroupCount++;
                }
            }
            return layerGroupCount;
        }

        private int GetLayerCount()
        {
            int layerCount = 0;
            foreach (ILayer layer in this.LayerList)
            {
                if (layer is Layer)
                {
                    layerCount++;
                }
            }
            return layerCount;
        }

        #endregion methods
    }
}
