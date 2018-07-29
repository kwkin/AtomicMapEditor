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
using Ame.Infrastructure.Messages.Interactions;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Ame.Modules.Windows.Interactions.TilesetProperties;
using Emgu.CV;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Docks.ItemEditorDock
{
    // TODO add image loading
    public class ItemEditorViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private CoordinateTransform itemTransform;
        private Rect selectionBorder;
        private Point lastSelectPoint;
        private bool isMouseDown;
        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private long drawSelectLineDelay = Global.defaultDrawSelectLineDelay;
        private Stopwatch updatePositionLabelStopWatch;
        private Stopwatch selectLineStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup tilesetImage;
        private DrawingGroup gridLines;
        private DrawingGroup selectLines;
        private DrawingGroup extendedBorder;

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

            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;
            this.Session = session;
            this.TilesetModel = tilesetModel;

            // TODO create default scroll modes, add a factory method
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
            this.backgroundBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#b8e5ed"));
            this.backgroundPen = new Pen(Brushes.Transparent, 0);
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();
            this.selectLineStopWatch = Stopwatch.StartNew();

            this.HandleLeftClickDownCommand = new DelegateCommand<object>((point) =>
            {
                HandleLeftClickDown((Point)point);
            });
            this.HandleLeftClickUpCommand = new DelegateCommand<object>(
                (point) => HandleLeftClickUp((Point)point));
            this.HandleMouseMoveCommand = new DelegateCommand<object>(
                (point) => HandleMouseMove((Point)point));
            this.AddTilesetCommand = new DelegateCommand(
                () => AddTileset());
            this.AddImageCommand = new DelegateCommand(
                () => AddImage());
            this.RemoveItemCommand = new DelegateCommand(
                () => RemoveItem());
            this.ViewPropertiesCommand = new DelegateCommand(
                () => ViewProperties());
            this.EditCollisionsCommand = new DelegateCommand(
                () => EditCollisions());
            this.CropCommand = new DelegateCommand(
                () => Crop());
            this.ChangeItemCommand = new DelegateCommand(
                () => UpdateItemModel());
            this.UpdateModelCommand = new DelegateCommand(
                () => UpdateTilesetModel());
            this.ShowGridCommand = new DelegateCommand(
                () => RefreshGrid());
            this.ShowRulerCommand = new DelegateCommand(
                () => RefreshRuler());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(
                (zoomLevel) => this.SetZoom(zoomLevel));
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
        public ICommand UpdateModelCommand { get; private set; }
        public ICommand ChangeItemCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand ShowRulerCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public AmeSession Session { get; set; }

        private DrawingImage tileImage;
        public DrawingImage TileImage
        {
            get
            {
                return this.tileImage;
            }
            set
            {
                SetProperty(ref this.tileImage, value);
            }
        }

        private string positionText;
        public string PositionText
        {
            get
            {
                return this.positionText;
            }
            set
            {
                SetProperty(ref this.positionText, value);
            }
        }

        private ScaleType scale;
        public ScaleType Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                SetProperty(ref this.scale, value);
            }
        }

        private bool isGridOn;
        public bool IsGridOn
        {
            get
            {
                return this.isGridOn;
            }
            set
            {
                SetProperty(ref this.isGridOn, value);
            }
        }

        private bool isRulerOn;
        public bool IsRulerOn
        {
            get
            {
                return this.isRulerOn;
            }
            set
            {
                SetProperty(ref this.isRulerOn, value);
            }
        }

        private ObservableCollection<ZoomLevel> zoomLevels;
        public ObservableCollection<ZoomLevel> ZoomLevels
        {
            get
            {
                return this.zoomLevels;
            }
            set
            {
                SetProperty(ref this.zoomLevels, value);
            }
        }

        private int zoomIndex;
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

        private Mat itemImage;
        public Mat ItemImage
        {
            get
            {
                return this.itemImage;
            }
            set
            {
                if (SetProperty(ref this.itemImage, value))
                {
                    this.TilesetModel.TilesetImage = ImageUtils.MatToDrawingGroup(this.ItemImage);
                }
            }
        }

        private bool isSelectingTransparency;
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
                        this.Title = "Item - " + Path.GetFileNameWithoutExtension(this.tilesetModel.Name);
                        ChangeItemModel(this.tilesetModel);
                        RaisePropertyChanged(nameof(this.TileImage));
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

        private Pen gridPen;
        public Pen GridPen
        {
            get
            {
                return gridPen;
            }
            set
            {
                if (SetProperty(ref this.gridPen, value))
                {
                    RefreshGrid();
                }
            }
        }

        private Brush backgroundBrush;
        public Brush BackgroundBrush
        {
            get
            {
                return backgroundBrush;
            }
            set
            {
                if (SetProperty(ref this.backgroundBrush, value))
                {
                    RefreshBackground();
                }
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
                if (SetProperty(ref this.backgroundPen, value))
                {
                    RefreshBackground();
                }
            }
        }

        #endregion properties


        #region methods

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void HandleLeftClickUp(Point selectPoint)
        {
            this.isMouseDown = false;
            bool wasSelectingTransparency = this.IsSelectingTransparency;
            this.IsSelectingTransparency = false;
            if (this.ItemImage == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            if (wasSelectingTransparency)
            {
                SelectTransparency(pixelPoint);
            }
            else
            {
                SelectTiles(pixelPoint, this.lastSelectPoint);
            }
        }

        public void HandleLeftClickDown(Point selectPoint)
        {
            this.isMouseDown = true;
            if (this.ItemImage == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            selectPoint = selectToPixel.Transform(selectPoint);
            if (!ImageUtils.Intersects(this.ItemImage, selectPoint))
            {
                return;
            }
            if (!this.IsSelectingTransparency)
            {
                ComputeSelectLinesFromPixels(selectPoint, selectPoint);
                this.lastSelectPoint = selectPoint;
            }
        }

        public void HandleMouseMove(Point selectPoint)
        {
            if (this.ItemImage == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(pixelPoint);
                if (this.IsSelectingTransparency)
                {
                    this.TransparentColor = ImageUtils.ColorAt(this.ItemImage, pixelPoint);
                }
            }
            if (this.isMouseDown
                && !this.IsSelectingTransparency
                && this.selectLineStopWatch.ElapsedMilliseconds > this.drawSelectLineDelay)
            {
                this.ComputeSelectLinesFromPixels(this.lastSelectPoint, pixelPoint);
            }
        }

        public void SelectTiles(Point pixelPoint1, Point pixelPoint2)
        {
            pixelPoint1 = this.TilesetModel.BindPoint(pixelPoint1);
            pixelPoint2 = this.TilesetModel.BindPoint(pixelPoint2);
            GeneralTransform pixelToTile = GeometryUtils.CreateTransform(this.itemTransform.pixelToTile);
            Point tile1 = GeometryUtils.TransformInt(pixelToTile, pixelPoint1);
            Point tile2 = GeometryUtils.TransformInt(pixelToTile, pixelPoint2);
            Point topLeftTile = GeometryUtils.TopLeftPoint(tile1, tile2);
            Point topLeftPixel = GeometryUtils.TransformInt(pixelToTile.Inverse, topLeftTile);
            Size tileSize = GeometryUtils.ComputeSize(tile1, tile2);
            Size pixelSize = GeometryUtils.TransformInt(pixelToTile.Inverse, tileSize);

            this.DrawSelectLinesFromPixels(topLeftPixel, pixelSize);
            BrushModel brushModel = new BrushModel((int)tileSize.Width, (int)tileSize.Height, this.TilesetModel.TileWidth, this.TilesetModel.TileHeight);
            Mat croppedImage = BrushUtils.CropImage(this.ItemImage, topLeftPixel, pixelSize);

            if (this.IsTransparent)
            {
                croppedImage = ImageUtils.ColorToTransparent(croppedImage, this.TransparentColor);
            }
            brushModel.TileImage(croppedImage);

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Publish(brushModel);
        }

        public void SelectTransparency(Point pixelPoint)
        {
            if (!ImageUtils.Intersects(this.ItemImage, pixelPoint))
            {
                return;
            }
            this.TransparentColor = ImageUtils.ColorAt(this.ItemImage, pixelPoint);
            if (this.IsTransparent)
            {
                Mat transparentImage = ImageUtils.ColorToTransparent(this.ItemImage, this.TransparentColor);
                using (DrawingContext context = this.tilesetImage.Open())
                {
                    context.DrawDrawing(ImageUtils.MatToImageDrawing(transparentImage));
                }
            }
            RaisePropertyChanged(nameof(this.TransparentColor));
        }

        public void RefreshGrid()
        {
            if (this.IsGridOn)
            {
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.TilesetModel);
                double thickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
                gridParameters.DrawingPen.Thickness = thickness < Global.maxGridThickness ? thickness : Global.maxGridThickness;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    DrawingGroup group = gridParameters.CreateGrid();
                    this.gridLines.Children = group.Children;
                }),
                DispatcherPriority.Render);
            }
            else
            {
                this.gridLines.Children.Clear();
            }
        }

        public void RefreshRuler()
        {
            Console.WriteLine("Draw Ruler");
        }

        public void RefreshBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.TilesetModel.PixelWidth + this.TilesetModel.TileWidth;
            extendedSize.Height = this.TilesetModel.PixelHeight + this.TilesetModel.TileHeight;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.TilesetModel.TileWidth / 2;
            extendedPoint.Y = -this.TilesetModel.TileHeight / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            Size backgroundSize = new Size();
            backgroundSize.Width = this.TilesetModel.PixelWidth;
            backgroundSize.Height = this.TilesetModel.PixelHeight;
            Point backgroundPoint = new Point();
            backgroundPoint.X = 0;
            backgroundPoint.Y = 0;
            Rect backgroundRectangle = new Rect(backgroundPoint, backgroundSize);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (DrawingContext context = this.extendedBorder.Open())
                {
                    context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                    context.DrawRectangle(backgroundBrush, backgroundPen, backgroundRectangle);
                }
            }),
            DispatcherPriority.Render);
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
            NewTilesetInteraction interaction = new NewTilesetInteraction(OnNewTilesetWindowClosed);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
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
            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.TileWidth, this.TilesetModel.TileHeight);
            this.itemTransform.SetSlectionToPixel(this.TilesetModel.TileWidth / 2, this.TilesetModel.TileHeight / 2);

            this.TransparentColor = this.TilesetModel.TransparentColor;
            this.IsTransparent = this.TilesetModel.IsTransparent;
            Mat drawingMat = this.ItemImage;
            if (this.IsTransparent)
            {
                drawingMat = ImageUtils.ColorToTransparent(this.ItemImage, this.TransparentColor);
            }
            using (DrawingContext context = this.tilesetImage.Open())
            {
                context.DrawDrawing(ImageUtils.MatToImageDrawing(drawingMat));
            }
            RefreshBackground();
            RefreshGrid();
        }

        private void ComputeSelectLinesFromPixels(Point pixelPoint1, Point pixelPoint2)
        {
            Point pixel1 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint1);
            Point pixel2 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint2);
            Point topLeftPixel = GeometryUtils.TopLeftPoint(pixel1, pixel2);
            Point bottomRightPixel = GeometryUtils.BottomRightPoint(pixel1, pixel2);
            bottomRightPixel = this.itemTransform.PixelToBottomRightTileEdge(bottomRightPixel);

            topLeftPixel = this.TilesetModel.BindPointIncludeSize(topLeftPixel);
            bottomRightPixel = this.TilesetModel.BindPointIncludeSize(bottomRightPixel);
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
            RefreshGrid();
        }

        private void DrawSelectLinesFromPixels(Point topLeftPixel, Size pixelSize)
        {
            this.selectionBorder = new Rect(topLeftPixel, pixelSize);
            using (DrawingContext context = this.selectLines.Open())
            {
                context.DrawRectangle(Brushes.Transparent, this.GridPen, this.selectionBorder);
            }
            this.selectLineStopWatch.Restart();
        }

        private void DrawSelectLinesFromPixels(Point topLeftPixel, Point bottomRightPixel)
        {
            this.selectionBorder = new Rect(topLeftPixel, bottomRightPixel);
            using (DrawingContext context = this.selectLines.Open())
            {
                context.DrawRectangle(Brushes.Transparent, this.GridPen, this.selectionBorder);
            }
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
            EditTilesetInteraction interaction = new EditTilesetInteraction(this.TilesetModel);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        private void Crop()
        {
            Console.WriteLine("Crop ");
        }

        private void RemoveItem()
        {
            Console.WriteLine("Remove Item");
        }

        private void OnNewTilesetWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                TilesetInteractionMessage message = confirmation.Content as TilesetInteractionMessage;
                TilesetModel messageTilesetModel = message.Tileset as TilesetModel;
                this.Session.CurrentTilesetList.Add(messageTilesetModel);
                this.TilesetModel = messageTilesetModel;
            }
        }

        #endregion methods
    }
}
