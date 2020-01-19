using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.Attributes;
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

        private const string DefaultTitle = "Layer List";

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;
        private ILayerListNodeViewModel selectedNode;

        private ObservableCollection<ILayer> currentLayers;

        #endregion fields


        #region constructor

        public LayerListViewModel(IEventAggregator eventAggregator, IAmeSession session, IActionHandler actionHandler)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Session = session ?? throw new ArgumentNullException("session is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("handler is null");

            this.Title.Value = DefaultTitle;
            this.LayerNodes = new ObservableCollection<ILayerListNodeViewModel>();

            LoadLayers(this.Session.CurrentLayers.Value);

            this.Session.CurrentMap.PropertyChanged += CurrentMapChanged;
            this.Session.CurrentLayers.PropertyChanged += CurrentLayersChanged;
            this.Session.CurrentLayer.PropertyChanged += CurrentLayerChanged;

            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.NewLayerGroupCommand = new DelegateCommand(() => this.actionHandler.NewLayerGroup());
            this.MoveLayerDownCommand = new DelegateCommand(() => this.actionHandler.MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => this.actionHandler.MoveLayerUp());
            this.DuplicateLayerCommand = new DelegateCommand(() => this.actionHandler.DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => this.actionHandler.DeleteLayer(this.CurrentLayer.Value));
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

        public BindableProperty<Map> CurrentMap { get; set; } = BindableProperty.Prepare<Map>();
        public ObservableCollection<ILayerListNodeViewModel> LayerNodes { get; private set; }
        public BindableProperty<ILayer> CurrentLayer { get; set; } = BindableProperty.Prepare<ILayer>();

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

        public void ChangeLayers(ObservableCollection<ILayer> layers)
        {
            if (this.currentLayers != null)
            {
                this.currentLayers.CollectionChanged -= UpdateLayers;
            }
            this.currentLayers = layers;
            this.currentLayers.CollectionChanged += UpdateLayers;

            LoadLayers(this.currentLayers);
        }

        private void LoadLayers(ObservableCollection<ILayer> layers)
        {
            this.LayerNodes.Clear();
            if (layers == null)
            {
                return;
            }

            foreach (ILayer layer in layers)
            {
                ILayerListNodeViewModel node = LayerListMethods.Generate(this.eventAggregator, this.Session, this.actionHandler, layer);
                this.LayerNodes.Add(node);

                if (this.Session.CurrentLayer.Value == layer)
                {
                    node.IsSelected.Value = true;
                    this.selectedNode = node;
                }
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
            if (this.selectedNode != null)
            {
                this.selectedNode.IsSelected.Value = false;
            }
            this.selectedNode = layerEntry;
            this.selectedNode.IsSelected.Value = true;

            this.Session.CurrentLayer.Value = this.selectedNode.Layer;
        }

        private void CurrentMapChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CurrentMap.Value = GetNewValue(sender, e) as Map;
            this.Title.Value = DefaultTitle + " - " + this.CurrentMap.Value.Name.Value;
        }

        private void CurrentLayersChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangeLayers(GetNewValue(sender, e) as ObservableCollection<ILayer>);
        }

        private void CurrentLayerChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CurrentLayer.Value = GetNewValue(sender, e) as ILayer;

            ILayerListNodeViewModel layerEntry = this.LayerNodes.FirstOrDefault(entry => entry.Layer == this.CurrentLayer.Value);
            ChangeCurrentLayer(layerEntry);
        }

        private void UpdateLayers(object sender, NotifyCollectionChangedEventArgs e)
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
                        ILayerListNodeViewModel toRemove = this.LayerNodes.FirstOrDefault(entry => entry.Layer == layer);
                        this.LayerNodes.Remove(toRemove);
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
            if (data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.LayerListNode)))
            {
                ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.LayerListNode)) as ILayer;
                this.Session.CurrentMap.Value.AddLayer(draggedLayer);
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
