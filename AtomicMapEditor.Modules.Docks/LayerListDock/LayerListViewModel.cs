using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
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
        // TODO add layer renaming by double clicking/right click option
        public LayerListViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.Title = "Layer List";
            this.eventAggregator = eventAggregator;

            this.AddTilesetLayerCommand = new DelegateCommand(() => NewTilesetLayer());
            this.AddLayerGroupCommand = new DelegateCommand(() => AddLayerGroup());
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
            this.NewLayerCommand = new DelegateCommand(() => NewLayer());

            this.LayerList = new ObservableCollection<object>();
            //this.LayerList.Add(new Layer("Layer #1", 32, 32));
            //this.LayerList.Add(new Layer("Layer #2", 32, 32));

            //IList<ILayer> layerGroupLayers = new List<ILayer>();
            //layerGroupLayers.Add(new Layer("Layer #3", 32, 32));

            //IList<ILayer> layerGroupLayers2 = new List<ILayer>();
            //layerGroupLayers2.Add(new Layer("Layer #4", 32, 32));
            //layerGroupLayers2.Add(new Layer("Layer #5", 32, 32));
            //layerGroupLayers.Add(new LayerGroup("Layer Group #2", layerGroupLayers2));

            //layerGroupLayers.Add(new Layer("Layer #6", 32, 32));
            //this.LayerList.Add(new LayerGroup("Layer Group #1", layerGroupLayers));

            //this.LayerList.Add(new Layer("Layer #7", 32, 32));

            this.eventAggregator.GetEvent<NewLayerEvent>().Subscribe(AddTilesetLayerMessage);
        }

        #endregion constructor & destructer


        #region properties

        public ICommand AddTilesetLayerCommand { get; private set; }
        public ICommand AddImageLayerCommand { get; private set; }
        public ICommand AddLayerGroupCommand { get; private set; }
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
        public ICommand NewLayerCommand { get; private set; }

        public override DockType DockType
        {
            get
            {
                return DockType.LayerList;
            }
        }

        public ObservableCollection<object> LayerList { get; set; }

        #endregion properties


        #region methods
        
        public void AddTilesetLayerMessage(NewLayerMessage message)
        {
            AddTilesetLayer(message.Layer);
        }

        public void AddTilesetLayer(Layer layer)
        {
            this.LayerList.Add(layer);
            RaisePropertyChanged(nameof(this.LayerList));
        }

        public void NewTilesetLayer()
        {
            OpenWindowMessage window = new OpenWindowMessage(WindowType.Layer);
            window.WindowTitle = "New Layer";
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(window);
        }

        public void AddLayerGroup()
        {
            Console.WriteLine("Add layer group");
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
            Console.WriteLine("Move layer down");
        }

        public void MoveLayerUp()
        {
            Console.WriteLine("Move layer up");
        }

        public void DuplicateLayer()
        {
            Console.WriteLine("Duplicate layer");
        }

        public void RemoveLayer()
        {
            Console.WriteLine("Remove layer");
        }

        public void EditProperties()
        {
            Console.WriteLine("Edit Properties");
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

        public void NewLayer()
        {
            Console.WriteLine("New layer");
        }

        #endregion methods
    }
}
