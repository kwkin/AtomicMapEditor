using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
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

        private Map previousMap;

        #endregion fields


        #region constructor

        public LayerListViewModel(IEventAggregator eventAggregator, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Session = session ?? throw new ArgumentNullException("session is null");

            this.Title.Value = "Layer List";
            this.Layers = new ObservableCollection<ILayerListEntryViewModel>();

            if (this.Session.CurrentMap.Value != null)
            {
                ChangeMap(this.Session.CurrentMap.Value);
            }

            this.Session.CurrentMap.PropertyChanged += CurrentMapChanged;

            this.NewLayerCommand = new DelegateCommand(() => NewTilesetLayer());
            this.NewLayerGroupCommand = new DelegateCommand(() => NewLayerGroup());
            this.MoveLayerDownCommand = new DelegateCommand(() => MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => MoveLayerUp());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => RemoveLayer());
            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.LayerToMapSizeCommand = new DelegateCommand(() => LayerToMapSize());
            this.CurrentLayerChangedCommand = new DelegateCommand<object>((entry) => CurrentLayerChanged(entry as ILayerListEntryViewModel));

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

        public ObservableCollection<ILayerListEntryViewModel> Layers { get; private set; }

        public IAmeSession Session { get; set; }

        #endregion properties


        #region methods

        public void AddTilesetLayerMessage(NewLayerMessage message)
        {
            AddTilesetLayer(message.Layer);
        }

        public void AddTilesetLayer(ILayer layer)
        {
            this.Session.CurrentMap.Value.Layers.Add(layer);
        }

        public void NewTilesetLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void NewLayerGroup()
        {
            int layerGroupCount = this.Session.CurrentMap.Value.GetLayerGroupCount();
            string newLayerGroupName = string.Format("Layer Group #{0}", layerGroupCount);
            this.Session.CurrentLayer.Value.AddSibling(new LayerGroup(newLayerGroupName));
        }

        public void MoveLayerDown()
        {
            int currentLayerIndex = this.Session.CurrentLayers.Value.IndexOf(this.Session.CurrentLayer.Value);
            if (currentLayerIndex < this.Session.CurrentLayers.Value.Count - 1 && currentLayerIndex >= 0)
            {
                this.Session.CurrentLayers.Value.Move(currentLayerIndex, currentLayerIndex + 1);
            }
        }

        public void MoveLayerUp()
        {
            int currentLayerIndex = this.Session.CurrentLayers.Value.IndexOf(this.Session.CurrentLayer.Value);
            if (currentLayerIndex > 0)
            {
                this.Session.CurrentLayers.Value.Move(currentLayerIndex, currentLayerIndex - 1);
            }
        }

        public void DuplicateLayer()
        {
            ILayer copiedLayer = Utils.DeepClone<ILayer>(this.Session.CurrentLayer.Value);
            this.Session.CurrentLayer.Value.AddSibling(copiedLayer);
        }

        public void RemoveLayer()
        {
            if (this.Session.CurrentLayer == null)
            {
                return;
            }
            this.Session.CurrentLayers.Value.Remove(this.Session.CurrentLayer.Value);
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

        public void CurrentLayerChanged(ILayerListEntryViewModel layerEntry)
        {
            if (layerEntry != null)
            {
                this.Session.CurrentLayer.Value = layerEntry.Layer;
            }
        }

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void CurrentMapChanged(object d, PropertyChangedEventArgs e)
        {
            ChangeMap(this.Session.CurrentMap.Value);
        }

        public void ChangeMap(Map map)
        {
            this.Layers.Clear();
            if (this.previousMap != null)
            {
                this.previousMap.Layers.CollectionChanged -= UpdateLayerList;
            }
            this.previousMap = map;
            map.Layers.CollectionChanged += UpdateLayerList;
            foreach (ILayer layer in this.Session.CurrentLayers.Value)
            {
                ILayerListEntryViewModel entry = LayerListEntryGenerator.Generate(this.eventAggregator, this.Session, layer);
                this.Layers.Add(entry);
            }
        }

        private void UpdateLayerList(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in e.NewItems)
                    {
                        ILayerListEntryViewModel entry = LayerListEntryGenerator.Generate(this.eventAggregator, this.Session, layer);
                        int insertIndex = e.NewStartingIndex;
                        if (insertIndex < this.Layers.Count)
                        {
                            this.Layers.Insert(insertIndex, entry);
                        }
                        else
                        {
                            this.Layers.Add(entry);
                        }
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
