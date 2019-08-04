using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
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
using System.Windows;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;
        private ILayerListNodeViewModel selectedNode;

        #endregion fields


        #region constructor

        public LayerListViewModel(IEventAggregator eventAggregator, IAmeSession session, IActionHandler actionHandler)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Session = session ?? throw new ArgumentNullException("session is null");
            this.CurrentMap.Value = session.CurrentMap.Value ?? throw new ArgumentNullException("current map is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("handler is null");

            this.Title.Value = "Layer List";
            this.LayerNodes = new ObservableCollection<ILayerListNodeViewModel>();

            if (this.CurrentMap.Value != null)
            {
                ChangeMap(this.CurrentMap.Value);
            }

            this.CurrentMap.PropertyChanged += CurrentMapChanged;

            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.NewLayerGroupCommand = new DelegateCommand(() => this.actionHandler.NewLayerGroup());
            this.MoveLayerDownCommand = new DelegateCommand(() => this.actionHandler.MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => this.actionHandler.MoveLayerUp());
            this.DuplicateLayerCommand = new DelegateCommand(() => this.actionHandler.DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => this.actionHandler.DeleteLayer());
            this.EditCollisionsCommand = new DelegateCommand(() => this.actionHandler.EditCollisions());
            this.LayerToMapSizeCommand = new DelegateCommand(() => this.actionHandler.LayerToMapSize());
            this.CurrentLayerChangedCommand = new DelegateCommand<object>((entry) => ChangeCurrentLayer(entry as ILayerListNodeViewModel));
            this.DropCommand = new DelegateCommand<object>((args) => HandleDropCommand((DragEventArgs)args));
            this.DragEnterCommand = new DelegateCommand<object>((args) => HandleDragEnterCommand((DragEventArgs)args));
            this.DragLeaveCommand = new DelegateCommand<object>((args) => HandleDragLeaveCommand((DragEventArgs)args));
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
        public ICommand DropCommand { get; private set; }
        public ICommand DragEnterCommand { get; private set; }
        public ICommand DragLeaveCommand { get; private set; }

        public ObservableCollection<ILayerListNodeViewModel> LayerNodes { get; private set; }
        public BindableProperty<ILayer> CurrentLayer { get; set; } = BindableProperty.Prepare<ILayer>();
        public BindableProperty<Map> CurrentMap { get; set; } = BindableProperty.Prepare<Map>();

        public IAmeSession Session { get; set; }

        #endregion properties


        #region methods

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void NewLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void EditProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void ChangeMap(Map map)
        {
            this.LayerNodes.Clear();
            if (this.CurrentMap != null)
            {
                this.CurrentMap.Value.Layers.CollectionChanged -= UpdateLayerList;
                this.CurrentMap.Value.CurrentLayer.PropertyChanged -= CurrentLayerChanged;
            }
            this.CurrentMap.Value = map;
            this.CurrentLayer.Value = map.CurrentLayer.Value;

            map.Layers.CollectionChanged += UpdateLayerList;
            map.CurrentLayer.PropertyChanged += CurrentLayerChanged;

            foreach (ILayer layer in this.CurrentMap.Value.Layers)
            {
                ILayerListNodeViewModel node = LayerListMethods.Generate(this.eventAggregator, this.Session, this.actionHandler, layer);
                this.LayerNodes.Add(node);
            }

            IEnumerable<ILayerListNodeViewModel> selectedNodes = this.LayerNodes.Where(entry => entry.Layer == map.CurrentLayer.Value);
            foreach (ILayerListNodeViewModel node in selectedNodes)
            {
                node.IsSelected.Value = true;
                this.selectedNode = node;
            }
        }

        public void ChangeCurrentLayer(ILayerListNodeViewModel layerEntry)
        {
            if (layerEntry == null)
            {
                return;
            }
            if (layerEntry == this.selectedNode)
            {
                return;
            }
            this.selectedNode.IsSelected.Value = false;
            layerEntry.IsSelected.Value = true;
            this.selectedNode = layerEntry;
            this.CurrentMap.Value.CurrentLayer.Value = layerEntry.Layer;
            this.CurrentLayer.Value = layerEntry.Layer;
        }

        private void CurrentLayerChanged(object sender, PropertyChangedEventArgs e)
        {
            ILayer currentLayer = this.CurrentMap.Value.CurrentLayer.Value;
            this.selectedNode.IsSelected.Value = false;

            List<ILayerListNodeViewModel> selected = new List<ILayerListNodeViewModel>();
            foreach (ILayerListNodeViewModel layerNode in this.LayerNodes)
            {
                selected.AddRange(layerNode.GetNodeFromLayer(currentLayer));
            }

            foreach (ILayerListNodeViewModel entry in selected)
            {
                this.selectedNode = entry;
                this.CurrentLayer.Value = currentLayer;
            }
            this.selectedNode.IsSelected.Value = true;
        }

        private void CurrentMapChanged(object d, PropertyChangedEventArgs e)
        {
            ChangeMap(this.Session.CurrentMap.Value);
        }

        private void UpdateLayerList(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in e.NewItems)
                    {
                        ILayerListNodeViewModel entry = LayerListMethods.Generate(this.eventAggregator, this.Session, this.actionHandler, layer);
                        int insertIndex = e.NewStartingIndex;
                        if (insertIndex < this.LayerNodes.Count)
                        {
                            this.LayerNodes.Insert(insertIndex++, entry);
                        }
                        else
                        {
                            this.LayerNodes.Add(entry);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ILayer layer in e.OldItems)
                    {
                        IEnumerable<ILayerListNodeViewModel> toRemove = new ObservableCollection<ILayerListNodeViewModel>(this.LayerNodes.Where(entry => entry.Layer == layer));
                        foreach (ILayerListNodeViewModel entry in toRemove)
                        {
                            this.LayerNodes.Remove(entry);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        ILayerListNodeViewModel entry = this.LayerNodes[oldIndex];
                        this.LayerNodes[oldIndex] = this.LayerNodes[newIndex];
                        this.LayerNodes[newIndex] = entry;
                    }
                    break;

                default:
                    break;
            }
        }

        private void HandleDropCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (data.GetDataPresent(LayerListMethods.DragDataName))
            {
                ILayer draggedLayer = data.GetData(LayerListMethods.DragDataName) as ILayer;

                draggedLayer.Parent.Layers.Remove(draggedLayer);
                draggedLayer.Parent = this.Session.CurrentMap.Value;
                this.CurrentMap.Value.Layers.Add(draggedLayer);
            }
            args.Handled = true;

            ILayerListNodeViewModel lastLayertNode = this.LayerNodes.Last<ILayerListNodeViewModel>();
            lastLayertNode.IsDragBelow.Value = false;
        }

        private void HandleDragEnterCommand(DragEventArgs args)
        {
            ILayerListNodeViewModel lastLayertNode = this.LayerNodes.Last<ILayerListNodeViewModel>();
            lastLayertNode.IsDragBelow.Value = true;
        }

        private void HandleDragLeaveCommand(DragEventArgs args)
        {
            ILayerListNodeViewModel lastLayertNode = this.LayerNodes.Last<ILayerListNodeViewModel>();
            lastLayertNode.IsDragBelow.Value = false;
        }

        #endregion methods
    }
}
