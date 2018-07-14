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
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.DrawingBrushes;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.MapEditor.Editor
{
    public class MapEditorViewModel : EditorViewModelTemplate
    {
        #region fields

        private long updatePositionLabelDelay = 30;
        private Stopwatch updatePositionLabelStopWatch;

        private TileDrawer tileDrawer;
        private BrushModel brush;

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private CoordinateTransform imageTransform;

        private DrawingGroup drawingGroup;
        private DrawingGroup mapBackground;
        private DrawingGroup layerItems;
        private DrawingGroup gridLines;

        #endregion fields


        #region constructor

        public MapEditorViewModel(IEventAggregator eventAggregator, Map map) : this(eventAggregator, map, new ScrollModel())
        {
        }

        public MapEditorViewModel(IEventAggregator eventAggregator, Map map, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel");
            }
            this.eventAggregator = eventAggregator;
            this.Map = map;
            this.scrollModel = scrollModel;
            this.Title = map.Name;
            this.CurrentLayer = this.Map.CurrentLayer as Layer;
            this.tileDrawer = new TileDrawer(this.layerItems);
            this.DrawingCanvas = new DrawingImage();
            this.drawingGroup = new DrawingGroup();
            this.mapBackground = new DrawingGroup();
            this.layerItems = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.mapBackground);
            this.drawingGroup.Children.Add(this.layerItems);
            this.drawingGroup.Children.Add(this.gridLines);
            this.DrawingCanvas.Drawing = this.drawingGroup;

            this.backgroundBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#a5efda"));
            this.backgroundPen = new Pen(Brushes.Transparent, 0);
            drawBackground(this.BackgroundBrush, this.BackgroundPen);

            this.imageTransform = new CoordinateTransform();
            this.imageTransform.SetPixelToTile(this.Map.TileWidth, this.Map.TileHeight);
            
            if (this.scrollModel.ZoomLevels == null)
            {
                this.ZoomLevels = ZoomLevel.CreateZoomList(0.125, 0.25, 0.5, 1, 2, 4, 8, 16, 32);
                this.scrollModel.ZoomLevels = this.ZoomLevels;
            }
            else
            {
                this.ZoomLevels = this.scrollModel.ZoomLevels;
            }
            if (this.scrollModel.ZoomIndex < 0 || this.scrollModel.ZoomIndex >= this.ZoomLevels.Count)
            {
                this.ZoomIndex = 3;
                this.scrollModel.ZoomIndex = this.ZoomIndex;
            }
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";

            this.ShowGridCommand = new DelegateCommand(
                () => DrawGrid(this.IsGridOn));
            this.UpdatePositionCommand = new DelegateCommand<object>(
                (point) => UpdateMousePosition((Point)point));
            this.ZoomInCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(
                (zoomLevel) => this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel));
            this.DrawCommand = new DelegateCommand<object>(
                (point) => Draw((Point)point));
            this.DrawReleaseCommand = new DelegateCommand<object>(
                (point) => DrawRelease((Point)point));

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Subscribe(
                UpdateBrushImage,
                ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        public ICommand ShowGridCommand { get; private set; }
        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }
        public ICommand DrawCommand { get; private set; }
        public ICommand DrawReleaseCommand { get; private set; }

        public bool IsGridOn { get; set; }
        public string PositionText { get; set; }
        public ScaleType Scale { get; set; }
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
                        DrawGrid();
                    }),
                    DispatcherPriority.Background);
                }
            }
        }

        public Map Map { get; set; }
        public Layer CurrentLayer { get; set; }
        public DrawingImage DrawingCanvas { get; set; }
        
        private Brush backgroundBrush;
        public Brush BackgroundBrush
        {
            get
            {
                return backgroundBrush;
            }
            set
            {
                this.backgroundBrush = value;
                this.drawBackground(this.backgroundBrush, this.BackgroundPen);
            }
        }

        private Pen backgroundPen;
        public Pen BackgroundPen
        {
            get
            {
                return backgroundPen;
            }
            set
            {
                this.backgroundPen = value;
                this.drawBackground(this.backgroundBrush, this.BackgroundPen);
            }
        }

        #endregion properties


        #region methods

        public void UpdateBrushImage(UpdateBrushMessage brushMessage)
        {
            this.brush = brushMessage.BrushModel;
        }

        public void Draw(Point point)
        {
            if (this.brush == null)
            {
                return;
            }

            // TODO force images into tiles
            Console.WriteLine("Not Transformed: " + point);
            Point tilePoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            this.CurrentLayer.SetTile(this.brush.Image, tilePoint);
            this.layerItems.Children.Add(this.CurrentLayer.LayerItems.Drawing);
            RaisePropertyChanged(nameof(this.DrawingCanvas));
        }

        public void DrawRelease(Point point)
        {
            Console.WriteLine("Up: " + point);
        }

        public void DrawGrid()
        {
            DrawGrid(this.IsGridOn);
        }

        public void DrawGrid(bool drawGrid)
        {
            this.IsGridOn = drawGrid;
            if (this.IsGridOn)
            {
                GridModel gridParameters = new GridModel()
                {
                    rows = this.Map.Rows,
                    columns = this.Map.Columns,
                    cellWidth = this.Map.TileWidth,
                    cellHeight = this.Map.TileHeight,
                };
                gridParameters.drawingPen.Thickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
                using (DrawingContext context = this.gridLines.Open())
                {
                    context.DrawDrawing(GridModel.CreateGrid(gridParameters));
                }
            }
            else
            {
                this.gridLines.Children.Clear();
            }
            RaisePropertyChanged(nameof(this.IsGridOn));
            RaisePropertyChanged(nameof(this.DrawingCanvas));
        }

        public void UpdateMousePosition(Point position)
        {
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(position);
            }
        }

        public override void ZoomIn()
        {
            this.ZoomIndex = this.scrollModel.ZoomIn();
        }

        public override void ZoomOut()
        {
            this.ZoomIndex = this.scrollModel.ZoomOut();
        }

        public override void SetZoom(int zoomIndex)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomIndex);
        }

        public override void SetZoom(ZoomLevel zoomLevel)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel);
        }

        public override object GetContent()
        {
            return this.Map;
        }

        private void drawBackground(Brush backgroundBrush, Pen backgroundPen)
        {
            Point backgroundLocation = new Point(0, 0);
            Size backgroundSize = new Size(this.Map.GetPixelWidth(), this.Map.GetPixelHeight());
            Rect rect = new Rect(backgroundLocation, backgroundSize);
            using (DrawingContext context = this.mapBackground.Open())
            {
                context.DrawRectangle(backgroundBrush, backgroundPen, rect);
            }
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            switch (Scale)
            {
                case ScaleType.Pixel:
                    transformedPosition = position;
                    break;

                case ScaleType.Tile:
                    if (this.Map != null)
                    {
                        transformedPosition = imageTransform.PixelToTile(position);
                    }
                    break;
            }
            transformedPosition = PointUtils.IntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));

            this.updatePositionLabelStopWatch.Restart();
        }

        #endregion methods
    }
}
