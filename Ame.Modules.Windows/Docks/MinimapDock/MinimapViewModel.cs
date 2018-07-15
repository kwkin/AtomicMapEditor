using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.MinimapDock
{
    public class MinimapViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        #endregion fields


        #region constructor

        public MinimapViewModel(IEventAggregator eventAggregator) 
            : this(eventAggregator, new ScrollModel())
        {
        }

        public MinimapViewModel(IEventAggregator eventAggregator, ScrollModel scrollModel)
        {
            this.Title = "Minimap";
            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;

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

            this.FitMinimapCommand = new DelegateCommand(() => FitMinimap());
            this.ToggleGridCommand = new DelegateCommand(() => ToggleGrid());
            this.ToggleCollisionCommand = new DelegateCommand(() => ToggleCollision());
            this.CenterOnPointCommand = new DelegateCommand(() => CenterOnPoint());
            this.ZoomInCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(
                () => this.ZoomIndex = this.scrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>(
                (zoomLevel) => this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel));
            this.UpdatePositionCommand = new DelegateCommand<object>(
                (point) => UpdatePosition((Point)point));
        }

        #endregion constructor


        #region properties

        public ICommand FitMinimapCommand { get; private set; }
        public ICommand ToggleGridCommand { get; private set; }
        public ICommand ToggleCollisionCommand { get; private set; }
        public ICommand CenterOnPointCommand { get; private set; }
        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        public string PositionText { get; set; }
        public ScaleType Scale { get; set; }
        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }
        public int zoomIndex;
        public int ZoomIndex
        {
            get { return this.zoomIndex; }
            set
            {
                SetProperty(ref this.zoomIndex, value);
            }
        }

        #endregion properties


        #region methods

        private void FitMinimap()
        {
            Console.WriteLine("Fit minimap");
        }

        private void ToggleGrid()
        {
            Console.WriteLine("Toggle grid minimap");
        }

        private void ToggleCollision()
        {
            Console.WriteLine("Toggle collision minimap");
        }

        private void CenterOnPoint()
        {
            Console.WriteLine("Center On Point");
        }

        private void UpdatePosition(Point position)
        {
            Point transformedPosition = GeometryUtils.CreateIntPoint(position);
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
        }

        #endregion methods
    }
}
