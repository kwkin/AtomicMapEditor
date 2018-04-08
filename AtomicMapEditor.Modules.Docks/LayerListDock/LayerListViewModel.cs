using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;

namespace Ame.Modules.Docks.LayerListDock
{
    public class LayerListViewModel : DockToolViewModelTemplate
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public LayerListViewModel()
        {
            this.Title = "Layer List";

            this.AddTilesetLayerCommand = new DelegateCommand(() => AddTilesetLayer());
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

            this.LayerList = new List<object>();
            this.LayerList.Add(new Layer("Layer #1", 32, 32));
            this.LayerList.Add(new Layer("Layer #2", 32, 32));

            IList<ILayer> layerGroupLayers = new List<ILayer>();
            layerGroupLayers.Add(new Layer("Layer #3", 32, 32));

            IList<ILayer> layerGroupLayers2 = new List<ILayer>();
            layerGroupLayers2.Add(new Layer("Layer #4", 32, 32));
            layerGroupLayers2.Add(new Layer("Layer #5", 32, 32));
            layerGroupLayers.Add(new LayerGroup("Layer Group #2", layerGroupLayers2));

            layerGroupLayers.Add(new Layer("Layer #6", 32, 32));
            this.LayerList.Add(new LayerGroup("Layer Group #1", layerGroupLayers));
            
            this.LayerList.Add(new Layer("Layer #7", 32, 32));
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

        public IList<object> LayerList { get; set; }

        #endregion properties


        #region methods

        public void AddTilesetLayer()
        {
            Console.WriteLine("Add tileset layer");
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
