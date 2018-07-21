using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using Ame.Modules.Windows.Interactions.TilesetEditorInteraction;
using Emgu.CV;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemEditorDock
{
    // TODO fix application crash when closing docks with multiple tabs
    // TODO add image loading
    public class ItemEditorViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private CoordinateTransform itemTransform;
        private Point lastSelectPoint;
        private bool isSelecting;
        private bool isMouseDown;
        private Rect selectionBorder;
        private int zoomIndex;
        private bool isSelectingTransparency;
        private TilesetModel tilesetModel;
        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private long drawSelectLineDelay = Global.defaultDrawSelectLineDelay;
        private Stopwatch updatePositionLabelStopWatch;
        private Stopwatch selectLineStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup tilesetImage;
        private DrawingGroup gridLines;
        private DrawingGroup selectLines;
        private DrawingGroup extendedBorder;
        private Pen gridPen;
        private Brush backgroundBrush;
        private Pen backgroundPen;

        #endregion fields


        #region constructor

        public ItemEditorViewModel(IEventAggregator eventAggregator, AmeSession session) : this(eventAggregator, session, new TilesetModel(), new ScrollModel())
        {
        }

        public ItemEditorViewModel(IEventAggregator eventAggregator, AmeSession session, TilesetModel tilesetModel) : this(eventAggregator, session, tilesetModel, new ScrollModel())
        {
        }

        public ItemEditorViewModel(IEventAggregator eventAggregator, AmeSession session, IScrollModel scrollModel) : this(eventAggregator, session, new TilesetModel(), scrollModel)
        {
        }

        public ItemEditorViewModel(IEventAggregator eventAggregator, AmeSession session, TilesetModel tilesetModel, IScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel");
            }
            if (tilesetModel == null)
            {
                throw new ArgumentNullException("tilesetModel");
            }
            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;
            this.TilesetModel = tilesetModel;
            this.Session = session;

            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.TileWidth, this.TilesetModel.TileHeight);

            this.Title = "Item - " + Path.GetFileNameWithoutExtension(tilesetModel.SourcePath);
            this.drawingGroup = new DrawingGroup();
            this.tilesetImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.selectLines = new DrawingGroup();
            this.extendedBorder = new DrawingGroup();
            this.drawingGroup.Children.Add(this.extendedBorder);
            this.drawingGroup.Children.Add(this.tilesetImage);
            this.drawingGroup.Children.Add(this.gridLines);
            this.drawingGroup.Children.Add(this.selectLines);
            this.TileImage = new DrawingImage(this.drawingGroup);
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
            this.GridPen = new Pen(Brushes.Orange, 1);
            this.backgroundBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#a5efda"));
            this.backgroundPen = new Pen(Brushes.Transparent, 0);
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();
            this.selectLineStopWatch = Stopwatch.StartNew();

            this.HandleLeftClickDownCommand = new DelegateCommand<object>(
                (point) => HandleLeftClickDown((Point)point));
            this.HandleLeftClickUpCommand = new DelegateCommand<object>(
                (point) => HandleLeftClickUp((Point)point));
            this.HandleMouseMoveCommand = new DelegateCommand<object>(
                (point) => HandleMouseMove((Point)point));
            this.EditCollisionsCommand = new DelegateCommand(
                () => EditCollisions());
            this.ViewPropertiesCommand = new DelegateCommand(
                () => ViewProperties());
            this.CropCommand = new DelegateCommand(
                () => Crop());
            this.AddTilesetCommand = new DelegateCommand(
                () => AddTileset());
            this.AddImageCommand = new DelegateCommand(
                () => AddImage());
            this.RemoveItemCommand = new DelegateCommand(
                () => RemoveItem());
            this.ShowGridCommand = new DelegateCommand(
                () => DrawGrid(this.IsGridOn));
            this.ShowRulerCommand = new DelegateCommand(
                () => DrawRuler());
            this.ZoomInCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(
                (zoomLevel) => this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel));
            this.UpdateModelCommand = new DelegateCommand(
                () => UpdateTilesetModel());
            this.ChangeItemCommand = new DelegateCommand(
                () => UpdateItemModel());
        }

        #endregion constructor


        #region properties

        public ICommand HandleLeftClickDownCommand { get; private set; }
        public ICommand HandleLeftClickUpCommand { get; private set; }
        public ICommand HandleMouseMoveCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand ViewPropertiesCommand { get; private set; }
        public ICommand CropCommand { get; private set; }
        public ICommand AddTilesetCommand { get; private set; }
        public ICommand AddImageCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand ShowRulerCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }
        public ICommand UpdateModelCommand { get; private set; }
        public ICommand ChangeItemCommand { get; private set; }

        public AmeSession Session { get; set; }

        public DrawingImage TileImage { get; set; }
        public bool IsGridOn { get; set; }
        public bool IsRulerOn { get; set; }
        public string PositionText { get; set; }
        public ScaleType Scale { get; set; }
        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }
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

        private Mat itemImage;
        public Mat ItemImage
        {
            get
            {
                return this.itemImage;
            }
            set
            {
                SetProperty(ref this.itemImage, value);
            }
        }

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

        public TilesetModel TilesetModel
        {
            get
            {
                return tilesetModel;
            }
            set
            {
                if (SetProperty(ref this.tilesetModel, value))
                {
                    if (!string.IsNullOrEmpty(this.tilesetModel.SourcePath))
                    {
                        this.ItemImage = CvInvoke.Imread(this.tilesetModel.SourcePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
                    }
                }
            }
        }

        public bool IsTransparent
        {
            get
            {
                return this.TilesetModel.IsTransparent;
            }
            set
            {
                this.TilesetModel.IsTransparent = value;
            }
        }

        public Color TransparentColor
        {
            get
            {
                return this.TilesetModel.TransparentColor;
            }
            set
            {
                this.TilesetModel.TransparentColor = value;
            }
        }

        public Pen GridPen
        {
            get
            {
                return gridPen;
            }
            set
            {
                this.gridPen = value;
            }
        }

        public Brush BackgroundBrush
        {
            get
            {
                return backgroundBrush;
            }
            set
            {
                this.backgroundBrush = value;
                DrawBackground(this.backgroundBrush, this.BackgroundPen);
            }
        }

        public Pen BackgroundPen
        {
            get
            {
                return backgroundPen;
            }
            set
            {
                this.backgroundPen = value;
                DrawBackground(this.backgroundBrush, this.BackgroundPen);
            }
        }

        #endregion properties


        #region methods

        public void HandleLeftClickUp(Point selectPoint)
        {
            this.isMouseDown = false;
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            if (!this.IsSelectingTransparency)
            {
                this.isSelecting = false;
                SelectTiles(pixelPoint, this.lastSelectPoint);
            }
            else
            {
                SelectTransparency(pixelPoint);
                this.IsSelectingTransparency = false;
            }
        }

        public void HandleLeftClickDown(Point selectPoint)
        {
            this.isMouseDown = true;
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            selectPoint = selectToPixel.Transform(selectPoint);
            if (!ImageUtils.Intersects(this.ItemImage, selectPoint))
            {
                return;
            }
            if (!this.IsSelectingTransparency)
            {
                this.isSelecting = true;
                ComputeSelectLinesFromPixels(selectPoint, selectPoint);
                this.lastSelectPoint = selectPoint;
            }
        }

        public void HandleMouseMove(Point selectPosition)
        {
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPosition);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(pixelPoint);
                if (this.IsSelectingTransparency && this.isMouseDown)
                {
                    PickTransparentColor(pixelPoint);
                }
            }
            if (this.isSelecting
                && this.selectLineStopWatch.ElapsedMilliseconds > this.drawSelectLineDelay
                && ImageUtils.Intersects(this.ItemImage, pixelPoint))
            {
                this.ComputeSelectLinesFromPixels(this.lastSelectPoint, pixelPoint);
            }
        }

        public void SelectTiles(Point pixelPoint1, Point pixelPoint2)
        {
            if (!ImageUtils.Intersects(this.ItemImage, pixelPoint1) 
                || !ImageUtils.Intersects(this.ItemImage, pixelPoint2))
            {
                return;
            }
            GeneralTransform pixelToTile = GeometryUtils.CreateTransform(this.itemTransform.pixelToTile);
            Point tile1 = GeometryUtils.TransformInt(pixelToTile, pixelPoint1);
            Point tile2 = GeometryUtils.TransformInt(pixelToTile, pixelPoint2);
            Point topLeftTile = GeometryUtils.TopLeftPoint(tile1, tile2);
            Point topLeftPixel = GeometryUtils.TransformInt(pixelToTile.Inverse, topLeftTile);
            Size tileSize = GeometryUtils.ComputeSize(tile1, tile2);
            Size pixelSize = GeometryUtils.TransformInt(pixelToTile.Inverse, tileSize);

            this.DrawSelectLinesFromPixels(topLeftPixel, pixelSize);
            BrushModel brushModel = new BrushModel(this.tilesetModel);
            Mat croppedImage = BrushUtils.CropImage(this.ItemImage, topLeftPixel, pixelSize);

            if (this.IsTransparent)
            {
                croppedImage = ImageUtils.ColorToTransparent(croppedImage, this.TransparentColor);
            }
            brushModel.Image = ImageUtils.MatToImageDrawing(croppedImage);

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Publish(brushModel);
        }

        public void SelectTransparency(Point pixelPoint)
        {
            if (!ImageUtils.Intersects(this.ItemImage, pixelPoint))
            {
                return;
            }
            PickTransparentColor(pixelPoint);
            Mat transparentImage = ImageUtils.ColorToTransparent(this.ItemImage, this.TransparentColor);
            using (DrawingContext context = this.tilesetImage.Open())
            {
                context.DrawDrawing(ImageUtils.MatToImageDrawing(transparentImage));
            }
            RaisePropertyChanged(nameof(this.TransparentColor));
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
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.TilesetModel);
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
            RaisePropertyChanged(nameof(this.TileImage));
        }

        public void DrawRuler()
        {
            DrawRuler(this.IsRulerOn);
        }

        public void DrawRuler(bool drawRuler)
        {
            Console.WriteLine("DrawRuler");
        }

        public void ZoomIn()
        {
            this.ZoomIndex = this.scrollModel.ZoomIn();
        }

        public void ZoomOut()
        {
            this.ZoomIndex = this.scrollModel.ZoomOut();
        }

        public void SetZoom(int zoomIndex)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomIndex);
        }

        public void SetZoom(ZoomLevel zoomLevel)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel);
        }

        public void AddTileset()
        {
            OpenFileDialog openTilesetDilog = new OpenFileDialog();
            openTilesetDilog.Title = "Select a Tileset";
            openTilesetDilog.Filter = ImageExtension.GetOpenFileImageExtensions();
            if (openTilesetDilog.ShowDialog() == true)
            {
                string tileFilePath = openTilesetDilog.FileName;
                if (File.Exists(tileFilePath))
                {
                    string tilesetName = string.Format("Tileset #{0}", this.Session.CurrentTilesetCount);
                    TilesetModel newTileset = new TilesetModel(tilesetName, tileFilePath);
                    this.Title = "Item - " + Path.GetFileNameWithoutExtension(tileFilePath);

                    Mat matImage = CvInvoke.Imread(tileFilePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
                    this.itemTransform = new CoordinateTransform();
                    newTileset.PixelWidth = matImage.Width;
                    newTileset.PixelHeight = matImage.Height;
                    this.itemTransform.SetPixelToTile(newTileset.TileWidth, newTileset.TileHeight);
                    this.itemTransform.SetSlectionToPixel(newTileset.TileWidth / 2, newTileset.TileHeight / 2);
                    this.Session.CurrentTilesetList.Add(newTileset);
                    this.TilesetModel = newTileset;
                    
                    DrawBackground(this.BackgroundBrush, this.BackgroundPen);
                    DrawGrid();
                    RaisePropertyChanged(nameof(this.TileImage));
                }
            }
        }

        public void AddImage()
        {
            Console.WriteLine("Add Image");
        }

        public void UpdateItemModel()
        {
            ChangeItemModel(this.TilesetModel);
        }

        public void ChangeItemModel(TilesetModel tilesetModel)
        {
            if (!this.Session.CurrentTilesetList.Contains(tilesetModel))
            {
                this.Session.CurrentTilesetList.Add(tilesetModel);
            }
            this.TilesetModel = tilesetModel;
            this.Session.CurrentTileset = this.TilesetModel;
            this.ItemImage = CvInvoke.Imread(this.TilesetModel.SourcePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
            DrawingGroup newGroup = ImageUtils.MatToDrawingGroup(this.ItemImage);
            this.TilesetModel.TilesetImage = newGroup;
            using (DrawingContext context = this.tilesetImage.Open())
            {
                context.DrawDrawing(this.TilesetModel.TilesetImage);
            }
        }

        private void ComputeSelectLinesFromPixels(Point pixelPoint1, Point pixelPoint2)
        {
            Point pixel1 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint1);
            Point pixel2 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint2);
            Point topLeftPixel = GeometryUtils.TopLeftPoint(pixel1, pixel2);
            Point bottomRightPixel = GeometryUtils.BottomRightPoint(pixel1, pixel2);
            bottomRightPixel = this.itemTransform.PixelToBottomRightTileEdge(bottomRightPixel);

            if (topLeftPixel != this.selectionBorder.TopLeft || bottomRightPixel != this.selectionBorder.BottomRight)
            {
                DrawSelectLinesFromPixels(topLeftPixel, bottomRightPixel);
            }
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.TileImage != null)
            {
                switch (Scale)
                {
                    case ScaleType.Pixel:
                        transformedPosition = position;
                        break;

                    case ScaleType.Tile:
                        GeneralTransform transform = GeometryUtils.CreateTransform(this.itemTransform.pixelToTile);
                        transformedPosition = transform.Transform(position);
                        break;
                }
            }
            transformedPosition = GeometryUtils.CreateIntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
            updatePositionLabelStopWatch.Restart();
        }

        private void UpdateTilesetModel()
        {
            this.itemTransform.SetPixelToTile(this.TilesetModel.TileWidth, this.TilesetModel.TileHeight, this.TilesetModel.OffsetX, this.TilesetModel.OffsetY, this.TilesetModel.PaddingX, this.TilesetModel.PaddingY);
            DrawGrid(this.IsGridOn);
        }

        private void DrawSelectLinesFromPixels(Point topLeftPixel, Size pixelSize)
        {
            this.selectionBorder = new Rect(topLeftPixel, pixelSize);
            using (DrawingContext context = this.selectLines.Open())
            {
                context.DrawRectangle(Brushes.Transparent, this.GridPen, this.selectionBorder);
            }
            RaisePropertyChanged(nameof(this.TileImage));
            this.selectLineStopWatch.Restart();
        }

        private void DrawSelectLinesFromPixels(Point topLeftPixel, Point bottomRightPixel)
        {
            this.selectionBorder = new Rect(topLeftPixel, bottomRightPixel);
            using (DrawingContext context = this.selectLines.Open())
            {
                context.DrawRectangle(Brushes.Transparent, this.GridPen, this.selectionBorder);
            }
            RaisePropertyChanged(nameof(this.TileImage));
            this.selectLineStopWatch.Restart();
        }

        private void EditCollisions()
        {
            Console.WriteLine("Edit Collision");
        }

        private void ViewProperties()
        {
            if (this.Session.CurrentTilesetCount == 0)
            {
                return;
            }
            OpenWindowMessage window = new OpenWindowMessage(typeof(EditTilesetInteraction));
            window.Title = string.Format("Tileset Properties - {0}", this.TilesetModel.Name);
            window.Content = this.TilesetModel;
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

        private void DrawBackground(Brush backgroundBrush, Pen backgroundPen)
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.tilesetModel.PixelWidth + this.tilesetModel.TileWidth;
            extendedSize.Height = this.tilesetModel.PixelHeight + this.tilesetModel.TileHeight;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.tilesetModel.TileWidth / 2;
            extendedPoint.Y = -this.tilesetModel.TileHeight / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            Size backgroundSize = new Size();
            backgroundSize.Width = this.tilesetModel.PixelWidth;
            backgroundSize.Height = this.tilesetModel.PixelHeight;
            Point backgroundPoint = new Point();
            backgroundPoint.X = 0;
            backgroundPoint.Y = 0;
            Rect backgroundRectangle = new Rect(backgroundPoint, backgroundSize);
            using (DrawingContext context = this.extendedBorder.Open())
            {
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                context.DrawRectangle(backgroundBrush, backgroundPen, backgroundRectangle);
            }
        }

        private void PickTransparentColor(Point pixelPoint)
        {
            byte[] colorsBGR = this.ItemImage.GetData((int)pixelPoint.Y, (int)pixelPoint.X);
            this.TransparentColor = Color.FromRgb(colorsBGR[2], colorsBGR[1], colorsBGR[0]);
            RaisePropertyChanged(nameof(this.TransparentColor));
        }

        #endregion methods
    }
}
