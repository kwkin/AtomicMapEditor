using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ame.App.Wpf.UI.Docks.MinimapDock
{
    public class MinimapViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        private DrawingGroup minimapLayers;

        #endregion fields


        #region constructor

        public MinimapViewModel(IEventAggregator eventAggregator, AmeSession session)
            : this(eventAggregator, session, Components.Behaviors.ScrollModel.DefaultScrollModel())
        {
        }

        public MinimapViewModel(IEventAggregator eventAggregator, AmeSession session, IScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session");
            this.ScrollModel = scrollModel ?? throw new ArgumentNullException("scrollModel");

            this.Title = "Minimap";
            
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";

            // TODO ensure this works for non-square maps
            this.minimapLayers = new DrawingGroup();
            if (this.session.CurrentMap != null)
            {
                Map currentMap = this.session.CurrentMap;
                DrawingGroup filled = new DrawingGroup();
                using (DrawingContext context = filled.Open())
                {
                    Rect drawingRect = new Rect(0, 0, currentMap.PixelWidth, currentMap.PixelHeight);
                    context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                }
                this.minimapLayers.Children.Add(filled);
                foreach (Layer layer in this.session.CurrentLayerList)
                {
                    this.minimapLayers.Children.Add(layer.Group);
                }
            }
            this.minimapPreview = new DrawingImage(this.minimapLayers);

            this.session.PropertyChanged += SessionChanged;

            this.FitMinimapCommand = new DelegateCommand(() => FitMinimap());
            this.ToggleGridCommand = new DelegateCommand(() => ToggleGrid());
            this.ToggleCollisionsCommand = new DelegateCommand(() => ToggleCollisions());
            this.CenterOnPointCommand = new DelegateCommand(() => CenterOnPoint());
            this.ZoomInCommand = new DelegateCommand(() => this.ScrollModel.ZoomIn() );
            this.ZoomOutCommand = new DelegateCommand(() => this.ScrollModel.ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => this.ScrollModel.SetZoom(zoomLevel));
            this.UpdatePositionCommand = new DelegateCommand<object>((point) => UpdatePosition((Point)point));
            this.SetRatioCommand = new DelegateCommand<object>((ratio) => UpdateRatio(Convert.ToDouble(ratio)));
        }

        #endregion constructor


        #region properties

        public ICommand FitMinimapCommand { get; private set; }
        public ICommand ToggleGridCommand { get; private set; }
        public ICommand ToggleCollisionsCommand { get; private set; }
        public ICommand CenterOnPointCommand { get; private set; }
        public ICommand UpdatePositionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }
        public ICommand SetRatioCommand { get; private set; }

        public string PositionText { get; set; }
        public ScaleType Scale { get; set; }


        public IScrollModel ScrollModel { get; set; }

        private DrawingImage minimapPreview;
        public DrawingImage MinimapPreview
        {
            get
            {
                return this.minimapPreview;
            }
        }

        #endregion properties


        #region methods

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        private void SessionChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(AmeSession.CurrentMap):
                    this.minimapLayers.Children.Clear();
                    Map currentMap = this.session.CurrentMap;
                    DrawingGroup filled = new DrawingGroup();
                    using (DrawingContext context = filled.Open())
                    {
                        Rect drawingRect = new Rect(0, 0, currentMap.PixelWidth, currentMap.PixelHeight);
                        context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                    }
                    this.minimapLayers.Children.Add(filled);
                    foreach (Layer layer in this.session.CurrentLayerList)
                    {
                        this.minimapLayers.Children.Add(layer.Group);
                    }
                    this.session.CurrentLayerList.CollectionChanged += LayerListChanged;
                    break;
                default:
                    break;
            }
        }

        private void LayerListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Layer layer in e.NewItems)
                    {
                        this.minimapLayers.Children.Add(layer.Group);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Layer layer in e.NewItems)
                    {
                        this.minimapLayers.Children.Remove(layer.Group);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        Drawing layer = this.minimapLayers.Children[oldIndex];
                        this.minimapLayers.Children[oldIndex] = this.minimapLayers.Children[newIndex];
                        this.minimapLayers.Children[newIndex] = layer;
                    }
                    break;
                default:
                    break;
            }
        }

        private void FitMinimap()
        {
            Console.WriteLine("Fit minimap");
        }

        private void ToggleGrid()
        {
            Console.WriteLine("Toggle grid minimap");
        }

        private void ToggleCollisions()
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

        private void UpdateRatio(double ratio)
        {
            Console.WriteLine("Updating Ratio: " + ratio);
        }

        #endregion methods
    }
}
