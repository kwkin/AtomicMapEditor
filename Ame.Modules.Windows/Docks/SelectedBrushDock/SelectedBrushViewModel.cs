using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.SelectedBrushDock
{
    public class SelectedBrushViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private DrawingGroup drawingGroup;
        private DrawingGroup extendedBackground;
        private DrawingGroup selectedBrushImage;
        private DrawingGroup gridLines;
        private CoordinateTransform imageTransform;
        private GridModel gridModel;

        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private Stopwatch updatePositionLabelStopWatch;

        #endregion fields


        #region constructor

        public SelectedBrushViewModel(IEventAggregator eventAggregator)
            : this(eventAggregator, ScrollModel.DefaultScrollModel())
        {
        }

        public SelectedBrushViewModel(IEventAggregator eventAggregator, IScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel");
            }
            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;
            this.Title = "Selected Brush";

            this.imageTransform = new CoordinateTransform();
            this.drawingGroup = new DrawingGroup();
            this.extendedBackground = new DrawingGroup();
            this.selectedBrushImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.selectedBrushImage);
            this.drawingGroup.Children.Add(this.extendedBackground);
            this.drawingGroup.Children.Add(this.gridLines);
            this.BrushImage = new DrawingImage(this.drawingGroup);
            RenderOptions.SetEdgeMode(this.selectedBrushImage, EdgeMode.Aliased);

            this.gridModel = new PaddedGridRenderable();
            this.ZoomLevels = this.scrollModel.ZoomLevels;
            this.ZoomIndex = this.scrollModel.ZoomIndex;
            this.Scale = ScaleType.Pixel;
            this.PositionText = "0, 0";
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            this.ShowGridCommand = new DelegateCommand(() =>
            {
                DrawGrid(this.IsGridOn);
            });
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) =>
            {
                HandleMouseMove((Point)point);
            });
            this.ZoomInCommand = new DelegateCommand(() =>
            {
                this.ZoomIndex = this.scrollModel.ZoomIn();
            });
            this.ZoomOutCommand = new DelegateCommand(() =>
            {
                this.ZoomIndex = this.scrollModel.ZoomOut();
            });
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) =>
            {
                this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel);
            });

            this.eventAggregator.GetEvent<NewPaddedBrushEvent>().Subscribe((brushEvent) =>
            {
                UpdateBrushImage(brushEvent);
            }, ThreadOption.UIThread);
        }

        #endregion constructor


        #region properties

        public ICommand ShowGridCommand { get; private set; }
        public ICommand HandleMouseMoveCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public string PositionText { get; set; }
        public ScaleType Scale { get; set; }
        public bool IsGridOn { get; set; }
        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }

        public int zoomIndex;
        public int ZoomIndex
        {
            get { return this.zoomIndex; }
            set
            {
                if (SetProperty(ref this.zoomIndex, value))
                {
                    this.gridLines.Children.Clear();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        RefreshGrid();
                    }),
                    DispatcherPriority.Background);
                }
            }
        }

        public DrawingImage BrushImage { get; set; }

        #endregion properties


        #region methods

        public void RefreshGrid()
        {
            DrawGrid(this.IsGridOn);
        }

        public void DrawGrid(bool drawGrid)
        {
            this.IsGridOn = drawGrid;
            if (this.IsGridOn)
            {
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.gridModel);
                double thickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
                gridParameters.DrawingPen.Thickness = thickness < Global.maxGridThickness ? thickness : Global.maxGridThickness;
                DrawingGroup group = gridParameters.CreateGrid();
                this.gridLines.Children = group.Children;
            }
            else
            {
                this.gridLines.Children.Clear();
            }
            RaisePropertyChanged(nameof(this.IsGridOn));
            RaisePropertyChanged(nameof(this.BrushImage));
        }

        public void HandleMouseMove(Point position)
        {
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.imageTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(position);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
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
                switch (Scale)
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
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);

            RaisePropertyChanged(nameof(this.PositionText));
            this.updatePositionLabelStopWatch.Restart();
        }

        private void UpdateBrushImage(PaddedBrushModel brushModel)
        {
            using (DrawingContext context = this.selectedBrushImage.Open())
            {
                foreach (Tile tile in brushModel.Tiles)
                {
                    context.DrawDrawing(tile.Image);
                }
            }
            this.gridModel.SetHeightWithRows(brushModel.RowCount(), brushModel.TileHeight);
            this.gridModel.SetWidthWithColumns(brushModel.ColumnCount(), brushModel.TileWidth);

            this.imageTransform.SetPixelToTile(this.gridModel.TileWidth, this.gridModel.TileHeight);
            this.imageTransform.SetSlectionToPixel(this.gridModel.TileWidth / 2, this.gridModel.TileHeight / 2);

            redrawExtendedBackground();
            RefreshGrid();
            RaisePropertyChanged(nameof(this.BrushImage));
        }

        private void redrawExtendedBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.gridModel.PixelWidth + this.gridModel.TileWidth;
            extendedSize.Height = this.gridModel.PixelHeight + this.gridModel.TileHeight;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.gridModel.TileWidth / 2;
            extendedPoint.Y = -this.gridModel.TileHeight / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            using (DrawingContext context = this.extendedBackground.Open())
            {
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
        }

        #endregion methods
    }
}
