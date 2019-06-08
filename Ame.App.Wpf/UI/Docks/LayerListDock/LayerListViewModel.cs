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
            this.LayerList = new ObservableCollection<LayerListEntryViewModel>();

            this.Session.PropertyChanged += SessionUpdated;

            this.NewLayerCommand = new DelegateCommand(() => NewTilesetLayer());
            this.NewLayerGroupCommand = new DelegateCommand(() => NewLayerGroup());
            this.MergeLayerDownCommand = new DelegateCommand(() => MergeLayerDown());
            this.MergeLayerUpCommand = new DelegateCommand(() => MergeLayerUp());
            this.MoveLayerDownCommand = new DelegateCommand(() => MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => MoveLayerUp());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => RemoveLayer());
            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.LayerToMapSizeCommand = new DelegateCommand(() => LayerToMapSize());
            this.CurrentLayerChangedCommand = new DelegateCommand<object>((e) =>
            {
                LayerListEntryViewModel currentEntry = e as LayerListEntryViewModel;
                CurrentLayerChanged(currentEntry.layer);
            });

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
        public ICommand EditPropertiesCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand LayerToMapSizeCommand { get; private set; }
        public ICommand CurrentLayerChangedCommand { get; private set; }

        public ObservableCollection<LayerListEntryViewModel> LayerList { get; private set; }

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

        public void CurrentLayerChanged(ILayer layer)
        {
            this.Session.CurrentLayer = layer;
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
                    foreach (Layer layer in this.Session.CurrentLayerList)
                    {
                        this.LayerList.Add(new LayerListEntryViewModel(this.eventAggregator, layer));
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
                    foreach(Layer layer in e.NewItems)
                    {
                        this.LayerList.Add(new LayerListEntryViewModel(this.eventAggregator, layer));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach(Layer layer in e.NewItems)
                    {
                        IEnumerable<LayerListEntryViewModel> toRemove = this.LayerList.Where(entry => entry.Layer == layer);
                        foreach (LayerListEntryViewModel entry in toRemove)
                        {
                            this.LayerList.Remove(entry);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // TODO implement
                    break;
                default:
                    break;
            }
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
