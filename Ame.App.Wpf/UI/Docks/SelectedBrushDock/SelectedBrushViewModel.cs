using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ame.App.Wpf.UI.Docks.SelectedBrushDock
{
    public class SelectedBrushViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        private DrawingGroup drawingGroup;
        private DrawingGroup extendedBackground;
        private DrawingGroup selectedBrushImage;
        private DrawingGroup gridLines;
        private CoordinateTransform imageTransform;
        private GridModel gridModel;

        private double maxGridThickness;

        private long updatePositionLabelMsDelay;
        private Stopwatch updatePositionLabelStopWatch;

        #endregion fields


        #region constructor

        public SelectedBrushViewModel(IEventAggregator eventAggregator, IConstants constants)
            : this(eventAggregator, constants, Components.Behaviors.ScrollModel.DefaultScrollModel())
        {
        }

        public SelectedBrushViewModel(IEventAggregator eventAggregator, IConstants constants, IScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.ScrollModel = scrollModel ?? throw new ArgumentNullException("scrollModel is null");

            this.Title.Value = "Selected Brush";

            this.imageTransform = new CoordinateTransform();
            this.drawingGroup = new DrawingGroup();
            this.extendedBackground = new DrawingGroup();
            this.selectedBrushImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.selectedBrushImage);
            this.drawingGroup.Children.Add(this.extendedBackground);
            this.drawingGroup.Children.Add(this.gridLines);
            this.BrushImage.Value = new DrawingImage(this.drawingGroup);
            RenderOptions.SetEdgeMode(this.selectedBrushImage, EdgeMode.Aliased);

            this.gridModel = new PaddedGridRenderable();
            this.Scale.Value = ScaleType.Pixel;
            this.PositionText.Value = "0, 0";
            this.maxGridThickness = constants.MaxGridThickness;

            this.updatePositionLabelMsDelay = constants.DefaultUpdatePositionLabelMsDelay;
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            this.ScrollModel.PropertyChanged += ScrollModelPropertyChanged;

            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn.Value));
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((Point)point));
            this.ZoomInCommand = new DelegateCommand(() => this.ScrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => this.ScrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => this.ScrollModel.SetZoom(zoomLevel));

            this.eventAggregator.GetEvent<NewPaddedBrushEvent>().Subscribe((brushEvent) =>
            {
                UpdateBrushImage(brushEvent);
            }, ThreadOption.UIThread);
        }

        #endregion constructor


        #region properties

        public ICommand ShowCollisionsCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand HandleMouseMoveCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public BindableProperty<string> PositionText { get; set; } = BindableProperty<string>.Prepare(string.Empty);
        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty<ScaleType>.Prepare();
        public BindableProperty<bool> IsGridOn { get; set; } = BindableProperty<bool>.Prepare();
        public BindableProperty<DrawingImage> BrushImage { get; set; } = BindableProperty<DrawingImage>.Prepare();

        public IScrollModel ScrollModel { get; set; }

        #endregion properties


        #region methods

        public void RedrawGrid()
        {
            DrawGrid(this.IsGridOn.Value);
        }

        public void DrawGrid(bool drawGrid)
        {
            this.IsGridOn.Value = drawGrid;
            if (this.IsGridOn.Value)
            {
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.gridModel);
                double thickness = 1 / this.ScrollModel.ZoomLevels[this.ScrollModel.ZoomIndex].zoom;
                gridParameters.DrawingPen.Thickness = thickness < this.maxGridThickness ? thickness : this.maxGridThickness;
                DrawingGroup group = gridParameters.CreateGrid();
                this.gridLines.Children = group.Children;
            }
            else
            {
                this.gridLines.Children.Clear();
            }
        }

        public void HandleMouseMove(Point position)
        {
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.imageTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(position);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelMsDelay)
            {
                UpdatePositionLabel(pixelPoint);
            }
        }

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.BrushImage != null)
            {
                switch (this.Scale.Value)
                {
                    case ScaleType.Pixel:
                        transformedPosition = position;
                        break;

                    case ScaleType.Tile:
                        if (this.BrushImage != null)
                        {
                            GeneralTransform transform = GeometryUtils.CreateTransform(this.imageTransform.pixelToTile);
                            transformedPosition = transform.Transform(position);
                        }
                        break;
                }
            }
            transformedPosition = GeometryUtils.CreateIntPoint(transformedPosition);
            this.PositionText.Value = (transformedPosition.X + ", " + transformedPosition.Y);

            this.updatePositionLabelStopWatch.Restart();
        }

        private void UpdateBrushImage(PaddedBrushModel brushModel)
        {
            using (DrawingContext context = this.selectedBrushImage.Open())
            {
                foreach (Tile tile in brushModel.Tiles)
                {
                    context.DrawDrawing(tile.Image.Value);
                }
            }
            this.gridModel.Columns = brushModel.Columns;
            this.gridModel.Rows = brushModel.Rows;
            this.gridModel.TileWidth = brushModel.TileWidth;
            this.gridModel.TileHeight = brushModel.TileHeight;

            this.imageTransform.SetPixelToTile(this.gridModel.TileWidth.Value, this.gridModel.TileHeight.Value);
            this.imageTransform.SetSelectionToPixel(this.gridModel.TileWidth.Value / 2, this.gridModel.TileHeight.Value / 2);

            redrawExtendedBackground();
            RedrawGrid();
        }

        private void redrawExtendedBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.gridModel.PixelWidth.Value + this.gridModel.TileWidth.Value;
            extendedSize.Height = this.gridModel.PixelHeight.Value + this.gridModel.TileHeight.Value;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.gridModel.TileWidth.Value / 2;
            extendedPoint.Y = -this.gridModel.TileHeight.Value / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            using (DrawingContext context = this.extendedBackground.Open())
            {
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
        }

        private void ScrollModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ScrollModel.ZoomIndex):
                    this.gridLines.Children.Clear();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        RedrawGrid();
                    }),
                    DispatcherPriority.Background);
                    break;
                default:
                    break;
            }
        }

        #endregion methods
    }
}
