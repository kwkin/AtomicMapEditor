using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
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
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("handler is null");

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
            this.DropCommand = new DelegateCommand<object>((point) => HandleDrop((DragEventArgs)point));
        }

        #endregion constructor


        #region properties
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }
        public ICommand MouseLeftButtonDownCommand { get; private set; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand DropCommand { get; private set; }


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

        private DrawingImage layerPreview;
        public DrawingImage LayerPreview
        {
            get
            {
                return this.layerPreview;
            }
        }

        public ObservableCollection<ILayerListNodeViewModel> LayerNodes { get; private set; }
        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsSelected { get; set; } = BindableProperty<bool>.Prepare(false);

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

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in e.NewItems)
                    {
                        ILayerListNodeViewModel entry = LayerListNodeGenerator.Generate(this.eventAggregator, this.session, this.actionHandler, layer);
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

        private void HandleLeftClickDown(MouseEventArgs e)
        {
            this.startDragPoint = e.GetPosition(null);
        }

        private void HandleMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !this.isDragging)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - this.startDragPoint.X) > SystemParameters.MinimumHorizontalDragDistance
                        || Math.Abs(position.Y - this.startDragPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    HandleStartDrag(e);
                }
            }
        }

        private void HandleStartDrag(MouseEventArgs e)
        {
            this.isDragging = true;

            DataObject data = new DataObject(typeof(ILayer).ToString(), this.Layer);
            DependencyObject dragSource = e.Source as DependencyObject;

            DragDropEffects de = DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);

            this.isDragging = false;
        }

        private void HandleDrop(DragEventArgs e)
        {
            IDataObject data = e.Data;
            if (data.GetDataPresent(typeof(ILayer).ToString()))
            {
                ILayer draggedLayer = data.GetData(typeof(ILayer).ToString()) as ILayer;
                this.layer.AddToMe(draggedLayer);
            }
            e.Handled = true;
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
