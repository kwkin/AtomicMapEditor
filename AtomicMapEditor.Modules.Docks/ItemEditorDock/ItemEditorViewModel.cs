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
using Emgu.CV;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Docks.ItemEditorDock
{
    public class ItemEditorViewModel : DockToolViewModelTemplate
    {
        // TODO: look into switching all system.windows shapes to drawing

        #region fields

        private CoordinateTransform itemTransform;
        private Point lastSelectPoint;

        private Mat itemImage;
        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor & destructer

        public ItemEditorViewModel(IEventAggregator eventAggregator) : this(new TilesetModel(), eventAggregator)
        {
        }

        public ItemEditorViewModel(TilesetModel tilesetModel, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.TilesetModel = tilesetModel;
            this.eventAggregator = eventAggregator;
            this.Title = "Item - " + Path.GetFileNameWithoutExtension(tilesetModel.SourcePath);

            this.CanvasGridItems = new ObservableCollection<Visual>();
            this.CanvasSelectItems = new ObservableCollection<Visual>();
            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height);

            this.ZoomLevels = new List<ZoomLevel>();
            this.ZoomLevels.Add(new ZoomLevel("12.5%", 0.125));
            this.ZoomLevels.Add(new ZoomLevel("25%", 0.25));
            this.ZoomLevels.Add(new ZoomLevel("50%", 0.5));
            this.ZoomLevels.Add(new ZoomLevel("100%", 1));
            this.ZoomLevels.Add(new ZoomLevel("200%", 2));
            this.ZoomLevels.Add(new ZoomLevel("400%", 4));
            this.ZoomLevels.Add(new ZoomLevel("800%", 8));
            this.ZoomLevels.Add(new ZoomLevel("1600%", 16));
            this.ZoomLevels.Add(new ZoomLevel("3200%", 32));
            this.ZoomLevels = this.ZoomLevels.OrderBy(f => f.zoom).ToList();
            this.ZoomIndex = 3;
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";

            this.ViewPropertiesCommand = new DelegateCommand(() => SetMapProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.CropCommand = new DelegateCommand(() => Crop());
            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.RemoveItemCommand = new DelegateCommand(() => RemoveItem());
            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.ShowRulerCommand = new DelegateCommand(() => DrawRuler());
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(zoomLevel => SetZoom(zoomLevel));
            this.SetSelectPointCommand = new DelegateCommand<object>(point => SetLastSelectPoint((Point)point));
            this.SelectTilesCommand = new DelegateCommand<object>(point => SelectTiles(this.lastSelectPoint, (Point)point));
            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
            this.UpdateModelCommand = new DelegateCommand(() => updateTilesetModel());
        }

        #endregion constructor & destructer


        #region properties

        public ICommand ViewPropertiesCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand CropCommand { get; private set; }
        public ICommand AddTilesetCommand { get; private set; }
        public ICommand AddImageCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand ShowRulerCommand { get; private set; }
        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }
        public ICommand SetSelectPointCommand { get; private set; }
        public ICommand SelectTilesCommand { get; private set; }
        public ICommand UpdateModelCommand { get; private set; }

        public ScaleType Scale { get; set; }
        public String PositionText { get; set; }
        public List<ZoomLevel> ZoomLevels { get; set; }
        public int _ZoomIndex;
        public int ZoomIndex
        {
            get { return this._ZoomIndex; }
            set
            {
                if (SetProperty(ref this._ZoomIndex, value))
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
        public bool IsGridOn { get; set; }
        public ObservableCollection<Visual> CanvasGridItems { get; set; }
        public ObservableCollection<Visual> CanvasSelectItems { get; set; }

        public TilesetModel TilesetModel { get; set; }
        public DrawingImage ItemImage { get; set; }
        
        public override DockType DockType
        {
            get
            {
                return DockType.ItemEditor;
            }
        }

        #endregion properties


        #region methods

        /// <summary>
        /// Selects all tiles between the two pixel points.
        /// </summary>
        /// <param name="point1">First corner pixel point in the selection</param>
        /// <param name="point2">Second corner pixel point in the selection</param>
        public void SelectTiles(Point point1, Point point2)
        {
            Point tile1 = itemTransform.PixelToTileInt(point1);
            Point tile2 = itemTransform.PixelToTileInt(point2);
            Point topLeftTile = PointUtils.TopLeft(tile1, tile2);
            Point topLeftPixel = itemTransform.TileToPixel(topLeftTile);
            Size tileSize = PointUtils.SelectionSize(tile1, tile2);
            Size pixelSize = itemTransform.TileToPixel(tileSize);

            Select(topLeftPixel, pixelSize);
        }

        public void Select(Point topLeftPixel, Size pixelSize)
        {
            // TODO set image transparency
            DrawTileSelect(topLeftPixel, pixelSize);
            Mat croppedImage = BrushUtils.CropImage(this.itemImage, topLeftPixel, pixelSize);
            BrushModel brushModel = new BrushModel();
            brushModel.image = croppedImage;
            UpdateBrushMessage message = new UpdateBrushMessage(brushModel);
            this.eventAggregator.GetEvent<UpdateBrushEvent>().Publish(message);
        }

        public void updateTilesetModel()
        {
            itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height, this.TilesetModel.OffsetX, this.TilesetModel.OffsetY, this.TilesetModel.PaddingX, this.TilesetModel.PaddingY);
            DrawGrid(this.IsGridOn);
        }

        // TODO have a common grid class
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
                    width = this.ItemImage.Width,
                    height = this.ItemImage.Height,
                    cellWidth = this.TilesetModel.Width,
                    cellHeight = this.TilesetModel.Height,
                    offsetX = this.TilesetModel.OffsetX,
                    offsetY = this.TilesetModel.OffsetY,
                    paddingX = this.TilesetModel.PaddingX,
                    paddingY = this.TilesetModel.PaddingY
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

        public void DrawRuler()
        {
            Console.WriteLine("DrawRuler");
        }

        public void ZoomIn()
        {
            if (this.ZoomIndex < this.ZoomLevels.Count - 1)
            {
                this.ZoomIndex += 1;
                RaisePropertyChanged(nameof(this.ZoomIndex));
            }
        }

        public void ZoomOut()
        {
            if (this.ZoomIndex > 0)
            {
                this.ZoomIndex -= 1;
                RaisePropertyChanged(nameof(this.ZoomIndex));
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
            RaisePropertyChanged(nameof(this.ZoomIndex));
        }

        /// <summary>
        /// </summary>
        /// <param name="topLeftPixel">Top left pixel in the selection</param>
        /// <param name="pixelSize">Size of the selection in pixels</param>
        private void DrawTileSelect(Point topLeftPixel, Size pixelSize)
        {
            Rect selectionBorder = new Rect(topLeftPixel, pixelSize);
            RectangleGeometry tileGeometry = new RectangleGeometry(selectionBorder);

            // TODO have getters/setters for the stroke brush and thickness
            System.Windows.Shapes.Path rectPath = new System.Windows.Shapes.Path();
            rectPath.Stroke = Brushes.Orange;
            rectPath.StrokeThickness = 1;
            rectPath.Data = tileGeometry;

            CanvasSelectItems.Clear();
            CanvasSelectItems.Add(rectPath);

            RaisePropertyChanged(nameof(this.CanvasSelectItems));
        }

        private void AddTileset()
        {
            OpenFileDialog openTilesetDilog = new OpenFileDialog();
            openTilesetDilog.Title = "Select a Tileset";
            openTilesetDilog.Filter = ImageExtension.GetOpenFileImageExtensions();
            if (openTilesetDilog.ShowDialog() == true)
            {
                string tileFilePath = openTilesetDilog.FileName;
                if (File.Exists(tileFilePath))
                {
                    this.Title = "Item - " + Path.GetFileNameWithoutExtension(tileFilePath);
                    this.TilesetModel.SourcePath = tileFilePath;
                    this.itemImage = CvInvoke.Imread(tileFilePath, Emgu.CV.CvEnum.ImreadModes.AnyColor);

                    this.itemTransform = new CoordinateTransform();
                    this.itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height);

                    BitmapSource bitmapSource = BitmapFrame.Create(new Uri(tileFilePath));
                    Rect tileRectangle = new Rect(0, 0, bitmapSource.PixelWidth, bitmapSource.PixelHeight);
                    ImageDrawing tileImage = new ImageDrawing(bitmapSource, tileRectangle);
                    this.ItemImage = new DrawingImage(tileImage);

                    RaisePropertyChanged(nameof(this.TilesetModel));
                    RaisePropertyChanged(nameof(this.ItemImage));
                }
            }
        }

        private void AddImage()
        {
            Console.WriteLine("Add Image");
        }

        private void SetMapProperties()
        {
            Console.WriteLine("Set Map Properties");
        }

        private void EditCollisions()
        {
            Console.WriteLine("Edit Collision");
        }

        private void Crop()
        {
            Console.WriteLine("Crop ");
        }

        private void RemoveItem()
        {
            Console.WriteLine("Remove Item");
        }

        private void SetLastSelectPoint(Point point)
        {
            this.lastSelectPoint = point;
        }

        private void UpdatePosition(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.ItemImage != null)
            {
                switch (Scale)
                {
                    case ScaleType.Pixel:
                        transformedPosition = position;
                        break;

                    case ScaleType.Tile:
                        transformedPosition = itemTransform.PixelToTile(position);
                        break;
                }
            }
            transformedPosition = PointUtils.IntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
        }

        #endregion methods
    }
}
