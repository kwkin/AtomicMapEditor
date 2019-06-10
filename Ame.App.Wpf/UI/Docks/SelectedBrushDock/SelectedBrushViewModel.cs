﻿using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ame.App.Wpf.UI.Docks.SelectedBrushDock
{
    public class SelectedBrushViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        private DrawingGroup drawingGroup;
        private DrawingGroup extendedBackground;
        private DrawingGroup selectedBrushImage;
        private DrawingGroup gridLines;
        private CoordinateTransform imageTransform;
        private GridModel gridModel;

        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private Stopwatch updatePositionLabelStopWatch;

        #endregion fields


        #region constructor

        public SelectedBrushViewModel(IEventAggregator eventAggregator)
            : this(eventAggregator, Components.Behaviors.ScrollModel.DefaultScrollModel())
        {
        }

        public SelectedBrushViewModel(IEventAggregator eventAggregator, IScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.ScrollModel = scrollModel ?? throw new ArgumentNullException("scrollModel is null");

            this.Title = "Selected Brush";

            this.imageTransform = new CoordinateTransform();
            this.drawingGroup = new DrawingGroup();
            this.extendedBackground = new DrawingGroup();
            this.selectedBrushImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.selectedBrushImage);
            this.drawingGroup.Children.Add(this.extendedBackground);
            this.drawingGroup.Children.Add(this.gridLines);
            this.BrushImage = new DrawingImage(this.drawingGroup);
            RenderOptions.SetEdgeMode(this.selectedBrushImage, EdgeMode.Aliased);

            this.gridModel = new PaddedGridRenderable();
            this.Scale = ScaleType.Pixel;
            this.PositionText = "0, 0";
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            this.ScrollModel.PropertyChanged += ScrollModelPropertyChanged;

            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((Point)point));
            this.ZoomInCommand = new DelegateCommand(() => this.ScrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => this.ScrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => this.ScrollModel.SetZoom(zoomLevel));

            this.eventAggregator.GetEvent<NewPaddedBrushEvent>().Subscribe((brushEvent) =>
            {
                UpdateBrushImage(brushEvent);
            }, ThreadOption.UIThread);
        }

        #endregion constructor


        #region properties

        public ICommand ShowCollisionsCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand HandleMouseMoveCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public string PositionText { get; set; }
        public ScaleType Scale { get; set; }
        public bool IsGridOn { get; set; }

        public IScrollModel ScrollModel { get; set; }

        public DrawingImage BrushImage { get; set; }

        #endregion properties


        #region methods

        public void RefreshGrid()
        {
            DrawGrid(this.IsGridOn);
        }

        public void DrawGrid(bool drawGrid)
        {
            this.IsGridOn = drawGrid;
            if (this.IsGridOn)
            {
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.gridModel);
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
            RaisePropertyChanged(nameof(this.BrushImage));
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

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.BrushImage != null)
            {
                switch (Scale)
                {
                    case ScaleType.Pixel:
                        transformedPosition = position;
                        break;

                    case ScaleType.Tile:
                        if (this.BrushImage != null)
                        {
                            GeneralTransform transform = GeometryUtils.CreateTransform(this.imageTransform.pixelToTile);
                            transformedPosition = transform.Transform(position);
                        }
                        break;
                }
            }
            transformedPosition = GeometryUtils.CreateIntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);

            RaisePropertyChanged(nameof(this.PositionText));
            this.updatePositionLabelStopWatch.Restart();
        }

        private void UpdateBrushImage(PaddedBrushModel brushModel)
        {
            using (DrawingContext context = this.selectedBrushImage.Open())
            {
                foreach (Tile tile in brushModel.Tiles)
                {
                    context.DrawDrawing(tile.Image);
                }
            }
            this.gridModel.SetHeightWithRows(brushModel.Rows(), brushModel.TileHeight);
            this.gridModel.SetWidthWithColumns(brushModel.Columns(), brushModel.TileWidth);

            this.imageTransform.SetPixelToTile(this.gridModel.TileWidth, this.gridModel.TileHeight);
            this.imageTransform.SetSlectionToPixel(this.gridModel.TileWidth / 2, this.gridModel.TileHeight / 2);

            redrawExtendedBackground();
            RefreshGrid();
            RaisePropertyChanged(nameof(this.BrushImage));
        }

        private void redrawExtendedBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.gridModel.PixelWidth + this.gridModel.TileWidth;
            extendedSize.Height = this.gridModel.PixelHeight + this.gridModel.TileHeight;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.gridModel.TileWidth / 2;
            extendedPoint.Y = -this.gridModel.TileHeight / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            using (DrawingContext context = this.extendedBackground.Open())
            {
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
            }
        }

        private void ScrollModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.gridLines.Children.Clear();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                RefreshGrid();
            }),
            DispatcherPriority.Background);
        }

        #endregion methods
    }
}
