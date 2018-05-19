using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ame.Infrastructure.Models;
using Prism.Commands;

namespace Ame.Components.Extended
{
    public class ExtendedScrollViewer : ScrollViewer
    {
        #region fields

        private ScaleTransform scaleTransform;
        private Point lastPositionOnScroll;
        private Point lastPositionOnContent;
        private Point? lastPanPoint;

        #endregion fields


        #region constructor

        public ExtendedScrollViewer()
        {
            this.scaleTransform = new ScaleTransform(1, 1);
            this.ZoomInCommand = new DelegateCommand(() => this.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => this.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<object>((level) => this.SetZoom(level));

            this.PreviewMouseWheel += OnPreviewMouseWheel;
            this.MouseDown += OnMouseMiddleButtonDown;
            this.MouseUp += OnMouseMiddleButtonUp;
            this.MouseMove += OnMouseMove;
        }

        public ExtendedScrollViewer(ObservableCollection<ZoomLevel> ZoomLevels)
        {
            this.ZoomLevels = ZoomLevels;

            this.scaleTransform = new ScaleTransform(1, 1);
            this.ZoomInCommand = new DelegateCommand(() => this.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => this.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<object>((level) => this.SetZoom(level));

            this.PreviewMouseWheel += OnPreviewMouseWheel;
            this.MouseDown += OnMouseMiddleButtonDown;
            this.MouseUp += OnMouseMiddleButtonUp;
            this.MouseMove += OnMouseMove;
        }

        #endregion constructor


        #region properties

        public ICommand ZoomInCommand { get; set; }
        public ICommand ZoomOutCommand { get; set; }
        public ICommand SetZoomCommand { get; set; }

        public static readonly DependencyProperty ZoomLevelsProperty =
            DependencyProperty.Register(
                "ZoomLevels",
                typeof(ObservableCollection<ZoomLevel>),
                typeof(ExtendedScrollViewer),
                new PropertyMetadata(default(ObservableCollection<ZoomLevel>)));

        public ObservableCollection<ZoomLevel> ZoomLevels
        {
            get
            {
                return GetValue(ZoomLevelsProperty) as ObservableCollection<ZoomLevel>;
            }
            set
            {
                SetValue(ZoomLevelsProperty, value);
            }
        }

        public static readonly DependencyProperty ZoomIndexProperty =
            DependencyProperty.Register(
                "ZoomIndex",
                typeof(int),
                typeof(ExtendedScrollViewer),
                        new FrameworkPropertyMetadata(
                                -1,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                new PropertyChangedCallback(OnZoomIndexChanged)));

        public int ZoomIndex
        {
            get
            {
                return (int)GetValue(ZoomIndexProperty);
            }
            set
            {
                SetValue(ZoomIndexProperty, value);
            }
        }

        #endregion properties


        #region methods

        public static int ZoomIn(int currentZoom, ObservableCollection<ZoomLevel> zoomLevels)
        {
            if (currentZoom < zoomLevels.Count - 1)
            {
                currentZoom += 1;
            }
            return currentZoom;
        }

        public static int ZoomOut(int currentZoom, ObservableCollection<ZoomLevel> zoomLevels)
        {
            if (currentZoom > 0)
            {
                currentZoom -= 1;
            }
            return currentZoom;
        }

        public static int SetZoom(ZoomLevel selectedZoomLevel, ObservableCollection<ZoomLevel> zoomLevels)
        {
            int zoomIndex = zoomLevels.IndexOf(selectedZoomLevel);
            if (zoomIndex == -1)
            {
                zoomLevels.Add(selectedZoomLevel);
                zoomIndex = zoomLevels.IndexOf(selectedZoomLevel);
            }
            if (zoomIndex > zoomLevels.Count - 1)
            {
                zoomIndex = zoomLevels.Count - 1;
            }
            else if (zoomIndex < 0)
            {
                zoomIndex = 0;
            }
            return zoomIndex;
        }

        private static void OnZoomIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedScrollViewer s = (ExtendedScrollViewer)d;
            int newIndex = (int)e.NewValue;
            s.SetZoom(newIndex);
        }

        private void OnMouseMiddleButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var mousePosition = e.GetPosition(this);
                if (mousePosition.X <= this.ViewportWidth && mousePosition.Y < this.ViewportHeight)
                {
                    this.Cursor = Cursors.SizeAll;
                    this.lastPanPoint = mousePosition;
                    Mouse.Capture(this);
                }
            }
        }

        private void OnMouseMiddleButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                this.Cursor = Cursors.Arrow;
                this.ReleaseMouseCapture();
                this.lastPanPoint = null;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.lastPanPoint.HasValue)
            {
                Point currentPosition = e.GetPosition(this);
                double dX = currentPosition.X - this.lastPanPoint.Value.X;
                double dY = currentPosition.Y - this.lastPanPoint.Value.Y;
                this.ScrollToHorizontalOffset(this.HorizontalOffset - dX);
                this.ScrollToVerticalOffset(this.VerticalOffset - dY);
                this.lastPanPoint = currentPosition;
            }
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            FrameworkElement frameworkContent = this.Content as FrameworkElement;
            this.lastPositionOnContent = Mouse.GetPosition(frameworkContent);
            this.lastPositionOnScroll = Mouse.GetPosition(this);
            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
            e.Handled = true;
        }

        private void updateZoom(int zoomIndex)
        {
            this.scaleTransform.ScaleX = ZoomLevels.ElementAt(zoomIndex).zoom;
            this.scaleTransform.ScaleY = ZoomLevels.ElementAt(zoomIndex).zoom;
            this.ScrollToHorizontalOffset((this.lastPositionOnContent.X * this.scaleTransform.ScaleX) - lastPositionOnScroll.X);
            this.ScrollToVerticalOffset((this.lastPositionOnContent.Y * this.scaleTransform.ScaleY) - lastPositionOnScroll.Y);

            FrameworkElement frameworkContent = this.Content as FrameworkElement;
            frameworkContent.LayoutTransform = this.scaleTransform;
        }

        public void ZoomIn()
        {
            if (this.ZoomIndex < this.ZoomLevels.Count - 1)
            {
                this.ZoomIndex += 1;
            }
            updateZoom(this.ZoomIndex);
        }

        public void ZoomOut()
        {
            if (this.ZoomIndex > 0)
            {
                this.ZoomIndex -= 1;
            }
            updateZoom(this.ZoomIndex);
        }

        public void SetZoom(object zoomIndexObject)
        {
            int zoomIndex = (int)zoomIndexObject;
            if (this.ZoomLevels == null)
            {
                return;
            }
            if (zoomIndex > this.ZoomLevels.Count - 1)
            {
                this.ZoomIndex = this.ZoomLevels.Count - 1;
            }
            else if (zoomIndex < 0)
            {
                this.ZoomIndex = 0;
            }
            else
            {
                this.ZoomIndex = zoomIndex;
            }
            updateZoom(this.ZoomIndex);
        }

        #endregion methods
    }
}
