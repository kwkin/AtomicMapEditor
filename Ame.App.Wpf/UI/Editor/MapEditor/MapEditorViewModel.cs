using Ame.App.Wpf.UILogic.Actions;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ame.App.Wpf.UI.Editor.MapEditor
{
    public class MapEditorViewModel : EditorViewModelTemplate
    {
        #region fields

        private const double hoverSampleOpacity = 0.5;

        private IEventAggregator eventAggregator;
        private IAmeSession session;

        private ILayer currentLayer;
        private PaddedBrushModel brush;
        private LayerOrderRenderer orderer;
        private CoordinateTransform imageTransform;

        private double maxGridThickness;

        private long updatePositionLabelMsDelay;
        private Stopwatch updatePositionLabelStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup mapBackground;
        private DrawingGroup hoverSample;
        private DrawingGroup layerItems;
        private DrawingGroup gridLines;
        private DrawingGroup layerBoundaries;

        private Point lastTilePoint;

        #endregion fields


        #region constructor

        public MapEditorViewModel(IEventAggregator eventAggregator, IConstants constants, IAmeSession session, Map map)
            : this(eventAggregator, constants, session, map, Components.Behaviors.ScrollModel.DefaultScrollModel())
        {
        }

        public MapEditorViewModel(IEventAggregator eventAggregator, IConstants constants, IAmeSession session, Map map, IScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.Map.Value = map ?? throw new ArgumentNullException("map is null");
            this.ScrollModel = scrollModel ?? throw new ArgumentNullException("scrollModel is null");

            this.Title.Value = map.Name.Value;
            this.orderer = new LayerOrderRenderer(this.session);

            this.imageTransform = new CoordinateTransform();
            this.imageTransform.SetPixelToTile(map.TileWidth.Value, map.TileHeight.Value);
            this.imageTransform.SetSelectionToPixel(map.TileWidth.Value / 2, map.TileHeight.Value / 2);

            this.drawingGroup = new DrawingGroup();
            this.mapBackground = new DrawingGroup();
            this.layerItems = new DrawingGroup();
            this.hoverSample = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.layerBoundaries = new DrawingGroup();

            RenderOptions.SetEdgeMode(this.hoverSample, EdgeMode.Aliased);

            this.drawingGroup.Children.Add(this.mapBackground);
            this.drawingGroup.Children.Add(this.layerItems);
            this.drawingGroup.Children.Add(this.hoverSample);
            this.drawingGroup.Children.Add(this.gridLines);
            this.drawingGroup.Children.Add(this.layerBoundaries);
            this.DrawingCanvas = new DrawingImage(this.drawingGroup);

            this.BackgroundBrush.Value = new SolidColorBrush(map.BackgroundColor.Value);
            this.BackgroundPen.Value = new Pen(Brushes.Transparent, 0);
            this.Scale.Value = ScaleType.Tile;
            this.PositionText.Value = "0, 0";
            this.HoverSampleOpacity.Value = hoverSampleOpacity;
            this.hoverSample.Opacity = hoverSampleOpacity;
            this.maxGridThickness = constants.MaxGridThickness;

            this.updatePositionLabelMsDelay = constants.DefaultUpdatePositionLabelMsDelay;
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            AddMapLayers(map);
            ChangeCurrentLayer(this.session.CurrentMap.Value.CurrentLayer.Value);
            RedrawBackground();
            UpdateMapRecentlySaved();

            this.session.CurrentMap.Value.CurrentLayer.PropertyChanged += CurrentLayerChanged;
            this.Map.Value.Name.PropertyChanged += MapNameChanged;
            this.Map.Value.Layers.CollectionChanged += LayersChanged;
            this.Map.Value.IsRecentlySaved.PropertyChanged += MapRecentlySavedChanged;
            this.ScrollModel.PropertyChanged += ScrollModelPropertyChanged;
            this.BackgroundBrush.PropertyChanged += BackgroundChanged;
            this.BackgroundPen.PropertyChanged += BackgroundChanged;
            this.HoverSampleOpacity.PropertyChanged += HoverSampleOpacityChanged;

            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn.Value));
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((Point)point));
            this.UndoCommand = new DelegateCommand(() => this.Undo());
            this.RedoCommand = new DelegateCommand(() => this.Redo());
            this.ZoomInCommand = new DelegateCommand(() => this.ScrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => this.ScrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => this.ScrollModel.SetZoom(zoomLevel));
            this.HandleLeftClickDownCommand = new DelegateCommand<object>((point) => HandleLeftClickDown((Point)point));
            this.HandleLeftClickUpCommand = new DelegateCommand<object>((point) => HandleLeftClickUp((Point)point));

            this.eventAggregator.GetEvent<NewPaddedBrushEvent>().Subscribe((brushEvent) =>
            {
                UpdateBrushImage(brushEvent);
            }, ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        public ICommand HandleLeftClickDownCommand { get; private set; }
        public ICommand HandleLeftClickUpCommand { get; private set; }
        public ICommand HandleMouseMoveCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }

        public BindableProperty<Map> Map { get; set; } = BindableProperty<Map>.Prepare();

        public BindableProperty<bool> IsGridOn { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty<ScaleType>.Prepare();

        public BindableProperty<string> PositionText { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<Brush> BackgroundBrush { get; set; } = BindableProperty<Brush>.Prepare();

        public BindableProperty<Pen> BackgroundPen { get; set; } = BindableProperty<Pen>.Prepare();

        public BindableProperty<double> HoverSampleOpacity { get; set; } = BindableProperty<double>.Prepare();

        public DrawingImage DrawingCanvas { get; set; }

        public IScrollModel ScrollModel { get; set; }

        private IDrawingTool DrawingTool
        {
            get
            {
                return this.session.DrawingTool.Value;
            }
        }

        #endregion properties


        #region methods

        public override void ZoomIn()
        {
            this.ScrollModel.ZoomIn();
        }

        public override void ZoomOut()
        {
            this.ScrollModel.ZoomOut();
        }

        public override void SetZoom(int zoomIndex)
        {
            this.ScrollModel.SetZoom(zoomIndex);
        }

        public override void SetZoom(ZoomLevel zoomLevel)
        {
            this.ScrollModel.SetZoom(zoomLevel);
        }

        public override object GetContent()
        {
            return this.Map.Value;
        }

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void HandleLeftClickDown(Point selectPoint)
        {
            if (this.brush == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.imageTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            DrawPressed(pixelPoint);
        }

        public void HandleLeftClickUp(Point selectPoint)
        {
            if (this.brush == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.imageTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            DrawReleased(pixelPoint);
        }

        public void HandleMouseMove(Point position)
        {
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.imageTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(position);
            DrawHover(pixelPoint);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelMsDelay)
            {
                UpdatePositionLabel(pixelPoint);
            }
        }

        public void UpdateBrushImage(PaddedBrushModel brushModel)
        {
            this.brush = brushModel;
            this.DrawingTool.Brush = this.brush;
            this.DrawingTool.Transform = this.imageTransform;
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
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.Map.Value);
                double thickness = 1 / this.ScrollModel.ZoomLevels[this.ScrollModel.ZoomIndex].zoom;
                gridParameters.DrawingPen.Thickness = thickness < this.maxGridThickness ? thickness : this.maxGridThickness;
                DrawingGroup group = gridParameters.CreateGrid();
                this.gridLines.Children = group.Children;
            }
            else
            {
                this.gridLines.Children.Clear();
            }
        }

        public void Undo()
        {
            this.Map.Value.Undo();
        }

        public void Redo()
        {
            this.Map.Value.Redo();
        }

        public void DrawLayerBoundaries(ILayer layer)
        {
            if (layer == null)
            {
                return;
            }

            LayerBoundariesRenderable renderer = new LayerBoundariesRenderable(this.session.CurrentMap.Value.CurrentLayer.Value);

            double thickness = 4 / this.ScrollModel.ZoomLevels[this.ScrollModel.ZoomIndex].zoom;
            thickness = thickness < this.maxGridThickness ? thickness : this.maxGridThickness;
            renderer.DrawingPen.Thickness = thickness;
            renderer.DrawingPenDashed.Thickness = thickness;
            DrawingGroup group = renderer.CreateBoundaries();
            this.layerBoundaries.Children = group.Children;
        }

        public void ChangeCurrentLayer(ILayer layer)
        {
            if (layer == null)
            {
                return;
            }
            if (layer == this.currentLayer)
            {
                return;
            }
            if (this.currentLayer != null)
            {
                this.currentLayer.Columns.PropertyChanged -= LayerBoundariesChanged;
                this.currentLayer.Rows.PropertyChanged -= LayerBoundariesChanged;
                this.currentLayer.OffsetY.PropertyChanged -= LayerBoundariesChanged;
                this.currentLayer.OffsetX.PropertyChanged -= LayerBoundariesChanged;
            }
            layer.Columns.PropertyChanged += LayerBoundariesChanged;
            layer.Rows.PropertyChanged += LayerBoundariesChanged;
            layer.OffsetY.PropertyChanged += LayerBoundariesChanged;
            layer.OffsetX.PropertyChanged += LayerBoundariesChanged;
            this.currentLayer = layer;
            DrawLayerBoundaries(layer);
        }

        private void HoverSampleOpacityChanged(object sender, PropertyChangedEventArgs e)
        {
            this.hoverSample.Opacity = this.HoverSampleOpacity.Value;
        }

        private void CurrentLayerChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangeCurrentLayer(this.session.CurrentMap.Value.CurrentLayer.Value);
        }

        private void BackgroundChanged(object sender, PropertyChangedEventArgs e)
        {
            RedrawBackground();
        }

        private void UpdateGridPen(object sender, PropertyChangedEventArgs e)
        {
            RedrawGrid();
        }

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILayer layer in e.NewItems)
                    {
                        this.layerItems.Children.Add(layer.Group);
                        layer.IsVisible.PropertyChanged += (s, args) =>
                        {
                            LayerChanged(layer);
                        };
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ILayer layer in e.OldItems)
                    {
                        this.layerItems.Children.Remove(layer.Group);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;

                    // TODO integrate with visibility
                    ILayer oldLayer = this.session.CurrentLayers.Value[oldIndex];
                    ILayer newLayer = this.session.CurrentLayers.Value[newIndex];

                    int oldGroupIndex = this.layerItems.Children.IndexOf(oldLayer.Group);
                    int newGroupIndex = this.layerItems.Children.IndexOf(newLayer.Group);
                    if (oldGroupIndex != -1 && newGroupIndex != -1)
                    {
                        Drawing oldLayerImage = this.layerItems.Children[oldGroupIndex];
                        this.layerItems.Children[oldGroupIndex] = this.layerItems.Children[newGroupIndex];
                        this.layerItems.Children[newGroupIndex] = oldLayerImage;
                    }
                    break;

                default:
                    break;
            }
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

        private void DrawPressed(Point point)
        {
            if (!ImageUtils.Intersects(this.DrawingCanvas, point))
            {
                return;
            }
            Point topLeftTilePixelPoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            this.DrawingTool.DrawPressed(this.Map.Value, topLeftTilePixelPoint);
        }

        private void DrawReleased(Point point)
        {
            Point topLeftTilePixelPoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            this.DrawingTool.DrawReleased(this.Map.Value, topLeftTilePixelPoint);
        }

        private void DrawHover(Point point)
        {
            if (this.brush == null)
            {
                return;
            }
            if (!this.DrawingTool.HasHoverSample())
            {
                return;
            }
            Point topLeftTilePixelPoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            if (topLeftTilePixelPoint == this.lastTilePoint)
            {
                return;
            }
            double zoomLevel = this.ScrollModel.ZoomLevels[this.ScrollModel.ZoomIndex].zoom;
            this.lastTilePoint = topLeftTilePixelPoint;

            this.DrawingTool.DrawHoverSample(this.Map.Value, this.hoverSample, zoomLevel, topLeftTilePixelPoint);
        }

        private void LayerChanged(ILayer layer)
        {
            if (layer.IsVisible.Value)
            {
                int insertIndex = this.orderer.getAffectedIndex(layer);
                this.layerItems.Children.Insert(insertIndex, layer.Group);
            }
            else
            {
                int removeIndex = this.orderer.getAffectedIndex(layer);
                this.layerItems.Children.RemoveAt(removeIndex);
            }
        }

        private void AddMapLayers(Map map)
        {
            this.layerItems.Children.Clear();
            foreach (ILayer layer in map.Layers)
            {
                this.layerItems.Children.Add(layer.Group);
                layer.IsVisible.PropertyChanged += (s, args) =>
                {
                    LayerChanged(layer);
                };
            }
        }

        private void LayerBoundariesChanged(object sender, PropertyChangedEventArgs e)
        {
            DrawLayerBoundaries(this.session.CurrentMap.Value.CurrentLayer.Value);
        }

        private void MapNameChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateMapRecentlySaved();
        }

        private void MapRecentlySavedChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateMapRecentlySaved();
        }

        private void UpdateMapRecentlySaved()
        {
            bool isMapRecentlySaved = this.Map.Value.IsRecentlySaved.Value;
            this.Title.Value = isMapRecentlySaved ? this.Map.Value.Name.Value : this.Map.Value.Name.Value + "*";
        }

        private void RedrawBackground()
        {
            Map map = this.Map.Value;
            Size extendedSize = new Size
            {
                Width = map.PixelWidth.Value + map.TileWidth.Value,
                Height = map.PixelHeight.Value + map.TileHeight.Value
            };
            Point extendedPoint = new Point
            {
                X = -map.TileWidth.Value / 2,
                Y = -map.TileHeight.Value / 2
            };
            Rect extendedRect = new Rect(extendedPoint, extendedSize);

            Point backgroundLocation = new Point(0, 0);
            Size backgroundSize = new Size(map.PixelWidth.Value, map.PixelHeight.Value);
            Rect backgroundRect = new Rect(backgroundLocation, backgroundSize);

            using (DrawingContext context = this.mapBackground.Open())
            {
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), extendedRect);
                context.DrawRectangle(this.BackgroundBrush.Value, this.BackgroundPen.Value, backgroundRect);
            }
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            switch (this.Scale.Value)
            {
                case ScaleType.Pixel:
                    transformedPosition = position;
                    break;

                case ScaleType.Tile:
                    if (this.Map.Value != null)
                    {
                        GeneralTransform transform = GeometryUtils.CreateTransform(this.imageTransform.pixelToTile);
                        transformedPosition = transform.Transform(position);
                    }
                    break;
            }
            transformedPosition = GeometryUtils.CreateIntPoint(transformedPosition);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.PositionText.Value = (transformedPosition.X + ", " + transformedPosition.Y);
            }), DispatcherPriority.Render);
            this.updatePositionLabelStopWatch.Restart();
        }
        #endregion methods
    }
}
