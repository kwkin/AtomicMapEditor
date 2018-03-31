﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AtomicMapEditor.Infrastructure.BaseTypes;
using AtomicMapEditor.Infrastructure.Models;
using Prism.Commands;

namespace AtomicMapEditor.Modules.MapEditor.Editor
{
    public class MainEditorViewModel : EditorViewModelTemplate
    {
        #region Constructor & destructor

        public MainEditorViewModel()
        {
            this.Title = "Main Editor";
            this.ContentId = "Map Editor";

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

        public ScaleType Scale { get; set; }
        public String PositionText { get; set; }
        public List<ZoomLevel> ZoomLevels { get; set; }
        private int _ZoomIndex;
        public int ZoomIndex
        {
            get { return _ZoomIndex; }
            set { SetProperty(ref _ZoomIndex, value); }
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
            this.PositionText = (Math.Floor(transformedPosition.X) + ", " + Math.Floor(transformedPosition.Y));
            RaisePropertyChanged(nameof(this.PositionText));
        }

        #endregion methods
    }
}
