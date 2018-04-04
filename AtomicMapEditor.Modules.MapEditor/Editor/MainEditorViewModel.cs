using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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

        public BrushModel brush;
        private CoordinateTransform imageTransform;
        private DrawingGroup imageDrawings;

        #endregion fields


        #region Constructor & destructor

        public MainEditorViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.Map = new Map();
            this.eventAggregator = eventAggregator;
            this.Title = "Main Editor";
            
            this.imageTransform = new CoordinateTransform();

            // TODO pass MapModel
            this.CanvasGridItems = new ObservableCollection<Visual>();
            this.imageTransform.SetPixelToTile(this.Map.TileWidth, this.Map.TileHeight);

            // Draw map background
            GeometryGroup rectangles = new GeometryGroup();
            rectangles.Children.Add(new RectangleGeometry(new Rect(0, 0, this.Map.Width, this.Map.Height)));
            GeometryDrawing aGeometryDrawing = new GeometryDrawing();
            aGeometryDrawing.Geometry = rectangles;
            aGeometryDrawing.Brush = new SolidColorBrush(Colors.AliceBlue);
            this.imageDrawings = new DrawingGroup();
            this.imageDrawings.Children.Add(aGeometryDrawing);
            this.MapItems = new DrawingImage(this.imageDrawings);

            this.ZoomLevels = new List<ZoomLevel>();
            this.ZoomLevels.Add(new ZoomLevel(0.125));
            this.ZoomLevels.Add(new ZoomLevel(0.25));
            this.ZoomLevels.Add(new ZoomLevel(0.5));
            this.ZoomLevels.Add(new ZoomLevel(1));
            this.ZoomLevels.Add(new ZoomLevel(2));
            this.ZoomLevels.Add(new ZoomLevel(4));
            this.ZoomLevels.Add(new ZoomLevel(8));
            this.ZoomLevels.Add(new ZoomLevel(16));
            this.ZoomLevels.Add(new ZoomLevel(32));
            this.ZoomLevels = this.ZoomLevels.OrderBy(f => f.zoom).ToList();
            this.ZoomIndex = 3;
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";

            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(zoomLevel => SetZoom(zoomLevel));
            this.DrawCommand = new DelegateCommand<object>(point => Draw((Point)point));
            this.DrawReleaseCommand = new DelegateCommand<object>(point => DrawRelease((Point)point));

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Subscribe(UpdateBrushImage);
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
        public List<ZoomLevel> ZoomLevels { get; set; }
        
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

        // TODO determine a better name for map model
        public Map Map { get; set; }

        public DrawingImage MapItems { get; set; }

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
            BitmapImage croppedBitmap = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                this.brush.image.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                croppedBitmap.BeginInit();
                croppedBitmap.StreamSource = ms;
                croppedBitmap.CacheOption = BitmapCacheOption.OnLoad;
                croppedBitmap.EndInit();
            }
            Point tilePoint = this.imageTransform.PixelToTileEdge(point);

            Rect rect = new Rect(tilePoint.X, tilePoint.Y, this.brush.image.Width, this.brush.image.Height);
            ImageDrawing tileImage = new ImageDrawing(croppedBitmap, rect);
            this.imageDrawings.Children.Add(tileImage);

            this.MapItems = new DrawingImage(this.imageDrawings);
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
                    width = this.Map.Width,
                    height = this.Map.Height,
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

        public void ZoomIn()
        {
            if (this.ZoomIndex < this.ZoomLevels.Count - 1)
            {
                this.ZoomIndex += 1;
            }
        }

        public void ZoomOut()
        {
            if (this.ZoomIndex > 0)
            {
                this.ZoomIndex -= 1;
            }
        }

        public void SetZoom(ZoomLevel selectedZoomLevel)
        {
            int zoomIndex = this.ZoomLevels.FindIndex(r => r.zoom == selectedZoomLevel.zoom);
            if (zoomIndex == -1)
            {
                this.ZoomLevels.Add(selectedZoomLevel);
                zoomIndex = this.ZoomLevels.FindIndex(r => r.zoom == selectedZoomLevel.zoom);
            }
            if (zoomIndex > ZoomLevels.Count - 1)
            {
                zoomIndex = ZoomLevels.Count - 1;
            }
            else if (zoomIndex < 0)
            {
                zoomIndex = 0;
            }
            this.ZoomIndex = zoomIndex;
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
