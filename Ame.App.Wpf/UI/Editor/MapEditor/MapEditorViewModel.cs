﻿using Ame.App.Wpf.UILogic.Actions;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
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

        private const double hoverSampleOpacity = 0.7;

        private IEventAggregator eventAggregator;
        private AmeSession session;

        private LayerOrderRenderer orderer; 
        private PaddedBrushModel brush;
        private CoordinateTransform imageTransform;
        public int zoomIndex;
        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private Stopwatch updatePositionLabelStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup mapBackground;
        private DrawingGroup hoverSample;
        private DrawingGroup layerItems;
        private DrawingGroup gridLines;
        private DrawingGroup layerBoundaries;
        private Brush backgroundBrush;
        private Pen backgroundPen;

        private Point lastTilePoint;

        #endregion fields


        #region constructor

        public MapEditorViewModel(IEventAggregator eventAggregator, AmeSession session, Map map)
            : this(eventAggregator, session, map, Components.Behaviors.ScrollModel.DefaultScrollModel())
        {
        }

        // TODO refactor constructor code
        public MapEditorViewModel(IEventAggregator eventAggregator, AmeSession session, Map map, IScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.Map = map ?? throw new ArgumentNullException("map is null");
            this.ScrollModel = scrollModel ?? throw new ArgumentNullException("scrollModel is null");

            this.orderer = new LayerOrderRenderer(this.session);

            this.CurrentLayer = this.Map.CurrentLayer;
            this.imageTransform = new CoordinateTransform();
            this.imageTransform.SetPixelToTile(this.Map.TileWidth.Value, this.Map.TileHeight.Value);
            this.imageTransform.SetSelectionToPixel(this.Map.TileWidth.Value / 2, this.Map.TileHeight.Value / 2);

            this.Title = this.Map.Name.Value;
            this.drawingGroup = new DrawingGroup();
            this.mapBackground = new DrawingGroup();
            this.hoverSample = new DrawingGroup();
            this.layerBoundaries = new DrawingGroup();

            this.layerItems = new DrawingGroup();
            foreach (ILayer layer in this.Map.LayerList)
            {
                this.layerItems.Children.Add(layer.Group);
                layer.IsVisible.PropertyChanged += (s, args) =>
                {
                    LayerChanged(layer);
                };
            }
            DrawLayerBoundaries();

            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.mapBackground);
            this.drawingGroup.Children.Add(this.layerItems);
            this.drawingGroup.Children.Add(this.hoverSample);
            this.drawingGroup.Children.Add(this.gridLines);
            this.drawingGroup.Children.Add(this.layerBoundaries);
            this.DrawingCanvas = new DrawingImage(this.drawingGroup);

            RenderOptions.SetEdgeMode(this.hoverSample, EdgeMode.Aliased);

            this.backgroundBrush = new SolidColorBrush(this.Map.BackgroundColor.Value);
            this.backgroundPen = new Pen(Brushes.Transparent, 0);
            redrawBackground();
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();
            this.HoverSampleOpacity = hoverSampleOpacity;

            this.session.PropertyChanged += SessionPropertyChanged;
            this.Map.LayerList.CollectionChanged += LayerListChanged;
            this.ScrollModel.PropertyChanged += ScrollModelPropertyChanged;
            
            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((Point)point));
            this.UndoCommand = new DelegateCommand(() => this.Undo());
            this.RedoCommand = new DelegateCommand(() => this.Redo());
            this.ZoomInCommand = new DelegateCommand(() => this.ScrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() =>this.ScrollModel.ZoomOut());
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

        public Map Map { get; set; }
        public ILayer CurrentLayer { get; set; }

        public DrawingImage DrawingCanvas { get; set; }

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

        public IScrollModel ScrollModel { get; set; }

        public Brush BackgroundBrush
        {
            get
            {
                return backgroundBrush;
            }
            set
            {
                this.backgroundBrush = value;
                redrawBackground();
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
                redrawBackground();
            }
        }

        public double HoverSampleOpacity
        {
            get
            {
                return this.hoverSample.Opacity;
            }
            set
            {
                this.hoverSample.Opacity = value;
            }
        }

        private IDrawingTool DrawingTool
        {
            get
            {
                return this.session.DrawingTool;
            }
        }

        #endregion properties


        #region methods

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
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
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

        public void DrawGrid()
        {
            DrawGrid(this.IsGridOn);
        }

        public void DrawGrid(bool drawGrid)
        {
            this.IsGridOn = drawGrid;
            if (this.IsGridOn)
            {
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.Map);
                double thickness = 1 / this.ScrollModel.ZoomLevels[this.ScrollModel.ZoomIndex].zoom;
                gridParameters.DrawingPen.Thickness = thickness < Global.maxGridThickness ? thickness : Global.maxGridThickness;
                DrawingGroup group = gridParameters.CreateGrid();
                this.gridLines.Children = group.Children;
            }
            else
            {
                this.gridLines.Children.Clear();
            }
            RaisePropertyChanged(nameof(this.IsGridOn));
            RaisePropertyChanged(nameof(this.DrawingCanvas));
        }

        public void Undo()
        {
            this.Map.Undo();
        }

        public void Redo()
        {
            this.Map.Redo();
        }

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
            return this.Map;
        }

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public override void ExportAs(string path, BitmapEncoder encoder)
        {
            if (encoder == null)
            {
                return;
            }
            // Remove the hover sample
            this.HoverSampleOpacity = 0;
            var drawingImage = new System.Windows.Controls.Image { Source = this.DrawingCanvas };
            var width = this.DrawingCanvas.Width;
            var height = this.DrawingCanvas.Height;
            drawingImage.Arrange(new Rect(0, 0, width, height));

            var bitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(drawingImage);
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new FileStream(path, FileMode.Create))
            {
                encoder.Save(stream);
            }
            this.HoverSampleOpacity = hoverSampleOpacity;
        }

        private void SessionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AmeSession.CurrentLayer))
            {
                Console.WriteLine("Rendering");
                DrawLayerBoundaries();
            }
        }

        private void DrawLayerBoundaries()
        {
            if (this.session.CurrentLayer != null)
            {
                LayerBoundariesRenderable renderer = new LayerBoundariesRenderable(this.session.CurrentLayer);

                double thickness = 4 / this.ScrollModel.ZoomLevels[this.ScrollModel.ZoomIndex].zoom;
                thickness = thickness < Global.maxGridThickness ? thickness : Global.maxGridThickness;
                renderer.DrawingPen.Thickness = thickness;
                renderer.DrawingPenDashed.Thickness = thickness;
                DrawingGroup group = renderer.CreateBoundaries();
                this.layerBoundaries.Children = group.Children;
            }
        }

        private void LayerListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
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
                    // TODO make groups work with this
                    ILayer oldLayer = this.session.CurrentLayerList[oldIndex];
                    ILayer newLayer = this.session.CurrentLayerList[newIndex];

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

        private void DrawPressed(Point point)
        {
            if (!ImageUtils.Intersects(this.DrawingCanvas, point))
            {
                return;
            }
            Point topLeftTilePixelPoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            this.DrawingTool.DrawPressed(this.Map, topLeftTilePixelPoint);
        }

        private void DrawReleased(Point point)
        {
            if (!ImageUtils.Intersects(this.DrawingCanvas, point))
            {
                return;
            }
            Point topLeftTilePixelPoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            this.DrawingTool.DrawReleased(this.Map, topLeftTilePixelPoint);
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
            if (!ImageUtils.Intersects(this.mapBackground, point))
            {
                return;
            }
            Point topLeftTilePixelPoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            if (topLeftTilePixelPoint == this.lastTilePoint)
            {
                return;
            }
            this.lastTilePoint = topLeftTilePixelPoint;
            this.DrawingTool.DrawHoverSample(this.hoverSample, topLeftTilePixelPoint);
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

        private void redrawBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.Map.PixelWidth + this.Map.TileWidth.Value;
            extendedSize.Height = this.Map.PixelHeight + this.Map.TileHeight.Value;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.Map.TileWidth.Value / 2;
            extendedPoint.Y = -this.Map.TileHeight.Value / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            Point backgroundLocation = new Point(0, 0);
            Size backgroundSize = new Size(this.Map.PixelWidth, this.Map.PixelHeight);
            Rect rect = new Rect(backgroundLocation, backgroundSize);

            using (DrawingContext context = this.mapBackground.Open())
            {
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                context.DrawRectangle(this.backgroundBrush, this.backgroundPen, rect);
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
                        GeneralTransform transform = GeometryUtils.CreateTransform(this.imageTransform.pixelToTile);
                        transformedPosition = transform.Transform(position);
                    }
                    break;
            }
            transformedPosition = GeometryUtils.CreateIntPoint(transformedPosition);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            }), DispatcherPriority.Render);
            this.updatePositionLabelStopWatch.Restart();
        }
        private void ScrollModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(ScrollModel.ZoomIndex):
                    this.gridLines.Children.Clear();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DrawGrid();
                    }),
                    DispatcherPriority.Background);
                    break;
                default:
                    break;
            }
        }

        #endregion methods
    }
}
