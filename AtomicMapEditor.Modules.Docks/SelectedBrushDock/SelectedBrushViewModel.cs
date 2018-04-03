using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Docks.SelectedBrushDock
{
    public class SelectedBrushViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region Constructor & destructor

        public SelectedBrushViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.eventAggregator = eventAggregator;
            this.Title = "Selected Brush";

            this.CanvasGridItems = new ObservableCollection<Visual>();

            this.ZoomLevels = new List<ZoomLevel>();
            this.ZoomLevels.Add(new ZoomLevel("12.5%", 0.125));
            this.ZoomLevels.Add(new ZoomLevel("25%", 0.25));
            this.ZoomLevels.Add(new ZoomLevel("50%", 0.5));
            this.ZoomLevels.Add(new ZoomLevel("100%", 1));
            this.ZoomLevels.Add(new ZoomLevel("200%", 2));
            this.ZoomLevels.Add(new ZoomLevel("400%", 4));
            this.ZoomLevels.Add(new ZoomLevel("800%", 8));
            this.ZoomLevels.Add(new ZoomLevel("1600%", 16));
            this.ZoomLevels.Add(new ZoomLevel("3200%", 32));
            this.ZoomLevels = this.ZoomLevels.OrderBy(f => f.zoom).ToList();

            this.ZoomIndex = 3;
            this.Scale = ScaleType.Pixel;
            this.PositionText = "0, 0";

            this.ShowGridCommand = new DelegateCommand(() => DrawGrid(this.IsGridOn));
            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(zoomLevel => SetZoom(zoomLevel));

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Subscribe(UpdateBrushImage);
        }

        #endregion Constructor & destructor


        #region properties

        public ICommand ShowGridCommand { get; private set; }
        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public String PositionText { get; set; }
        public ScaleType Scale { get; set; }
        public List<ZoomLevel> ZoomLevels { get; set; }
        public bool IsGridOn { get; set; }
        public ObservableCollection<Visual> CanvasGridItems { get; set; }

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

        public BitmapImage BrushImage { get; set; }

        public override DockType DockType
        {
            get
            {
                return DockType.SelectedBrush;
            }
        }

        #endregion properties


        #region methods

        private void UpdateBrushImage(UpdateBrushMessage message)
        {
            // TODO add this into a processing class/utils
            BrushModel brushModel = message.BrushModel;
            BitmapImage croppedBitmap = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                brushModel.image.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                croppedBitmap.BeginInit();
                croppedBitmap.StreamSource = ms;
                croppedBitmap.CacheOption = BitmapCacheOption.OnLoad;
                croppedBitmap.EndInit();
            }
            this.BrushImage = croppedBitmap;
            RaisePropertyChanged(nameof(this.BrushImage));
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
                // TODO find a way to get tile width and height
                GridModel gridParameters = new GridModel()
                {
                    width = this.BrushImage.PixelWidth,
                    height = this.BrushImage.PixelHeight,
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
            RaisePropertyChanged(nameof(this.IsGridOn));
            RaisePropertyChanged(nameof(this.CanvasGridItems));
        }

        // TODO delegate the zoom in/out/set command to another class Maybe have a infobar class in
        // the extended/custom components
        public void ZoomIn()
        {
            if (this.ZoomIndex < this.ZoomLevels.Count - 1)
            {
                this.ZoomIndex += 1;
            }
        }

        public void ZoomOut()
        {
            if (this.ZoomIndex > 0)
            {
                this.ZoomIndex -= 1;
            }
        }

        public void SetZoom(ZoomLevel selectedZoomLevel)
        {
            int zoomIndex = this.ZoomLevels.FindIndex(r => r.zoom == selectedZoomLevel.zoom);
            if (zoomIndex == -1)
            {
                this.ZoomLevels.Add(selectedZoomLevel);
                zoomIndex = this.ZoomLevels.FindIndex(r => r.zoom == selectedZoomLevel.zoom);
            }
            if (zoomIndex > ZoomLevels.Count - 1)
            {
                zoomIndex = ZoomLevels.Count - 1;
            }
            else if (zoomIndex < 0)
            {
                zoomIndex = 0;
            }
            this.ZoomIndex = zoomIndex;
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
