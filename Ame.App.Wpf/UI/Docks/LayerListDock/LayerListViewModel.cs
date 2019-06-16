using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public LayerListViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Session = session ?? throw new ArgumentNullException("session is null");

            this.Title = "Layer List";
            this.LayerList = new ObservableCollection<ILayerListEntryViewModel>();

            //Layer layer1a = new Layer("Layer 1a", 32, 32, 32, 32);
            //this.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer1a));
            //Layer layer1b = new Layer("Layer 1b", 32, 32, 32, 32);
            //this.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer1b));

            //LayerGroup layer1c = new LayerGroup("Layer 1c");
            //LayerListGroupViewModel group1c = new LayerListGroupViewModel(this.eventAggregator, layer1c);
            //this.LayerList.Add(group1c);
            //Layer layer2a = new Layer("Layer 2a", 32, 32, 32, 32);
            //group1c.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer2a));
            //Layer layer2b = new Layer("Layer 2b", 32, 32, 32, 32);
            //group1c.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer2b));

            //LayerGroup layer2c = new LayerGroup("Layer 2c");
            //LayerListGroupViewModel group2c = new LayerListGroupViewModel(this.eventAggregator, layer2c);
            //group1c.LayerList.Add(group2c);
            //Layer layer3a = new Layer("Layer 3a", 32, 32, 32, 32);
            //group2c.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer3a));
            //Layer layer3b = new Layer("Layer 3b", 32, 32, 32, 32);
            //group2c.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer3b));

            //Layer layer1d = new Layer("Layer 1d", 32, 32, 32, 32);
            //this.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer1d));

            this.Session.PropertyChanged += SessionUpdated;

            this.NewLayerCommand = new DelegateCommand(() => NewTilesetLayer());
            this.NewLayerGroupCommand = new DelegateCommand(() => NewLayerGroup());
            this.MoveLayerDownCommand = new DelegateCommand(() => MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => MoveLayerUp());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => RemoveLayer());
            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.LayerToMapSizeCommand = new DelegateCommand(() => LayerToMapSize());
            this.CurrentLayerChangedCommand = new DelegateCommand<object>((entry) => CurrentLayerChanged(entry as LayerListLayerViewModel));

            this.eventAggregator.GetEvent<NewLayerEvent>().Subscribe(AddTilesetLayerMessage);
        }

        #endregion constructor


        #region properties

        public ICommand NewLayerCommand { get; private set; }
        public ICommand NewLayerGroupCommand { get; private set; }
        public ICommand MoveLayerDownCommand { get; private set; }
        public ICommand MoveLayerUpCommand { get; private set; }
        public ICommand DuplicateLayerCommand { get; private set; }
        public ICommand RemoveLayerCommand { get; private set; }
        public ICommand EditPropertiesCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand LayerToMapSizeCommand { get; private set; }
        public ICommand CurrentLayerChangedCommand { get; private set; }

        public ObservableCollection<ILayerListEntryViewModel> LayerList { get; private set; }

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
            NewLayerInteraction interaction = new NewLayerInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void NewLayerGroup()
        {
            int layerGroupCount = GetLayerGroupCount();
            string newLayerGroupName = string.Format("Layer Group #{0}", layerGroupCount);
            this.Session.CurrentLayerList.Add(new LayerGroup(newLayerGroupName));
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
            EditLayerInteraction interaction = new EditLayerInteraction();
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

        public void CurrentLayerChanged(LayerListLayerViewModel layerEntry)
        {
            if (layerEntry != null)
            {
                this.Session.CurrentLayer = layerEntry.layer;
            }
        }

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void SessionUpdated(object d, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(AmeSession.CurrentMap):
                    this.LayerList.Clear();
                    this.Session.CurrentLayerList.CollectionChanged += UpdateLayerList;
                    foreach (ILayer layer in this.Session.CurrentLayerList)
                    {
                        this.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer));
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateLayerList(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(ILayer layer in e.NewItems)
                    {
                        this.LayerList.Add(new LayerListLayerViewModel(this.eventAggregator, layer));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach(ILayer layer in e.OldItems)
                    {
                        IEnumerable<ILayerListEntryViewModel> toRemove = new ObservableCollection<ILayerListEntryViewModel>(this.LayerList.Where(entry => entry.Layer == layer));
                        foreach (ILayerListEntryViewModel entry in toRemove)
                        {
                            this.LayerList.Remove(entry);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        ILayerListEntryViewModel entry = this.LayerList[oldIndex];
                        this.LayerList[oldIndex] = this.LayerList[newIndex];
                        this.LayerList[newIndex] = entry;
                    }
                    break;
                default:
                    break;
            }
        }

        // TODO move this to another class
        private ILayerListEntryViewModel GenerateEntryViewModel(ILayer layer)
        {
            ILayerListEntryViewModel entry = null;
            if (typeof(Layer).IsInstanceOfType(entry))
            {
                entry = new LayerListLayerViewModel(this.eventAggregator, layer);
            }
            else if (typeof(LayerGroup).IsInstanceOfType(entry))
            {
                entry = new LayerListGroupViewModel(this.eventAggregator, layer);
            }
            return entry;
        }

        private int GetLayerGroupCount()
        {
            IEnumerable<LayerGroup> groups = this.Session.CurrentLayerList.OfType<LayerGroup>();
            return groups.Count<LayerGroup>();
        }

        #endregion methods
    }
}
