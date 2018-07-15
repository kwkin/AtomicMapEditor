using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.SelectedBrushDock
{
    // TODO add transformation between pixels and tiles
    public class SelectedBrushViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private DrawingGroup drawingGroup;
        private DrawingGroup selectedBrushImage;
        private DrawingGroup gridLines;

        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private Stopwatch updatePositionLabelStopWatch;

        #endregion fields


        #region constructor

        public SelectedBrushViewModel(IEventAggregator eventAggregator) : this(eventAggregator, new ScrollModel())
        {
        }

        public SelectedBrushViewModel(IEventAggregator eventAggregator, IScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel");
            }
            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;
            this.Title = "Selected Brush";

            this.BrushImage = new DrawingImage();
            this.drawingGroup = new DrawingGroup();
            this.selectedBrushImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.selectedBrushImage);
            this.drawingGroup.Children.Add(this.gridLines);
            this.BrushImage.Drawing = this.drawingGroup;

            if (this.scrollModel.ZoomLevels == null)
            {
                this.ZoomLevels = ZoomLevel.CreateZoomList(0.125, 0.25, 0.5, 1, 2, 4, 8, 16, 32);
                this.scrollModel.ZoomLevels = this.ZoomLevels;
                this.ZoomIndex = 3;
                this.scrollModel.ZoomIndex = this.ZoomIndex;
            }
            else
            {
                this.ZoomLevels = this.scrollModel.ZoomLevels;
                if (this.scrollModel.ZoomIndex < 0 || this.scrollModel.ZoomIndex >= this.ZoomLevels.Count)
                {
                    this.ZoomIndex = 3;
                    this.scrollModel.ZoomIndex = this.ZoomIndex;
                }
            }
            this.Scale = ScaleType.Pixel;
            this.PositionText = "0, 0";
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            this.ShowGridCommand = new DelegateCommand(
                () => DrawGrid(this.IsGridOn));
            this.UpdatePositionCommand = new DelegateCommand<object>(
                (point) => UpdateMousePosition((Point)point));
            this.ZoomInCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(
                (zoomLevel) => this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel));

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Subscribe(UpdateBrushImage);
        }

        #endregion constructor


        #region properties

        public ICommand ShowGridCommand { get; private set; }
        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public string PositionText { get; set; }
        public ScaleType Scale { get; set; }
        public bool IsGridOn { get; set; }
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

        public DrawingImage BrushImage { get; set; }

        #endregion properties


        #region methods

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
                    Rows = this.BrushImage.Width / 32,
                    Columns = this.BrushImage.Height / 32,
                    CellWidth = 32,
                    CellHeight = 32
                };
                gridParameters.DrawingPen.Thickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
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
            RaisePropertyChanged(nameof(this.BrushImage));
        }

        public void UpdateMousePosition(Point position)
        {
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(position);
            }
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
                        break;
                }
            }
            transformedPosition = GeometryUtils.CreateIntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);

            RaisePropertyChanged(nameof(this.PositionText));
            this.updatePositionLabelStopWatch.Restart();
        }

        private void UpdateBrushImage(UpdateBrushMessage message)
        {
            BrushModel brushModel = message.BrushModel;
            using (DrawingContext context = this.selectedBrushImage.Open())
            {
                context.DrawDrawing(brushModel.Image);
            }
            DrawGrid();
            RaisePropertyChanged(nameof(this.BrushImage));
        }

        #endregion methods
    }
}
