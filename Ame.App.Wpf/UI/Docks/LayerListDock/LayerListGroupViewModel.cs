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
using System.Windows.Media;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListGroupViewModel : ILayerListNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;
        private IAmeSession session;

        private bool isDragging;
        private Point startDragPoint;

        #endregion fields


        #region constructor

        public LayerListGroupViewModel(IEventAggregator eventAggregator, IAmeSession session, IActionHandler handler, LayerGroup layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.layer = layer ?? throw new ArgumentNullException("layer");
            this.actionHandler = handler ?? throw new ArgumentNullException("handler is null");

            this.LayerNodes = new ObservableCollection<ILayerListNodeViewModel>();
            this.isDragging = false;

            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingGroup filled = new DrawingGroup();
            using (DrawingContext context = filled.Open())
            {
                Rect drawingRect = new Rect(0, 0, layer.GetPixelWidth(), layer.GetPixelHeight());
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
            drawingGroup.Children.Add(filled);
            drawingGroup.Children.Add(layer.Group);
            this.layerPreview = new DrawingImage(drawingGroup);

            this.layer.Layers.CollectionChanged += LayersChanged;

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

        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }
        public ICommand MouseLeftButtonDownCommand { get; private set; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand DropCommand { get; private set; }
        public ICommand DragOverCommand { get; private set; }
        public ICommand DragEnterCommand { get; private set; }
        public ICommand DragLeaveCommand { get; private set; }

        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsSelected { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragAbove { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragOnto { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragBelow { get; set; } = BindableProperty<bool>.Prepare(false);

        public LayerGroup layer;
        public ILayer Layer
        {
            get
            {
                return this.layer;
            }
            set
            {
                if (typeof(LayerGroup).IsInstanceOfType(value))
                {
                    this.layer = value as LayerGroup;
                }
            }
        }

        public ObservableCollection<ILayerListNodeViewModel> LayerNodes { get; private set; }

        private DrawingImage layerPreview;
        public DrawingImage LayerPreview
        {
            get
            {
                return this.layerPreview;
            }
        }

        #endregion properties


        #region methods

        public IEnumerable<ILayerListNodeViewModel> GetNodeFromLayer(ILayer layer)
        {
            List<ILayerListNodeViewModel> selected = new List<ILayerListNodeViewModel>();
            if (this.layer == layer)
            {
                selected.Add(this);
            }

            foreach (ILayerListNodeViewModel layerNode in this.LayerNodes)
            {
                selected.AddRange(layerNode.GetNodeFromLayer(layer));
            }
            return selected;
        }

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in args.NewItems)
                    {
                        ILayerListNodeViewModel entry = LayerListMethods.Generate(this.eventAggregator, this.session, this.actionHandler, layer);
                        int insertIndex = args.NewStartingIndex;
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
                    foreach (ILayer layer in args.OldItems)
                    {
                        IEnumerable<ILayerListNodeViewModel> toRemove = new ObservableCollection<ILayerListNodeViewModel>(this.LayerNodes.Where(entry => entry.Layer == layer));
                        foreach (ILayerListNodeViewModel entry in toRemove)
                        {
                            this.LayerNodes.Remove(entry);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = args.OldStartingIndex;
                    int newIndex = args.NewStartingIndex;
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
            if (data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.LayerListNode)))
            {
                ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.LayerListNode)) as ILayer;
                if (draggedLayer != null)
                {
                    if (this.IsDragAbove.Value)
                    {
                        this.layer.AddLayerAbove(draggedLayer);
                    }
                    else if (this.IsDragOnto.Value)
                    {
                        this.layer.AddLayer(draggedLayer);
                    }
                    else if (this.IsDragBelow.Value)
                    {
                        this.layer.AddLayerBelow(draggedLayer);
                    }
                }
            }
            this.IsDragAbove.Value = false;
            this.IsDragOnto.Value = false;
            this.IsDragBelow.Value = false;
            args.Handled = true;
        }

        private void HandleDragOverCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (!data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.LayerListNode)))
            {
                return;
            }
            ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.LayerListNode)) as ILayer;
            if (draggedLayer == this.layer)
            {
                return;
            }
            DrawSeparator(args);
            args.Handled = true;
        }

        private void HandleDragEnterCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (!data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.LayerListNode)))
            {
                return;
            }
            ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.LayerListNode)) as ILayer;
            if (draggedLayer == this.layer)
            {
                return;
            }
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

            DataObject data = new DataObject(SerializableNameUtils.GetName(DragDataType.LayerListNode), this.Layer);
            DependencyObject dragSource = args.Source as DependencyObject;
            DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);

            this.isDragging = false;
        }

        private void DrawSeparator(DragEventArgs args)
        {
            UIElement dragSource = args.Source as UIElement;
            double aboveHeight = 1 * dragSource.RenderSize.Height / 4;
            double belowHeight = 2 * dragSource.RenderSize.Height / 4;
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
