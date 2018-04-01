using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using AtomicMapEditor.Infrastructure.BaseTypes;
using Prism.Commands;

namespace Ame.Modules.MapEditor.Editor
{
    public class MainEditorViewModel : EditorViewModelTemplate
    {
        #region Constructor & destructor

        public MainEditorViewModel()
        {
            this.Title = "Main Editor";

            this.ZoomLevels = new List<ZoomLevel>();
            this.ZoomLevels.Add(new ZoomLevel(0.125));
            this.ZoomLevels.Add(new ZoomLevel(0.25));
            this.ZoomLevels.Add(new ZoomLevel(0.5));
            this.ZoomLevels.Add(new ZoomLevel(1));
            this.ZoomLevels.Add(new ZoomLevel(2));
            this.ZoomLevels.Add(new ZoomLevel(4));
            this.ZoomLevels.Add(new ZoomLevel(8));
            this.ZoomLevels.Add(new ZoomLevel(16));
            this.ZoomLevels.Add(new ZoomLevel(32));
            this.ZoomLevels = this.ZoomLevels.OrderBy(f => f.zoom).ToList();
            this.ZoomIndex = 3;
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";

            this.UpdatePositionCommand = new DelegateCommand<object>(point => UpdatePosition((Point)point));
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(zoomIndex => SetZoom(zoomIndex));
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

        public override DockType DockType
        {
            get
            {
                return DockType.MapEditor;
            }
        }

        #endregion properties

        #region methods
        
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
            switch (Scale)
            {
                case ScaleType.Pixel:
                    transformedPosition = position;
                    break;

                case ScaleType.Tile:
                    transformedPosition = position;
                    break;
            }
            transformedPosition = PointUtils.IntPoint(transformedPosition);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
        }

        #endregion methods
    }
}
