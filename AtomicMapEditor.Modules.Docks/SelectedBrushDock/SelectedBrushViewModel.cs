using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AtomicMapEditor.Infrastructure.BaseTypes;
using AtomicMapEditor.Infrastructure.Events;
using AtomicMapEditor.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;

namespace AtomicMapEditor.Modules.Docks.SelectedBrushDock
{
    public class SelectedBrushViewModel : DockViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region Constructor & destructor

        public SelectedBrushViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.Title = "Selected Brush";
            this.ContentId = "Selected Brush";

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

            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((System.Windows.Point)point));
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<object>(zoomLevel => SetZoom((int)zoomLevel));

            this.eventAggregator.GetEvent<UpdateBrushEvent>().Subscribe(UpdateBrushImage);
        }

        #endregion Constructor & destructor


        #region properties

        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public BitmapImage BrushImage { get; set; }

        private String _PositionText;
        public String PositionText
        {
            get { return _PositionText; }
            set { SetProperty(ref _PositionText, value); }
        }

        public ScaleType Scale { get; set; }
        public List<ZoomLevel> ZoomLevels { get; set; }

        private int _ZoomIndex;
        public int ZoomIndex
        {
            get { return _ZoomIndex; }
            set { SetProperty(ref _ZoomIndex, value); }
        }

        #endregion properties


        #region methods


        private void UpdateBrushImage(UpdateBrushMessage message)
        {
            BrushModel brushModel = message.BrushModel;
            System.Drawing.Image croppedImage = brushModel.image.Bitmap;

            BitmapImage croppedBitmap = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                croppedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                croppedBitmap.BeginInit();
                croppedBitmap.StreamSource = ms;
                croppedBitmap.CacheOption = BitmapCacheOption.OnLoad;
                croppedBitmap.EndInit();
            }
            this.BrushImage = croppedBitmap;
            RaisePropertyChanged(nameof(this.BrushImage));
        }

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

        public void SetZoom(int zoomIndex)
        {
            int zoom = zoomIndex;
            if (zoom > ZoomLevels.Count - 1)
            {
                zoom = ZoomLevels.Count - 1;
            }
            else if (zoom < 0)
            {
                zoom = 0;
            }
            this.ZoomIndex = zoom;
        }

        private void UpdatePosition(System.Windows.Point position)
        {
            System.Windows.Point transformedPosition = new System.Windows.Point(0, 0);
            if (this.BrushImage != null)
            {
                switch (Scale)
                {
                    case ScaleType.Pixel:
                        transformedPosition = position;
                        break;

                    default:
                        break;
                }
            }
            this.PositionText = (Math.Floor(transformedPosition.X) + ", " + Math.Floor(transformedPosition.Y));
            RaisePropertyChanged(nameof(this.PositionText));
        }

        #endregion methods
    }
}
