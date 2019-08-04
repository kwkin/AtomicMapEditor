using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public class LayerListNodeViewModel : ILayerListNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;
        private IAmeSession session;

        private DrawingGroup drawingGroup;
        private DrawingGroup filled;

        private bool isDragging;
        private Point startDragPoint;

        #endregion fields


        #region constructor

        public LayerListNodeViewModel(IEventAggregator eventAggregator, IAmeSession session, IActionHandler actionHandler, Layer layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.Layer = layer ?? throw new ArgumentNullException("layer");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("handler is null");

            this.drawingGroup = new DrawingGroup();
            this.filled = new DrawingGroup();
            this.drawingGroup.Children.Add(filled);
            this.drawingGroup.Children.Add(layer.Group);
            this.layerPreview = new DrawingImage(drawingGroup);
            this.isDragging = false;

            RefreshPreview();

            layer.PixelHeight.PropertyChanged += LayerSizeChanged;
            layer.PixelWidth.PropertyChanged += LayerSizeChanged;

            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => this.actionHandler.EditCollisions());
            this.MoveLayerDownCommand = new DelegateCommand(() => this.actionHandler.MoveLayerDown(this.Layer));
            this.MoveLayerUpCommand = new DelegateCommand(() => this.actionHandler.MoveLayerUp(this.Layer));
            this.LayerToMapSizeCommand = new DelegateCommand(() => this.actionHandler.LayerToMapSize());
            this.DuplicateLayerCommand = new DelegateCommand(() => this.actionHandler.DuplicateLayer(this.Layer));
            this.RemoveLayerCommand = new DelegateCommand(() => this.actionHandler.DeleteLayer(this.Layer));
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
            this.MouseLeftButtonDownCommand = new DelegateCommand<object>((args) => HandleLeftClickDown((MouseEventArgs)args));
            this.MouseMoveCommand = new DelegateCommand<object>((args) => HandleMouseMove((MouseEventArgs)args));
            this.DropCommand = new DelegateCommand<object>((args) => HandleDropCommand((DragEventArgs)args));
            this.DragOverCommand = new DelegateCommand<object>((args) => HandleDragOverCommand((DragEventArgs)args));
            this.DragEnterCommand = new DelegateCommand<object>((args) => HandleDragEnterCommand((DragEventArgs)args));
            this.DragLeaveCommand = new DelegateCommand<object>((args) => HandleDragLeaveCommand((DragEventArgs)args));
        }

        #endregion constructor


        #region properties

        public ICommand EditPropertiesCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand LayerToMapSizeCommand { get; private set; }
        public ICommand MoveLayerDownCommand { get; private set; }
        public ICommand MoveLayerUpCommand { get; private set; }
        public ICommand DuplicateLayerCommand { get; private set; }
        public ICommand RemoveLayerCommand { get; private set; }
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
        public BindableProperty<bool> IsDragBelow { get; set; } = BindableProperty<bool>.Prepare(false);

        public ILayer layer;
        public ILayer Layer
        {
            get
            {
                return this.layer;
            }
            set
            {
                this.layer = value;
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

        #endregion properties


        #region methods

        public void EditProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction(this.Layer);
            this.actionHandler.OpenWindow(interaction);
        }

        public void RefreshPreview()
        {
            using (DrawingContext context = filled.Open())
            {
                Rect drawingRect = new Rect(0, 0, layer.PixelWidth.Value, layer.PixelHeight.Value);
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
        }

        public IEnumerable<ILayerListNodeViewModel> GetNodeFromLayer(ILayer layer)
        {
            List<ILayerListNodeViewModel> selected = new List<ILayerListNodeViewModel>();
            if (this.layer == layer)
            {
                selected.Add(this);
            }
            return selected;
        }

        private void LayerSizeChanged(object sender, PropertyChangedEventArgs args)
        {
            RefreshPreview();
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
                    StartDrag(args);
                }
            }
        }

        private void HandleDropCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (data.GetDataPresent(LayerListMethods.DragDataName))
            {
                ILayer draggedLayer = data.GetData(LayerListMethods.DragDataName) as ILayer;
                if (this.IsDragAbove.Value)
                {
                    this.layer.AddLayerAbove(draggedLayer);
                }
                else if (this.IsDragBelow.Value)
                {
                    this.layer.AddLayerBelow(draggedLayer);
                }
            }
            this.IsDragAbove.Value = false;
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
            args.Handled = true;
        }

        private void StartDrag(MouseEventArgs args)
        {
            this.isDragging = true;

            DataObject data = new DataObject(LayerListMethods.DragDataName, this.Layer);
            DependencyObject dragSource = args.Source as DependencyObject;
            DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);

            this.isDragging = false;
        }

        private void DrawSeparator(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (!data.GetDataPresent(LayerListMethods.DragDataName))
            {
                return;
            }
            ILayer draggedLayer = data.GetData(LayerListMethods.DragDataName) as ILayer;
            if (draggedLayer == this.layer)
            {
                return;
            }
            UIElement dragSource = args.Source as UIElement;
            double heightMiddle = dragSource.RenderSize.Height / 2;

            bool isBelow = args.GetPosition(dragSource).Y - heightMiddle > 0;
            this.IsDragAbove.Value = !isBelow;
            this.IsDragBelow.Value = isBelow;
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
