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
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Docks.ItemEditorDock
{
    public class ItemEditorViewModel : DockToolViewModelTemplate
    {
        #region fields

        private CoordinateTransform itemTransform;
        private Point lastSelectPoint;

        private Mat itemImage;
        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        #endregion fields


        #region constructor

        public ItemEditorViewModel(IEventAggregator eventAggregator, IScrollModel scrollModel) : this(new TilesetModel(), eventAggregator, scrollModel)
        {
        }

        public ItemEditorViewModel(TilesetModel tilesetModel, IEventAggregator eventAggregator, IScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel");
            }
            this.TilesetModel = tilesetModel;
            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;
            this.Title = "Item - " + Path.GetFileNameWithoutExtension(tilesetModel.SourcePath);

            this.CanvasGridItems = new ObservableCollection<Visual>();
            this.CanvasSelectItems = new ObservableCollection<Visual>();
            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height);
            
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
            this.GridBrush = Brushes.Orange;
            
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.ViewPropertiesCommand = new DelegateCommand(() => ViewProperties());
            this.CropCommand = new DelegateCommand(() => Crop());
            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.RemoveItemCommand = new DelegateCommand(() => RemoveItem());
            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.ShowRulerCommand = new DelegateCommand(() => DrawRuler());
            this.ZoomInCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(
                (zoomLevel) => this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel));
            this.SetSelectPointCommand = new DelegateCommand<object>(point => SetLastSelectPoint((Point)point));
            this.SelectTilesCommand = new DelegateCommand<object>(point => SelectTiles(this.lastSelectPoint, (Point)point));
            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
            this.UpdateModelCommand = new DelegateCommand(() => updateTilesetModel());
        }

        #endregion constructor


        #region properties
        
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand ViewPropertiesCommand { get; private set; }
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

        public bool isSelectingTransparency;
        public bool IsSelectingTransparency
        {
            get
            {
                return this.isSelectingTransparency;
            }
            set
            {
                if (SetProperty(ref this.isSelectingTransparency, value))
                {
                    if (this.isSelectingTransparency)
                    {
                        Mouse.OverrideCursor = Cursors.Pen;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                    }
                }
            }
        }
        public ScaleType Scale { get; set; }
        public String PositionText { get; set; }
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
        public bool IsGridOn { get; set; }
        public ObservableCollection<Visual> CanvasGridItems { get; set; }
        public ObservableCollection<Visual> CanvasSelectItems { get; set; }
        public Brush GridBrush { get; set; }

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
            if (!this.IsSelectingTransparency)
            {
                Point tile1 = itemTransform.PixelToTileInt(point1);
                Point tile2 = itemTransform.PixelToTileInt(point2);
                Point topLeftTile = PointUtils.TopLeft(tile1, tile2);
                Point topLeftPixel = itemTransform.TileToPixel(topLeftTile);
                Size tileSize = PointUtils.SelectionSize(tile1, tile2);
                Size pixelSize = itemTransform.TileToPixel(tileSize);

                Select(topLeftPixel, pixelSize);
            }
            else
            {
                this.IsSelectingTransparency = false;
                RaisePropertyChanged(nameof(this.IsSelectingTransparency));
                RaisePropertyChanged(nameof(this.TilesetModel));
            }
        }

        public void Select(Point topLeftPixel, Size pixelSize)
        {
            DrawTileSelect(topLeftPixel, pixelSize);
            Mat croppedImage = BrushUtils.CropImage(this.itemImage, topLeftPixel, pixelSize);
            BrushModel brushModel = new BrushModel();

            if (this.TilesetModel.IsTransparent)
            {
                Mat trasparentMask = new Mat();
                Mat croppedTransparentImage = new Mat((int)pixelSize.Height, (int)pixelSize.Width, Emgu.CV.CvEnum.DepthType.Cv8U, -1);

                Color color = this.TilesetModel.TransparentColor;
                ScalarArray transparentColorLower = new ScalarArray(new MCvScalar(color.B, color.G, color.R, 0));
                ScalarArray transparentColorHigher = new ScalarArray(new MCvScalar(color.B, color.G, color.R, 255));
                CvInvoke.InRange(croppedImage, transparentColorLower, transparentColorHigher, trasparentMask);

                CvInvoke.BitwiseNot(trasparentMask, trasparentMask);
                croppedImage.CopyTo(croppedTransparentImage, trasparentMask);
                brushModel.Image = ImageUtils.MatToBitmap(croppedTransparentImage);
            }
            else
            {
                brushModel.Image = ImageUtils.MatToBitmap(croppedImage);
            }
            UpdateBrushMessage message = new UpdateBrushMessage(brushModel);
            this.eventAggregator.GetEvent<UpdateBrushEvent>().Publish(message);

            IInputArray transparencyMask = new Mat();
        }

        public void updateTilesetModel()
        {
            itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height, this.TilesetModel.OffsetX, this.TilesetModel.OffsetY, this.TilesetModel.PaddingX, this.TilesetModel.PaddingY);
            DrawGrid(this.IsGridOn);
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
                    rows = this.ItemImage.Width / this.TilesetModel.Width,
                    columns = this.ItemImage.Height / this.TilesetModel.Height,
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

        private void DrawTileSelect(Point topLeftPixel, Size pixelSize)
        {
            Rect selectionBorder = new Rect(topLeftPixel, pixelSize);
            RectangleGeometry tileGeometry = new RectangleGeometry(selectionBorder);
            
            System.Windows.Shapes.Path rectPath = new System.Windows.Shapes.Path();
            rectPath.Stroke = GridBrush;
            rectPath.StrokeThickness = 1;
            rectPath.Data = tileGeometry;

            this.CanvasSelectItems.Clear();
            this.CanvasSelectItems.Add(rectPath);

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
                    this.itemImage = CvInvoke.Imread(tileFilePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);

                    this.itemTransform = new CoordinateTransform();
                    this.itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height);

                    BitmapSource bitmapSource = BitmapFrame.Create(new Uri(tileFilePath));
                    Rect tileRectangle = new Rect(0, 0, bitmapSource.PixelWidth, bitmapSource.PixelHeight);
                    ImageDrawing tileImage = new ImageDrawing(bitmapSource, tileRectangle);
                    this.ItemImage = new DrawingImage(tileImage);

                    DrawGrid();
                    RaisePropertyChanged(nameof(this.TilesetModel));
                    RaisePropertyChanged(nameof(this.ItemImage));
                }
            }
        }

        private void AddImage()
        {
            Console.WriteLine("Add Image");
        }

        private void EditCollisions()
        {
            Console.WriteLine("Edit Collision");
        }
        
        private void ViewProperties()
        {
            OpenWindowMessage window = new OpenWindowMessage(WindowType.TilesetEditor);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(window);
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
            if (this.IsSelectingTransparency)
            {
                byte[] colorsBGR = this.itemImage.GetData((int)point.Y, (int)point.X);
                this.TilesetModel.TransparentColor = Color.FromRgb(colorsBGR[2], colorsBGR[1], colorsBGR[0]);
                Console.WriteLine(this.TilesetModel.TransparentColor);
            }
            else
            {
                this.lastSelectPoint = point;
            }
            
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
