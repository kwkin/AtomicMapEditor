﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.IO;
using Ame.Infrastructure.UILogic;

namespace Ame.App.Wpf.UI.Editor.MapEditor
{
    // TODO get notified when a new layer is added
    public class MapEditorViewModel : EditorViewModelTemplate
    {
        #region fields

        private const double hoverSampleOpacity = 0.7;

        private IEventAggregator eventAggregator;
        private AmeSession session;
        private IScrollModel scrollModel;

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
        private Brush backgroundBrush;
        private Pen backgroundPen;

        private Point lastTilePoint;

        #endregion fields


        #region constructor

        public MapEditorViewModel(IEventAggregator eventAggregator, AmeSession session, Map map)
            : this(eventAggregator, session, map, ScrollModel.DefaultScrollModel())
        {
        }

        public MapEditorViewModel(IEventAggregator eventAggregator, AmeSession session, Map map, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel");
            }
            this.eventAggregator = eventAggregator;
            this.session = session;
            this.Map = map;
            this.scrollModel = scrollModel;

            this.CurrentLayer = this.Map.CurrentLayer as Layer;
            this.imageTransform = new CoordinateTransform();
            this.imageTransform.SetPixelToTile(this.Map.TileWidth, this.Map.TileHeight);
            this.imageTransform.SetSlectionToPixel(this.Map.TileWidth / 2, this.Map.TileHeight / 2);

            this.Title = this.Map.Name;
            this.drawingGroup = new DrawingGroup();
            this.mapBackground = new DrawingGroup();
            this.hoverSample = new DrawingGroup();

            this.layerItems = new DrawingGroup();
            foreach (Layer layer in this.Map.LayerList)
            {
                this.layerItems.Children.Add(layer.Group);
            }

            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.mapBackground);
            this.drawingGroup.Children.Add(this.layerItems);
            this.drawingGroup.Children.Add(this.hoverSample);
            this.drawingGroup.Children.Add(this.gridLines);
            this.DrawingCanvas = new DrawingImage(this.drawingGroup);

            RenderOptions.SetEdgeMode(this.hoverSample, EdgeMode.Aliased);

            this.backgroundBrush = new SolidColorBrush(this.Map.BackgroundColor);
            this.backgroundPen = new Pen(Brushes.Transparent, 0);
            redrawBackground();
            this.ZoomLevels = this.scrollModel.ZoomLevels;
            this.ZoomIndex = this.scrollModel.ZoomIndex;
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();
            this.HoverSampleOpacity = hoverSampleOpacity;

            this.Map.LayerList.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Layer layer in e.NewItems)
                    {
                        this.layerItems.Children.Add(layer.Group);
                    }
                }
            };

            this.ShowGridCommand = new DelegateCommand(() =>
            {
                DrawGrid(this.IsGridOn);
            });
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) =>
            {
                HandleMouseMove((Point)point);
            });
            this.UndoCommand = new DelegateCommand(() =>
            {
                this.Undo();
            });
            this.RedoCommand = new DelegateCommand(() =>
            {
                this.Redo();
            });
            this.ZoomInCommand = new DelegateCommand(() =>
            {
                this.ZoomIndex = this.scrollModel.ZoomIn();
            });
            this.ZoomOutCommand = new DelegateCommand(() =>
            {
                this.ZoomIndex = this.scrollModel.ZoomOut();
            });
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) =>
            {
                this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel);
            });
            this.HandleLeftClickDownCommand = new DelegateCommand<object>((point) =>
            {
                HandleLeftClickDown((Point)point);
            });
            this.HandleLeftClickUpCommand = new DelegateCommand<object>((point) =>
            {
                HandleLeftClickUp((Point)point);
            });

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
        public Layer CurrentLayer { get; set; }

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
            Draw(pixelPoint);
        }

        public void HandleLeftClickUp(Point selectPoint)
        {
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
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.Map.Grid);
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
            this.ZoomIndex = this.scrollModel.ZoomIn();
        }

        public override void ZoomOut()
        {
            this.ZoomIndex = this.scrollModel.ZoomOut();
        }

        public override void SetZoom(int zoomIndex)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomIndex);
        }

        public override void SetZoom(ZoomLevel zoomLevel)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel);
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

        private void Draw(Point point)
        {
            if (!ImageUtils.Intersects(this.DrawingCanvas, point))
            {
                return;
            }
            Point topLeftTilePixelPoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            this.DrawingTool.Apply(this.Map, topLeftTilePixelPoint);
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
            Rect boundaries = new Rect(new Point(0, 0), this.Map.PixelSize);
            this.DrawingTool.DrawHoverSample(this.hoverSample, topLeftTilePixelPoint, boundaries);
        }

        private void redrawBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.Map.PixelWidth + this.Map.TileWidth;
            extendedSize.Height = this.Map.PixelHeight + this.Map.TileHeight;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.Map.TileWidth / 2;
            extendedPoint.Y = -this.Map.TileHeight / 2;
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

        #endregion methods
    }
}