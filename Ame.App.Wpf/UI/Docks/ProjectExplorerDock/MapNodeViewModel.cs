using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    public class MapNodeViewModel : IProjectExplorerNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;

        private bool isDragging;
        private Point startDragPoint;

        #endregion fields


        #region constructor

        public MapNodeViewModel(IEventAggregator eventAggregator, IActionHandler actionHandler, Map map)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Map = map ?? throw new ArgumentNullException("layer is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("actionHandler is null");

            this.LayerNodes = new ObservableCollection<LayerNodeViewModel>();
            map.Layers.ToList().ForEach(layer => AddLayer(layer));

            this.Map.Layers.CollectionChanged += LayersChanged;

            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
            this.MouseLeftButtonDownCommand = new DelegateCommand<object>((point) => HandleLeftClickDown((MouseEventArgs)point));
            this.MouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((MouseEventArgs)point));
            this.DropCommand = new DelegateCommand<object>((point) => HandleDropCommand((DragEventArgs)point));
            this.DragOverCommand = new DelegateCommand<object>((args) => HandleDragOverCommand((DragEventArgs)args));
            this.DragEnterCommand = new DelegateCommand<object>((args) => HandleDragEnterCommand((DragEventArgs)args));
            this.DragLeaveCommand = new DelegateCommand<object>((args) => HandleDragLeaveCommand((DragEventArgs)args));
        }

        #endregion constructor


        #region properties

        public ICommand NewLayerCommand { get; private set; }
        public ICommand EditMapPropertiesCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }
        public ICommand MouseLeftButtonDownCommand { get; private set; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand DropCommand { get; private set; }
        public ICommand DragOverCommand { get; private set; }
        public ICommand DragEnterCommand { get; private set; }
        public ICommand DragLeaveCommand { get; private set; }

        public ObservableCollection<LayerNodeViewModel> LayerNodes { get; set; }
        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsSelected { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragAbove { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragOnto { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragBelow { get; set; } = BindableProperty<bool>.Prepare(false);

        public Map Map { get; set; }

        #endregion properties


        #region methods

        public void AddLayer(ILayer layer)
        {
            LayerNodeViewModel node = new LayerNodeViewModel(this.eventAggregator, this.actionHandler, layer);
            this.LayerNodes.Add(node);
        }

        public void RemoveLayer(ILayer layer)
        {
            IEnumerable<LayerNodeViewModel> toRemove = this.LayerNodes.Where(entry => entry.Layer == layer);
            foreach (LayerNodeViewModel node in toRemove)
            {
                this.LayerNodes.Remove(node);
            }
        }

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in e.NewItems)
                    {
                        AddLayer(layer);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ILayer layer in e.OldItems)
                    {
                        RemoveLayer(layer);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        LayerNodeViewModel entry = this.LayerNodes[oldIndex];
                        this.LayerNodes[oldIndex] = this.LayerNodes[newIndex];
                        this.LayerNodes[newIndex] = entry;
                    }
                    break;

                default:
                    break;
            }
        }

        private void HandleLeftClickDown(MouseEventArgs args)
        {
            this.startDragPoint = args.GetPosition(null);
        }

        private void HandleMouseMove(MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed && !this.isDragging)
            {
                Point position = args.GetPosition(null);
                if (Math.Abs(position.X - this.startDragPoint.X) > SystemParameters.MinimumHorizontalDragDistance
                        || Math.Abs(position.Y - this.startDragPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    HandleStartDrag(args);
                }
            }
        }

        private void HandleDropCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.ExplorerLayerNode)))
            {
                ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.ExplorerLayerNode)) as ILayer;
                this.Map.Layers.Insert(0, draggedLayer);
            }
            else if (data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.ExplorerMapNode)))
            {
                Map draggedMap = data.GetData(SerializableNameUtils.GetName(DragDataType.ExplorerMapNode)) as Map;
            }
            else if (data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.ExplorerProjectNode)))
            {
                Project draggedProject = data.GetData(SerializableNameUtils.GetName(DragDataType.ExplorerMapNode)) as Project;
            }
            this.IsDragAbove.Value = false;
            this.IsDragOnto.Value = false;
            this.IsDragBelow.Value = false;
            args.Handled = true;
        }

        private void HandleDragOverCommand(DragEventArgs args)
        {
            DrawSeparator(args);
            args.Handled = true;
        }

        private void HandleDragEnterCommand(DragEventArgs args)
        {
            DrawSeparator(args);
            args.Handled = true;
        }

        private void HandleDragLeaveCommand(DragEventArgs args)
        {
            this.IsDragAbove.Value = false;
            this.IsDragBelow.Value = false;
            this.IsDragOnto.Value = false;
            args.Handled = true;
        }

        private void HandleStartDrag(MouseEventArgs args)
        {
            this.isDragging = true;

            DataObject data = new DataObject(typeof(Map).ToString(), this.Map);
            DependencyObject dragSource = args.Source as DependencyObject;
            DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);

            this.isDragging = false;
        }

        private void DrawSeparator(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (data.GetDataPresent(typeof(ILayer).ToString()))
            {
                ILayer draggedLayer = data.GetData(typeof(ILayer).ToString()) as ILayer;
            }
            UIElement dragSource = args.Source as UIElement;
            double aboveHeight = 1 * dragSource.RenderSize.Height / 3;
            double belowHeight = 2 * dragSource.RenderSize.Height / 3;
            if (aboveHeight - args.GetPosition(dragSource).Y > 0)
            {
                this.IsDragAbove.Value = true;
                this.IsDragOnto.Value = false;
                this.IsDragBelow.Value = false;
            }
            else if (belowHeight - args.GetPosition(dragSource).Y > 0 || this.LayerNodes.Count != 0)
            {
                this.IsDragAbove.Value = false;
                this.IsDragOnto.Value = true;
                this.IsDragBelow.Value = false;
            }
            else
            {
                this.IsDragAbove.Value = false;
                this.IsDragOnto.Value = false;
                this.IsDragBelow.Value = true;
            }
        }

        private void NewLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction(this.Map);
            this.actionHandler.OpenWindow(interaction);
        }

        private void EditMapProperties()
        {
            EditMapInteraction interaction = new EditMapInteraction(this.Map);
            this.actionHandler.OpenWindow(interaction);
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
