﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.DrawingBrushes;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.MapEditor.Editor
{
    public class MapEditorViewModel : EditorViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private IDrawingTool drawingTool;
        private BrushModel brush;
        private CoordinateTransform imageTransform;
        public int zoomIndex;
        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private Stopwatch updatePositionLabelStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup mapBackground;
        private DrawingGroup layerItems;
        private DrawingGroup gridLines;
        private Brush backgroundBrush;
        private Pen backgroundPen;

        #endregion fields


        #region constructor

        public MapEditorViewModel(IEventAggregator eventAggregator, Map map) : this(eventAggregator, map, new ScrollModel())
        {
        }

        public MapEditorViewModel(IEventAggregator eventAggregator, Map map, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
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
            this.Map = map;
            this.scrollModel = scrollModel;

            this.CurrentLayer = this.Map.CurrentLayer as Layer;
            this.drawingTool = new StampTool();
            this.imageTransform = new CoordinateTransform();
            this.imageTransform.SetPixelToTile(this.Map.TileWidth, this.Map.TileHeight);
            this.imageTransform.SetSlectionToPixel(this.Map.TileWidth / 2, this.Map.TileHeight / 2);

            this.Title = map.Name;
            this.drawingGroup = new DrawingGroup();
            this.mapBackground = new DrawingGroup();
            this.layerItems = this.Map.CurrentLayer.LayerGroup;
            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.mapBackground);
            this.drawingGroup.Children.Add(this.layerItems);
            this.drawingGroup.Children.Add(this.gridLines);
            this.DrawingCanvas = new DrawingImage(this.drawingGroup);

            this.backgroundBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#b8e5ed"));
            this.backgroundPen = new Pen(Brushes.Transparent, 0);
            redrawBackground();
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
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            this.ShowGridCommand = new DelegateCommand(
                () => DrawGrid(this.IsGridOn));
            this.HandleMouseMoveCommand = new DelegateCommand<object>(
                (point) => HandleMouseMove((Point)point));
            this.UndoCommand = new DelegateCommand(
                () => this.Undo());
            this.RedoCommand = new DelegateCommand(
                () => this.Redo());
            this.ZoomInCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(
                (zoomLevel) => this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel));
            this.HandleLeftClickDownCommand = new DelegateCommand<object>(
                (point) => HandleLeftClickDown((Point)point));
            this.HandleLeftClickUpCommand = new DelegateCommand<object>(
                (point) => HandleLeftClickUp((Point)point));

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Subscribe(
                UpdateBrushImage,
                ThreadOption.PublisherThread);
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
        public bool IsGridOn { get; set; }
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
            draw(pixelPoint);
        }

        public void HandleLeftClickUp(Point selectPoint)
        {

        }

        public void HandleMouseMove(Point position)
        {
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.imageTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(position);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(pixelPoint);
            }
        }

        public void UpdateBrushImage(BrushModel brushModel)
        {
            this.brush = brushModel;
            this.drawingTool.Brush = this.brush;
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

        private void draw(Point point)
        {
            if (!ImageUtils.Intersects(this.DrawingCanvas, point))
            {
                return;
            }
            Point tilePoint = this.imageTransform.PixelToTopLeftTileEdge(point);
            this.drawingTool.Apply(this.Map, tilePoint);
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
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
            this.updatePositionLabelStopWatch.Restart();
        }

        #endregion methods
    }
}
