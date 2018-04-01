using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Ame.Infrastructure.BaseTypes;
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
            this.eventAggregator = eventAggregator;
            this.Title = "Selected Brush";

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

            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(zoomLevel => SetZoom(zoomLevel));

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Subscribe(UpdateBrushImage);
        }

        #endregion Constructor & destructor


        #region properties

        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public String PositionText { get; set; }
        public ScaleType Scale { get; set; }
        public List<ZoomLevel> ZoomLevels { get; set; }
        public int ZoomIndex { get; set; }

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

        // TODO delegate the zoom in/out/set command to another class Maybe have a infobar class in
        // the extended/custom components
        public void ZoomIn()
        {
            if (this.ZoomIndex < this.ZoomLevels.Count - 1)
            {
                this.ZoomIndex += 1;
                RaisePropertyChanged(nameof(this.ZoomIndex));
            }
        }

        public void ZoomOut()
        {
            if (this.ZoomIndex > 0)
            {
                this.ZoomIndex -= 1;
                RaisePropertyChanged(nameof(this.ZoomIndex));
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
            RaisePropertyChanged(nameof(this.ZoomIndex));
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
