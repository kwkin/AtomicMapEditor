using Ame.App.Wpf.UI.Interactions.TilesetProperties;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Emgu.CV;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ame.App.Wpf.UI.Docks.ItemEditorDock
{
    public class ItemEditorViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        private CoordinateTransform itemTransform;
        private Rect selectionBorder;
        private Point lastSelectPoint;
        private bool isMouseDown;
        private double maxGridThickness;

        private long updatePositionLabelMsDelay;
        private Stopwatch updatePositionLabelStopWatch;
        private long updateSelectLineMsDelay;
        private Stopwatch updateSelectLineStopWatch;
        private long updateTransparentColorMsDelay;
        private Stopwatch updateTransparentColorStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup tilesetImage;
        private DrawingGroup gridLines;
        private DrawingGroup selectLines;
        private DrawingGroup extendedBorder;

        #endregion fields


        #region constructor

        public ItemEditorViewModel(IEventAggregator eventAggregator, IConstants constants, IAmeSession session)
            : this(eventAggregator, constants, session, session.CurrentTileset.Value, Components.Behaviors.ScrollModel.DefaultScrollModel())
        {
        }

        public ItemEditorViewModel(IEventAggregator eventAggregator, IConstants constants, IAmeSession session, TilesetModel tilesetModel)
            : this(eventAggregator, constants, session, tilesetModel, Components.Behaviors.ScrollModel.DefaultScrollModel())
        {
        }

        public ItemEditorViewModel(IEventAggregator eventAggregator, IConstants constants, IAmeSession session, IScrollModel scrollModel)
            : this(eventAggregator, constants, session, session.CurrentTileset.Value, scrollModel)
        {
        }

        public ItemEditorViewModel(IEventAggregator eventAggregator, IConstants constants, IAmeSession session, TilesetModel tilesetModel, IScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Session = session ?? throw new ArgumentNullException("session is null");
            this.ScrollModel = scrollModel ?? throw new ArgumentNullException("scrollModel is null");
            this.TilesetModel.Value = tilesetModel ?? throw new ArgumentNullException("tilesetModel is null");

            this.Title.Value = "Item";

            this.drawingGroup = new DrawingGroup();
            this.extendedBorder = new DrawingGroup();
            this.tilesetImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.selectLines = new DrawingGroup();

            this.drawingGroup.Children.Add(this.extendedBorder);
            this.drawingGroup.Children.Add(this.tilesetImage);
            this.drawingGroup.Children.Add(this.gridLines);
            this.drawingGroup.Children.Add(this.selectLines);
            this.TileImage.Value = new DrawingImage(this.drawingGroup);

            this.Scale.Value = ScaleType.Tile;
            this.PositionText.Value = "0, 0";
            this.IsGridOn.Value = true;
            this.GridPen.Value = new Pen(Brushes.Orange, 1);
            this.BackgroundBrush.Value = (SolidColorBrush)(new BrushConverter().ConvertFrom("#b8e5ed"));
            this.BackgroundPen.Value = new Pen(Brushes.Transparent, 0);
            this.maxGridThickness = constants.MaxGridThickness;
            
            this.TilesetModels = new ObservableCollection<TilesetModel>();
            foreach (TilesetModel model in this.Session.CurrentTilesets)
            {
                this.TilesetModels.Add(model);
            }
            RefreshItemModel();

            this.updatePositionLabelMsDelay = constants.DefaultUpdatePositionLabelMsDelay;
            this.updateSelectLineMsDelay = constants.DefaultUpdatePositionLabelMsDelay;
            this.updateTransparentColorMsDelay = constants.DefaultUpdatePositionLabelMsDelay;

            this.updatePositionLabelStopWatch = Stopwatch.StartNew();
            this.updateSelectLineStopWatch = Stopwatch.StartNew();
            this.updateTransparentColorStopWatch = Stopwatch.StartNew();

            this.TilesetModel.PropertyChanged += TilesetModelChanged;
            this.ItemImage.PropertyChanged += ItemImageChanged;
            this.TilesetModel.Value.IsTransparent.PropertyChanged += IsTransparentChanged;
            this.ScrollModel.PropertyChanged += ScrollModelPropertyChanged;
            this.GridPen.PropertyChanged += GridPenChanged;
            this.BackgroundBrush.PropertyChanged += BackgroundChanged;
            this.BackgroundPen.PropertyChanged += BackgroundChanged;
            this.Session.CurrentTilesets.CollectionChanged += TilesetsChanged;

            this.HandleLeftClickDownCommand = new DelegateCommand<object>((point) => HandleLeftClickDown((Point)point));
            this.HandleLeftClickUpCommand = new DelegateCommand<object>((point) => HandleLeftClickUp((Point)point));
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((Point)point));
            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.RemoveItemCommand = new DelegateCommand(() => RemoveItem());
            this.ViewPropertiesCommand = new DelegateCommand(() => ViewProperties());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.CropCommand = new DelegateCommand(() => Crop());
            this.ChangeItemCommand = new DelegateCommand<object>((entry) => ChangeItemModel(entry as TilesetModel));
            this.UpdateModelCommand = new DelegateCommand(() => UpdateTilesetModel());
            this.ShowGridCommand = new DelegateCommand(() => RedrawGrid());
            this.ShowRulerCommand = new DelegateCommand(() => RefreshRuler());
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => SetZoom(zoomLevel));

            this.eventAggregator.GetEvent<UpdatePaddedBrushEvent>().Subscribe((brushModel) =>
            {
                UpdatePaddedBrushModel(brushModel);
            }, ThreadOption.PublisherThread);
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

        public IAmeSession Session { get; set; }
        public IScrollModel ScrollModel { get; set; }

        public BindableProperty<DrawingImage> TileImage { get; set; } = BindableProperty<DrawingImage>.Prepare();
        public BindableProperty<bool> IsGridOn { get; set; } = BindableProperty<bool>.Prepare();
        public BindableProperty<bool> IsRulerOn { get; set; } = BindableProperty<bool>.Prepare();
        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty<ScaleType>.Prepare();
        public BindableProperty<string> PositionText { get; set; } = BindableProperty<string>.Prepare();
        public BindableProperty<Mat> ItemImage { get; set; } = BindableProperty<Mat>.Prepare();
        public BindableProperty<bool> IsSelectingTransparency { get; set; } = BindableProperty<bool>.Prepare();
        public BindableProperty<TilesetModel> TilesetModel { get; set; } = BindableProperty<TilesetModel>.Prepare();
        public BindableProperty<Pen> GridPen { get; set; } = BindableProperty<Pen>.Prepare();
        public BindableProperty<Brush> BackgroundBrush { get; set; } = BindableProperty<Brush>.Prepare();
        public BindableProperty<Pen> BackgroundPen { get; set; } = BindableProperty<Pen>.Prepare();
        public BindableProperty<bool> IsSourceLoaded { get; set; } = BindableProperty<bool>.Prepare();
        public ObservableCollection<TilesetModel> TilesetModels { get; set; }

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
            bool wasSelectingTransparency = this.IsSelectingTransparency.Value;
            this.IsSelectingTransparency.Value = false;
            if (this.ItemImage.Value == null)
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
            if (this.ItemImage.Value == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            if (!ImageUtils.Intersects(this.ItemImage.Value, pixelPoint))
            {
                return;
            }
            if (!this.IsSelectingTransparency.Value)
            {
                DrawSelectLinesFromPixels(pixelPoint, pixelPoint);
                this.lastSelectPoint = pixelPoint;
            }
        }

        public void HandleMouseMove(Point movePoint)
        {
            if (this.ItemImage.Value == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(movePoint);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelMsDelay)
            {
                UpdatePositionLabel(pixelPoint);
            }
            if (this.updateSelectLineStopWatch.ElapsedMilliseconds > this.updateSelectLineMsDelay
                    && this.isMouseDown
                    && !this.IsSelectingTransparency.Value)
            {
                DrawSelectLinesFromPixels(this.lastSelectPoint, pixelPoint);
            }
            else if (this.updateTransparentColorStopWatch.ElapsedMilliseconds > this.updateTransparentColorMsDelay
                    && this.isMouseDown
                    && this.IsSelectingTransparency.Value)
            {
                this.TilesetModel.Value.TransparentColor.Value = ImageUtils.ColorAt(this.ItemImage.Value, pixelPoint);
            }
        }

        public void SelectTiles(Point pixelPoint1, Point pixelPoint2)
        {
            pixelPoint1 = this.TilesetModel.Value.GetPoint(pixelPoint1);
            pixelPoint2 = this.TilesetModel.Value.GetPoint(pixelPoint2);
            GeneralTransform pixelToTile = GeometryUtils.CreateTransform(this.itemTransform.pixelToTile);
            Point tile1 = GeometryUtils.TransformInt(pixelToTile, pixelPoint1);
            Point tile2 = GeometryUtils.TransformInt(pixelToTile, pixelPoint2);
            Point topLeftTile = GeometryUtils.TopLeftPoint(tile1, tile2);
            Point topLeftPixel = GeometryUtils.TransformInt(pixelToTile.Inverse, topLeftTile);
            Size tileSize = GeometryUtils.ComputeSize(tile1, tile2);
            Size pixelSize = GeometryUtils.TransformInt(pixelToTile.Inverse, tileSize);

            DrawSelectLinesFromPixels(topLeftPixel, pixelSize);

            PaddedBrushModel brushModel = new PaddedBrushModel(this.TilesetModel.Value, (int)topLeftTile.X, (int)topLeftTile.Y);
            brushModel.SetSize(tileSize);
            Mat croppedImage = BrushUtils.CropImage(this.ItemImage.Value, topLeftPixel, pixelSize);
            if (this.TilesetModel.Value.IsTransparent.Value)
            {
                croppedImage = ImageUtils.ColorToTransparent(croppedImage, this.TilesetModel.Value.TransparentColor.Value);
            }
            brushModel.TileImage(croppedImage, topLeftPixel, this.TilesetModel.Value);

            this.eventAggregator.GetEvent<NewPaddedBrushEvent>().Publish(brushModel);
        }

        public void SelectTransparency(Point pixelPoint)
        {
            if (!ImageUtils.Intersects(this.ItemImage.Value, pixelPoint))
            {
                return;
            }
            this.TilesetModel.Value.TransparentColor.Value = ImageUtils.ColorAt(this.ItemImage.Value, pixelPoint);
            if (this.TilesetModel.Value.IsTransparent.Value)
            {
                Mat transparentImage = ImageUtils.ColorToTransparent(this.ItemImage.Value, this.TilesetModel.Value.TransparentColor.Value);
                using (DrawingContext context = this.tilesetImage.Open())
                {
                    context.DrawDrawing(ImageUtils.MatToImageDrawing(transparentImage));
                }
            }
        }

        public void RefreshItemModel()
        {
            ChangeItemModel(this.TilesetModel.Value);
        }

        public void ChangeItemModel(TilesetModel tilesetModel)
        {
            if (tilesetModel == null)
            {
                return;
            }
            if (!this.TilesetModels.Contains(tilesetModel))
            {
                return;
            }
            this.IsSourceLoaded.Value = true;
            this.TilesetModel.Value = tilesetModel;
            this.TilesetModel.Value.IsTransparent = tilesetModel.IsTransparent;
            this.TilesetModel.Value.TransparentColor = tilesetModel.TransparentColor;
            this.Session.CurrentTileset.Value = this.TilesetModel.Value;
            this.ItemImage.Value = CvInvoke.Imread(this.TilesetModel.Value.SourcePath.Value, Emgu.CV.CvEnum.ImreadModes.Unchanged);

            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.Value.TileWidth.Value, this.TilesetModel.Value.TileHeight.Value);
            this.itemTransform.SetSelectionToPixel(this.TilesetModel.Value.TileWidth.Value / 2, this.TilesetModel.Value.TileHeight.Value / 2);

            Mat drawingMat = this.ItemImage.Value;
            if (this.TilesetModel.Value.IsTransparent.Value)
            {
                drawingMat = ImageUtils.ColorToTransparent(this.ItemImage.Value, this.TilesetModel.Value.TransparentColor.Value);
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (DrawingContext context = this.tilesetImage.Open())
                {
                    context.DrawDrawing(ImageUtils.MatToImageDrawing(drawingMat));
                }
            }), DispatcherPriority.Render);

            RedrawBackground();
            RedrawGrid();
        }

        public void UpdatePaddedBrushModel(PaddedBrushModel model)
        {
            int pixelOffsetX = model.TileOffsetX.Value * model.TileWidth.Value;
            int pixelOffsetY = model.TileOffsetY.Value * model.TileHeight.Value;
            this.TilesetModel.Value.SetTileSize(model.GetTileSize());

            Point pixelOffset = new Point(pixelOffsetX, pixelOffsetY);
            Point pixelEnd = new Point(pixelOffsetX + model.PixelWidth.Value - 1, pixelOffsetY + model.PixelHeight.Value - 1);
            SelectTiles(pixelOffset, pixelEnd);
        }

        public void RedrawGrid()
        {
            DrawGrid(this.IsGridOn.Value);
        }

        public void DrawGrid(bool drawGrid)
        {
            this.IsGridOn.Value = drawGrid;
            if (this.IsGridOn.Value)
            {
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.TilesetModel.Value);
                double thickness = 1 / this.ScrollModel.ZoomLevels[this.ScrollModel.ZoomIndex].zoom;
                gridParameters.DrawingPen.Thickness = thickness < this.maxGridThickness ? thickness : this.maxGridThickness;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    DrawingGroup group = gridParameters.CreateGrid();
                    this.gridLines.Children = group.Children;
                }), DispatcherPriority.Render);
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

        public void RedrawBackground()
        {
            TilesetModel model = this.TilesetModel.Value;
            Size extendedSize = new Size();
            extendedSize.Width = model.PixelWidth.Value + model.TileWidth.Value;
            extendedSize.Height = model.PixelHeight.Value + model.TileHeight.Value;
            Point extendedPoint = new Point();
            extendedPoint.X = -model.TileWidth.Value / 2;
            extendedPoint.Y = -model.TileHeight.Value / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            Size backgroundSize = new Size();
            backgroundSize.Width = model.PixelWidth.Value;
            backgroundSize.Height = model.PixelHeight.Value;
            Point backgroundPoint = new Point();
            backgroundPoint.X = 0;
            backgroundPoint.Y = 0;
            Rect backgroundRectangle = new Rect(backgroundPoint, backgroundSize);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (DrawingContext context = this.extendedBorder.Open())
                {
                    context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                    context.DrawRectangle(this.BackgroundBrush.Value, this.BackgroundPen.Value, backgroundRectangle);
                }
            }), DispatcherPriority.Render);
        }

        public void ZoomIn()
        {
            this.ScrollModel.ZoomIn();
        }

        public void ZoomOut()
        {
            this.ScrollModel.ZoomOut();
        }

        public void SetZoom(int zoomIndex)
        {
            this.ScrollModel.SetZoom(zoomIndex);
        }

        public void SetZoom(ZoomLevel zoomLevel)
        {
            this.ScrollModel.SetZoom(zoomLevel);
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

        private void TilesetModelChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TilesetModel.Value.IsTransparent.PropertyChanged += IsTransparentChanged;
            if (!string.IsNullOrEmpty(this.TilesetModel.Value.SourcePath.Value))
            {
                this.Title.Value = "Item - " + Path.GetFileNameWithoutExtension(this.TilesetModel.Value.Name.Value);
                ChangeItemModel(this.TilesetModel.Value);
            }
            else
            {
                this.Title.Value = "Item Editor";
            }
        }

        private void ItemImageChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TilesetModel.Value.TilesetImage = ImageUtils.MatToDrawingGroup(this.ItemImage.Value);
        }

        private void BackgroundChanged(object sender, PropertyChangedEventArgs e)
        {
            RedrawBackground();
        }

        private void GridPenChanged(object sender, PropertyChangedEventArgs e)
        {
            RedrawGrid();
        }

        private void IsTransparentChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshItemModel();
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

        private void TilesetsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(TilesetModel model in e.NewItems)
                    {
                        this.TilesetModels.Add(model);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach(TilesetModel model in e.OldItems)
                    {
                        this.TilesetModels.Remove(model);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.TilesetModels.Clear();
                    break;
                default:
                    break;
            }
        }

        private void DrawSelectLinesFromPixels(Point pixelPoint1, Point pixelPoint2)
        {
            Point pixel1 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint1);
            Point pixel2 = this.itemTransform.PixelToTopLeftTileEdge(pixelPoint2);
            Point topLeftPixel = GeometryUtils.TopLeftPoint(pixel1, pixel2);
            Point bottomRightPixel = GeometryUtils.BottomRightPoint(pixel1, pixel2);
            bottomRightPixel = this.itemTransform.PixelToBottomRightTileEdge(bottomRightPixel);

            topLeftPixel = this.TilesetModel.Value.BindPointIncludeSize(topLeftPixel);
            bottomRightPixel = this.TilesetModel.Value.BindPointIncludeSize(bottomRightPixel);

            if (topLeftPixel != this.selectionBorder.TopLeft || bottomRightPixel != this.selectionBorder.BottomRight)
            {
                this.selectionBorder = new Rect(topLeftPixel, bottomRightPixel);
                using (DrawingContext context = this.selectLines.Open())
                {
                    context.DrawRectangle(Brushes.Transparent, this.GridPen.Value, this.selectionBorder);
                }
                this.updateSelectLineStopWatch.Restart();
            }
        }

        private void UpdateTilesetModel()
        {
            TilesetModel model = this.TilesetModel.Value;
            this.itemTransform.SetPixelToTile(model.TileWidth.Value, model.TileHeight.Value, model.OffsetX.Value, model.OffsetY.Value, model.PaddingX.Value, model.PaddingY.Value);
            RedrawGrid();
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.TileImage != null)
            {
                switch (this.Scale.Value)
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
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.PositionText.Value = (transformedPosition.X + ", " + transformedPosition.Y);
            }), DispatcherPriority.Render);
            updatePositionLabelStopWatch.Restart();
        }

        private void DrawSelectLinesFromPixels(Point topLeftPixel, Size pixelSize)
        {
            this.selectionBorder = new Rect(topLeftPixel, pixelSize);
            using (DrawingContext context = this.selectLines.Open())
            {
                context.DrawRectangle(Brushes.Transparent, this.GridPen.Value, this.selectionBorder);
            }
            this.updateSelectLineStopWatch.Restart();
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
            EditTilesetInteraction interaction = new EditTilesetInteraction(this.TilesetModel.Value, OnNewTilesetWindowClosed);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        private void Crop()
        {
            Console.WriteLine("Crop ");
        }

        private void RemoveItem()
        {
            this.Session.CurrentTilesets.Remove(this.TilesetModel.Value);
        }

        private void OnNewTilesetWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (Mouse.OverrideCursor == Cursors.Pen)
            {
                Mouse.OverrideCursor = null;
            }
            if (confirmation.Confirmed)
            {
                TilesetModel messageTilesetModel = confirmation.Content as TilesetModel;
                this.Session.CurrentTilesets.Add(messageTilesetModel);
                this.TilesetModel.Value = messageTilesetModel;
                ChangeItemModel(this.TilesetModel.Value);
            }
        }

        #endregion methods
    }
}
