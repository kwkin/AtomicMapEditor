using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
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
        private IAmeSession session;

        private DrawingGroup drawingGroup = new DrawingGroup();
        private DrawingGroup filled = new DrawingGroup();

        private bool isDragging;
        private Point startDragPoint;

        #endregion fields


        #region constructor

        public LayerListNodeViewModel(IEventAggregator eventAggregator, IAmeSession session, Layer layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.Layer = layer ?? throw new ArgumentNullException("layer");

            drawingGroup = new DrawingGroup();
            filled = new DrawingGroup();

            drawingGroup.Children.Add(filled);
            drawingGroup.Children.Add(layer.Group);
            this.layerPreview = new DrawingImage(drawingGroup);

            RefreshPreview();
            this.isDragging = false;

            layer.PixelHeight.PropertyChanged += LayerSizeChanged;
            layer.PixelWidth.PropertyChanged += LayerSizeChanged;

            this.EditPropertiesCommand = new DelegateCommand(() => EditProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.MoveLayerDownCommand = new DelegateCommand(() => MoveLayerDown());
            this.MoveLayerUpCommand = new DelegateCommand(() => MoveLayerUp());
            this.LayerToMapSizeCommand = new DelegateCommand(() => LayerToMapSize());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.RemoveLayerCommand = new DelegateCommand(() => RemoveLayer());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
            this.MouseLeftButtonDownCommand = new DelegateCommand<object>((point) => HandleLeftClickDown((MouseEventArgs)point));
            this.MouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((MouseEventArgs)point));
            this.DropCommand = new DelegateCommand<object>((point) => HandleDropCommand((DragEventArgs)point));
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

        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsSelected { get; set; } = BindableProperty<bool>.Prepare(false);

        #endregion properties


        #region methods

        public void MoveLayerDown()
        {
            int currentLayerIndex = this.session.CurrentLayers.Value.IndexOf(this.Layer);
            if (currentLayerIndex < this.session.CurrentLayers.Value.Count - 1 && currentLayerIndex >= 0)
            {
                this.session.CurrentLayers.Value.Move(currentLayerIndex, currentLayerIndex + 1);
            }
        }

        public void MoveLayerUp()
        {
            int currentLayerIndex = this.session.CurrentLayers.Value.IndexOf(this.layer);
            if (currentLayerIndex > 0)
            {
                this.session.CurrentLayers.Value.Move(currentLayerIndex, currentLayerIndex - 1);
            }
        }

        public void DuplicateLayer()
        {
            ILayer copiedLayer = Utils.DeepClone<ILayer>(this.session.CurrentMap.Value.CurrentLayer.Value);
            this.Layer.AddToMe(copiedLayer);
        }

        public void RemoveLayer()
        {
            this.session.CurrentLayers.Value.Remove(this.Layer);
        }

        public void EditProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction(this.Layer);
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

        public void RefreshPreview()
        {
            using (DrawingContext context = filled.Open())
            {
                Rect drawingRect = new Rect(0, 0, layer.PixelWidth.Value, layer.PixelHeight.Value);
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
        }

        public void HandleLeftClickDown(MouseEventArgs e)
        {
            this.startDragPoint = e.GetPosition(null);
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !this.isDragging)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - this.startDragPoint.X) > SystemParameters.MinimumHorizontalDragDistance
                        || Math.Abs(position.Y - this.startDragPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag(e);
                }
            }
        }

        private void HandleDropCommand(DragEventArgs e)
        {
            IDataObject data = e.Data;
            if (data.GetDataPresent(typeof(ILayer).ToString()))
            {
                ILayer draggedLayer = data.GetData(typeof(ILayer).ToString()) as ILayer;
                this.layer.AddToMe(draggedLayer);
            }
            e.Handled = true;
        }

        private void StartDrag(MouseEventArgs e)
        {
            this.isDragging = true;

            // TODO make a standard text for drag and drop data types
            DataObject data = new DataObject(typeof(ILayer).ToString(), this.Layer);
            DependencyObject dragSource = e.Source as DependencyObject;

            DragDropEffects de = DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);

            this.isDragging = false;
        }

        private void LayerSizeChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshPreview();
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
