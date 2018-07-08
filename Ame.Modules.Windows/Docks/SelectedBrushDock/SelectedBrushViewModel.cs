using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.SelectedBrushDock
{
    public class SelectedBrushViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

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

            this.CanvasGridItems = new ObservableCollection<Visual>();
            
            if (this.scrollModel.ZoomLevels == null)
            {
                this.ZoomLevels = new ObservableCollection<ZoomLevel>();
                this.ZoomLevels.Add(new ZoomLevel(0.125));
                this.ZoomLevels.Add(new ZoomLevel(0.25));
                this.ZoomLevels.Add(new ZoomLevel(0.5));
                this.ZoomLevels.Add(new ZoomLevel(1));
                this.ZoomLevels.Add(new ZoomLevel(2));
                this.ZoomLevels.Add(new ZoomLevel(4));
                this.ZoomLevels.Add(new ZoomLevel(8));
                this.ZoomLevels.Add(new ZoomLevel(16));
                this.ZoomLevels.Add(new ZoomLevel(32));
                this.ZoomLevels.OrderBy(f => f.zoom);
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
            this.Scale = ScaleType.Pixel;
            this.PositionText = "0, 0";

            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
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
        public ObservableCollection<Visual> CanvasGridItems { get; set; }
        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }

        public int zoomIndex;
        public int ZoomIndex
        {
            get { return this.zoomIndex; }
            set
            {
                if (SetProperty(ref this.zoomIndex, value))
                {
                    this.CanvasGridItems.Clear();
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

        private void UpdateBrushImage(UpdateBrushMessage message)
        {
            BrushModel brushModel = message.BrushModel;
            DrawingGroup drawingGroup = ImageUtils.MatToDrawingGroup(ImageUtils.BitmapImageToMat(brushModel.Image));
            
            this.BrushImage = new DrawingImage(drawingGroup);
            DrawGrid();
            RaisePropertyChanged(nameof(this.BrushImage));
        }
        
        public void DrawGrid()
        {
            DrawGrid(this.IsGridOn);
        }

        public void DrawGrid(bool drawGrid)
        {
            // TODO switch from shapes namespace to media
            DrawingImage drawingImage = new DrawingImage();

            DrawingGroup gridLines = new DrawingGroup();

            this.IsGridOn = drawGrid;
            if (this.IsGridOn)
            {
                GridModel gridParameters = new GridModel()
                {
                    rows = this.BrushImage.Width / 32,
                    columns = this.BrushImage.Height / 32,
                    cellWidth = 32,
                    cellHeight = 32
                };
                GridFactory.StrokeThickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
                this.CanvasGridItems = GridFactory.CreateGrid(gridParameters);
            }
            else
            {
                this.CanvasGridItems.Clear();
            }
            if (this.BrushImage != null)
            {
                DrawingGroup currentDrawingGroup = this.BrushImage.Drawing as DrawingGroup;
                if (currentDrawingGroup != null)
                {
                    if (currentDrawingGroup.Children.Count > 1)
                    {
                        currentDrawingGroup.Children.RemoveAt(1);
                    }
                }
            }
            foreach (Visual visual in this.CanvasGridItems)
            {
                Line line = visual as Line;
                if (line != null)
                {
                    GeometryDrawing geometry = new GeometryDrawing();
                    geometry.Pen = new Pen(Brushes.Black, 1);
                    Point pointStart = new Point(line.X1, line.Y1);
                    Point pointStop = new Point(line.X2, line.Y2);
                    geometry.Geometry = new LineGeometry(pointStart, pointStop);
                    gridLines.Children.Add(geometry);
                }
            }
            if (this.BrushImage != null)
            {
                DrawingGroup currentDrawingGroup = this.BrushImage.Drawing as DrawingGroup;
                if (currentDrawingGroup != null)
                {
                    (this.BrushImage.Drawing as DrawingGroup).Children.Add(gridLines);
                }
            }

            RaisePropertyChanged(nameof(this.BrushImage));
            RaisePropertyChanged(nameof(this.IsGridOn));
            RaisePropertyChanged(nameof(this.CanvasGridItems));
        }

        private void UpdatePosition(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.BrushImage != null)
            {
                switch (this.Scale)
                {
                    case ScaleType.Pixel:
                        transformedPosition = position;
                        break;

                    default:
                        break;
                }
            }
            transformedPosition = PointUtils.IntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
        }

        #endregion methods
    }
}
