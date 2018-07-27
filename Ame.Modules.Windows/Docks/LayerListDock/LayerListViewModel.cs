using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Ame.Modules.Windows.Interactions.LayerProperties;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.LayerListDock
{
    public class LayerListViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public LayerListViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.Title = "Layer List";
            this.Session = session;
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
            this.CurrentLayerChangedCommand = new DelegateCommand<object>((currentLayer) => CurrentLayerChanged((ILayer)currentLayer));

            this.eventAggregator.GetEvent<NewLayerEvent>().Subscribe(AddTilesetLayerMessage);
        }

        #endregion constructor


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

        public AmeSession Session { get; set; }

        #endregion properties


        #region methods

        public void AddTilesetLayerMessage(NewLayerMessage message)
        {
            AddTilesetLayer(message.Layer);
        }

        public void AddTilesetLayer(ILayer layer)
        {
            this.Session.CurrentLayerList.Add(layer);
        }

        public void NewTilesetLayer()
        {
            OpenWindowMessage openWindowMessage = new OpenWindowMessage(typeof(NewLayerInteraction));
            openWindowMessage.Title = "New Layer";
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(openWindowMessage);
        }

        public void NewLayerGroup()
        {
            int layerGroupCount = GetLayerGroupCount();
            string newLayerGroupName = string.Format("Layer Group #{0}", layerGroupCount);
            this.Session.CurrentLayerList.Add(new LayerGroup(newLayerGroupName));
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
            int currentLayerIndex = this.Session.CurrentLayerList.IndexOf(this.Session.CurrentLayer);
            if (currentLayerIndex < this.Session.CurrentLayerList.Count - 1 && currentLayerIndex >= 0)
            {
                this.Session.CurrentLayerList.Move(currentLayerIndex, currentLayerIndex + 1);
            }
        }

        public void MoveLayerUp()
        {
            int currentLayerIndex = this.Session.CurrentLayerList.IndexOf(this.Session.CurrentLayer);
            if (currentLayerIndex > 0)
            {
                this.Session.CurrentLayerList.Move(currentLayerIndex, currentLayerIndex - 1);
            }
        }

        public void DuplicateLayer()
        {
            ILayer copiedLayer = Utils.DeepClone<ILayer>(this.Session.CurrentLayer);
            AddTilesetLayer(copiedLayer);
        }

        public void RemoveLayer()
        {
            if (this.Session.CurrentLayer == null)
            {
                return;
            }
            this.Session.CurrentLayerList.Remove(this.Session.CurrentLayer);
        }

        public void EditProperties()
        {
            if (this.Session.CurrentLayer == null)
            {
                return;
            }
            OpenWindowMessage openWindowMessage = new OpenWindowMessage(typeof(EditLayerInteraction));
            openWindowMessage.Title = string.Format("Edit Layer - {0}", this.Session.CurrentLayer.LayerName);
            openWindowMessage.Content = this.Session.CurrentLayer;
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

        public void CurrentLayerChanged(ILayer layer)
        {
            this.Session.CurrentLayer = layer;
        }

        private int GetLayerGroupCount()
        {
            IEnumerable<LayerGroup> groups = this.Session.CurrentLayerList.OfType<LayerGroup>();
            return groups.Count<LayerGroup>();
        }

        private int GetLayerCount()
        {
            IEnumerable<Layer> groups = this.Session.CurrentLayerList.OfType<Layer>();
            return groups.Count<Layer>();
        }

        #endregion methods
    }
}
