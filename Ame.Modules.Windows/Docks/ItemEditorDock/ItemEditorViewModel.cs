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
using Emgu.CV.Structure;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemEditorDock
{
    // TODO convert image formats to Mat for models and DrawingImage for views
    // TODO fix image adjust when selecting the outside border
    // TODO organize properties
    public class ItemEditorViewModel : DockToolViewModelTemplate
    {
        #region fields
        
        private Mat itemImage;
        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private DrawingGroup drawingGroup;
        private DrawingGroup tilesetImage;
        private DrawingGroup gridLines;
        private DrawingGroup selectLines;

        private CoordinateTransform itemTransform;
        private Point lastSelectPoint;
        private bool isSelecting;
        private Rect selectionBorder;

        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private long drawSelectLineDelay = Global.defaultDrawSelectLineDelay;
        private Stopwatch updatePositionLabelStopWatch;
        private Stopwatch selectLineStopWatch;

        private Vector offsetVector;

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

            this.Title = "Item - " + Path.GetFileNameWithoutExtension(tilesetModel.SourcePath);
            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height);

            this.TileImage = new DrawingImage();
            this.drawingGroup = new DrawingGroup();
            this.tilesetImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.selectLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.tilesetImage);
            this.drawingGroup.Children.Add(this.gridLines);
            this.drawingGroup.Children.Add(this.selectLines);
            this.TileImage.Drawing = this.drawingGroup;

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
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();
            this.selectLineStopWatch = Stopwatch.StartNew();

            this.HandleLeftClickDownCommand = new DelegateCommand<object>(
                (point) => HandleLeftClickDown((Point)point));
            this.HandleLeftClickUpCommand = new DelegateCommand<object>(
                (point) => HandleLeftClickUp(this.lastSelectPoint, (Point)point));
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
            this.UpdatePositionCommand = new DelegateCommand<object>(
                (point) => UpdateMousePosition((Point)point));
            this.UpdateModelCommand = new DelegateCommand(
                () => updateTilesetModel());
        }

        #endregion constructor


        #region properties

        public ICommand HandleLeftClickDownCommand { get; private set; }
        public ICommand HandleLeftClickUpCommand { get; private set; }
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
        public ICommand UpdateModelCommand { get; private set; }

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

        public Pen GridPen { get; set; }

        public AmeSession Session { get; set; }
        private TilesetModel tilesetModel;
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
                        this.itemImage = CvInvoke.Imread(this.tilesetModel.SourcePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
                    }
                }
            }
        }

        public DrawingImage TileImage { get; set; }

        #endregion properties


        #region methods

        /// <summary>
        /// Selects all tiles between the two pixel points.
        /// </summary>
        /// <param name="point1">First corner pixel point in the selection</param>
        /// <param name="point2">Second corner pixel point in the selection</param>
        public void HandleLeftClickUp(Point point1, Point point2)
        {
            point2 += offsetVector;
            if (!ImageUtils.Intersects(this.itemImage, point1) || !ImageUtils.Intersects(this.itemImage, point2))
            {
                return;
            }
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

                // TODO undo transparency
                Mat trasparentMask = new Mat();
                Mat transparentImage = new Mat(this.itemImage.Height, this.itemImage.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 1);

                Color color = this.TilesetModel.TransparentColor;
                ScalarArray transparentColorLower = new ScalarArray(new MCvScalar(color.B, color.G, color.R, 0));
                ScalarArray transparentColorHigher = new ScalarArray(new MCvScalar(color.B, color.G, color.R, 255));
                CvInvoke.InRange(this.itemImage, transparentColorLower, transparentColorHigher, trasparentMask);
                CvInvoke.BitwiseNot(trasparentMask, trasparentMask);
                this.itemImage.CopyTo(transparentImage, trasparentMask);
                this.tilesetModel.ItemImage = ImageUtils.MatToDrawingImage(transparentImage);
                this.itemImage = transparentImage;

                RaisePropertyChanged(nameof(this.IsSelectingTransparency));
                RaisePropertyChanged(nameof(this.TilesetModel));
                RaisePropertyChanged(nameof(this.TileImage));
            }
        }

        public void HandleLeftClickDown(Point point)
        {
            point += offsetVector;
            if (!ImageUtils.Intersects(this.itemImage, point))
            {
                return;
            }
            if (this.IsSelectingTransparency)
            {
                this.SetTransparentColor(point);
            }
            else
            {
                this.isSelecting = true;
                ComputeSelectLinesFromPixels(point, point);
                this.lastSelectPoint = point;
            }
        }

        public void Select(Point topLeftPixel, Size pixelSize)
        {
            this.DrawSelectLinesFromPixels(topLeftPixel, pixelSize);
            BrushModel brushModel = new BrushModel();
            Mat croppedImage = BrushUtils.CropImage(this.itemImage, topLeftPixel, pixelSize);

            if (this.TilesetModel.IsTransparent)
            {
                Mat trasparentMask = new Mat();
                Color transparentColor = this.TilesetModel.TransparentColor;
                IInputArray transparency = new ScalarArray(new MCvScalar(transparentColor.B, transparentColor.G, transparentColor.R, transparentColor.A));
                CvInvoke.InRange(croppedImage, transparency, transparency, trasparentMask);
                CvInvoke.BitwiseNot(trasparentMask, trasparentMask);
                croppedImage.CopyTo(croppedImage, trasparentMask);
            }
            brushModel.Image = ImageUtils.MatToImageDrawing(croppedImage);

            this.isSelecting = false;
            UpdateBrushMessage message = new UpdateBrushMessage(brushModel);
            this.eventAggregator.GetEvent<UpdateBrushEvent>().Publish(message);
        }

        public void updateTilesetModel()
        {
            this.itemTransform.SetPixelToTile(this.TilesetModel.Width, this.TilesetModel.Height, this.TilesetModel.OffsetX, this.TilesetModel.OffsetY, this.TilesetModel.PaddingX, this.TilesetModel.PaddingY);
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
                    rows = this.itemImage.Width / this.TilesetModel.Width,
                    columns = this.itemImage.Height / this.TilesetModel.Height,
                    cellWidth = this.TilesetModel.Width,
                    cellHeight = this.TilesetModel.Height,
                    offsetX = this.TilesetModel.OffsetX,
                    offsetY = this.TilesetModel.OffsetY,
                    paddingX = this.TilesetModel.PaddingX,
                    paddingY = this.TilesetModel.PaddingY
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
            RaisePropertyChanged(nameof(this.TileImage));
        }

        public void DrawRuler()
        {
            Console.WriteLine("DrawRuler");
        }

        public void UpdateMousePosition(Point position)
        {
            position += offsetVector;
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(position);
            }

            if (this.isSelecting && this.selectLineStopWatch.ElapsedMilliseconds > this.drawSelectLineDelay && ImageUtils.Intersects(this.itemImage, position))
            {
                this.ComputeSelectLinesFromPixels(this.lastSelectPoint, position);
            }
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
                    // TODO look into not having an itemImage and tilesetImage
                    TilesetModel newTilesetModel = new TilesetModel();
                    newTilesetModel.SourcePath = tileFilePath;
                    this.Title = "Item - " + Path.GetFileNameWithoutExtension(tileFilePath);
                    this.itemImage = CvInvoke.Imread(tileFilePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
                    this.itemTransform = new CoordinateTransform();
                    this.itemTransform.SetPixelToTile(newTilesetModel.Width, newTilesetModel.Height);
                    this.Session.CurrentTilesetList.Add(newTilesetModel);
                    this.TilesetModel = newTilesetModel;

                    using (DrawingContext context = this.tilesetImage.Open())
                    {
                        Size drawingRectSize = new Size();
                        drawingRectSize.Width = this.itemImage.Size.Width + newTilesetModel.Width;
                        drawingRectSize.Height = this.itemImage.Size.Height + newTilesetModel.Height;
                        this.offsetVector = new Vector();
                        offsetVector.X = -newTilesetModel.Width / 2;
                        offsetVector.Y = -newTilesetModel.Width / 2;
                        Rect drawingRect = new Rect((Point)offsetVector, drawingRectSize);
                        context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                        context.DrawDrawing(ImageUtils.MatToDrawingGroup(this.itemImage));
                    }
                    DrawGrid();

                    RaisePropertyChanged(nameof(this.TileImage));
                }
            }
        }

        public void AddImage()
        {
            Console.WriteLine("Add Image");
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

        private void ComputeSelectLinesFromPixels(Point pixelPoint1, Point pixelPoint2)
        {
            Point pixel1 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint1);
            Point pixel2 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint2);
            Point topLeftPixel = PointUtils.TopLeft(pixel1, pixel2);
            Point bottomRightPixel = PointUtils.BottomRight(pixel1, pixel2);
            bottomRightPixel = this.itemTransform.PixelToBottomRightTileEdge(bottomRightPixel);

            if (topLeftPixel != this.selectionBorder.TopLeft || bottomRightPixel != this.selectionBorder.BottomRight)
            {
                DrawSelectLinesFromPixels(topLeftPixel, bottomRightPixel);
            }
        }

        private void EditCollisions()
        {
            Console.WriteLine("Edit Collision");
        }

        private void ViewProperties()
        {
            OpenWindowMessage window = new OpenWindowMessage(typeof(EditTilesetInteraction));
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

        private void SetTransparentColor(Point point)
        {
            byte[] colorsBGR = this.itemImage.GetData((int)point.Y, (int)point.X);
            this.TilesetModel.TransparentColor = Color.FromRgb(colorsBGR[2], colorsBGR[1], colorsBGR[0]);
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
                        transformedPosition = itemTransform.PixelToTile(position);
                        break;
                }
            }
            transformedPosition = PointUtils.IntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);

            RaisePropertyChanged(nameof(this.PositionText));
            updatePositionLabelStopWatch.Restart();
        }

        #endregion methods
    }
}
