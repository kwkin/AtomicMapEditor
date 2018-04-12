using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.MapEditor.Editor
{
    public class MainEditorViewModel : EditorViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        public BrushModel brush;
        private CoordinateTransform imageTransform;
        private DrawingGroup imageDrawings;

        #endregion fields


        #region Constructor & destructor

        public MainEditorViewModel(IEventAggregator eventAggregator, IScrollModel scrollModel, Map map)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel");
            }
            this.Map = map;
            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;
            this.Title = "Main Editor";
            this.CurrentLayer = this.Map.LayerList[0];

            this.imageTransform = new CoordinateTransform();
            this.imageTransform.SetPixelToTile(this.Map.TileWidth, this.Map.TileHeight);
            this.CanvasGridItems = new ObservableCollection<Visual>();

            // Draw map background
            GeometryGroup rectangles = new GeometryGroup();
            rectangles.Children.Add(new RectangleGeometry(new Rect(0, 0, this.Map.GetPixelWidth(), this.Map.GetPixelHeight())));
            GeometryDrawing aGeometryDrawing = new GeometryDrawing();
            aGeometryDrawing.Geometry = rectangles;
            aGeometryDrawing.Brush = new SolidColorBrush(Colors.AliceBlue);
            this.imageDrawings = new DrawingGroup();
            this.imageDrawings.Children.Add(aGeometryDrawing);
            this.MapBackground = new DrawingImage(this.imageDrawings);

            if (this.scrollModel.ZoomLevels == null)
            {
                this.ZoomLevels = new ObservableCollection<ZoomLevel>();
                this.ZoomLevels.Add(new ZoomLevel(0.125));
                this.ZoomLevels.Add(new ZoomLevel(0.25));
                this.ZoomLevels.Add(new ZoomLevel(0.5));
                this.ZoomLevels.Add(new ZoomLevel(1));
                this.ZoomLevels.Add(new ZoomLevel(2));
                this.ZoomLevels.Add(new ZoomLevel(4));
                this.ZoomLevels.Add(new ZoomLevel(8));
                this.ZoomLevels.Add(new ZoomLevel(16));
                this.ZoomLevels.Add(new ZoomLevel(32));
                this.ZoomLevels.OrderBy(f => f.zoom);
                this.scrollModel.ZoomLevels = this.ZoomLevels;
            }
            if (this.scrollModel.ZoomIndex < 0 || this.scrollModel.ZoomIndex >= this.ZoomLevels.Count)
            {
                this.ZoomIndex = 3;
                this.scrollModel.ZoomIndex = this.ZoomIndex;
            }
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";

            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
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

        #endregion Constructor & destructor


        #region properties

        public ICommand ShowGridCommand { get; private set; }
        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }
        public ICommand DrawCommand { get; private set; }
        public ICommand DrawReleaseCommand { get; private set; }

        public bool IsGridOn { get; set; }
        public ObservableCollection<Visual> CanvasGridItems { get; set; }
        public String PositionText { get; set; }
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
                    this.CanvasGridItems.Clear();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DrawGrid();
                    }),
                    DispatcherPriority.Background);
                }
            }
        }

        public override DockType DockType
        {
            get
            {
                return DockType.MapEditor;
            }
        }

        public Map Map { get; set; }
        public Layer CurrentLayer { get; set; }
        
        public DrawingImage MapBackground { get; set; }

        // TODO switch between layers 
        // TODO add drawing images to a canvas, set the z index of the individual drawing images to its position in the list
        public DrawingImage LayerItems { get; set; }

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
            BitmapImage croppedBitmap = this.brush.Image;
            Point tilePoint = this.imageTransform.PixelToTileEdge(point);

            this.CurrentLayer.SetTile(croppedBitmap, tilePoint);
            this.LayerItems = this.CurrentLayer.LayerItems;
            RaisePropertyChanged(nameof(this.LayerItems));
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
                // TODO, update the grid model with columns and rows instead of pixel width and height
                GridModel gridParameters = new GridModel()
                {
                    width = this.Map.GetPixelWidth(),
                    height = this.Map.GetPixelHeight(),
                    cellWidth = this.Map.TileWidth,
                    cellHeight = this.Map.TileHeight,
                };
                GridFactory.StrokeThickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
                this.CanvasGridItems = GridFactory.CreateGrid(gridParameters);
            }
            else
            {
                this.CanvasGridItems.Clear();
            }
            RaisePropertyChanged(nameof(this.IsGridOn));
            RaisePropertyChanged(nameof(this.CanvasGridItems));
        }

        private void UpdatePosition(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            switch (Scale)
            {
                case ScaleType.Pixel:
                    transformedPosition = position;
                    break;

                case ScaleType.Tile:
                    transformedPosition = position;
                    break;
            }
            transformedPosition = PointUtils.IntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
        }

        #endregion methods
    }
}
