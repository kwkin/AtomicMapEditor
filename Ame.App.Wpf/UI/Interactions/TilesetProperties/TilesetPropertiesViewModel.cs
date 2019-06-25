using Ame.Components.Behaviors;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Emgu.CV;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ame.App.Wpf.UI.Interactions.TilesetProperties
{
    public class TilesetPropertiesViewModel : IInteractionRequestAware
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;
        private AmeSession session;

        private int tilesetPixelWidth;
        private int tilesetPixelHeight;
        private bool isMouseDown;
        private CoordinateTransform itemTransform;
        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private Stopwatch updatePositionLabelStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup tilesetImage;
        private DrawingGroup gridLines;
        private DrawingGroup extendedBorder;

        #endregion fields


        #region constructor

        public TilesetPropertiesViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.scrollModel = ScrollModel.DefaultScrollModel();
            this.session = session;

            this.WindowTitle.Value = "New Map";
            this.drawingGroup = new DrawingGroup();
            this.tilesetImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.extendedBorder = new DrawingGroup();
            this.drawingGroup.Children.Add(this.extendedBorder);
            this.drawingGroup.Children.Add(this.tilesetImage);
            this.drawingGroup.Children.Add(this.gridLines);
            this.TileImage.Value = new DrawingImage(this.drawingGroup);
            this.ZoomLevels = this.scrollModel.ZoomLevels;
            this.ZoomIndex.Value = this.scrollModel.ZoomIndex;
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            this.IsTransparent.PropertyChanged += TransparencyChanged;
            this.TransparentColor.PropertyChanged += TransparencyChanged;
            this.ZoomIndex.PropertyChanged += UpdateZoomIndex;
            this.GridPen.PropertyChanged += UpdateGridPen;
            this.BackgroundBrush.PropertyChanged += UpdateBackground;
            this.BackgroundPen.PropertyChanged += UpdateBackground;
            this.SelectedMetadata.PropertyChanged += SelectedMetadataChanged;
            this.IsSelectingTransparency.PropertyChanged += IsSelectingTransparencyChanged;

            this.HandleLeftClickDownCommand = new DelegateCommand<object>((point) => HandleLeftClickDown((Point)point));
            this.HandleLeftClickUpCommand = new DelegateCommand<object>((point) => HandleLeftClickUp((Point)point));
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((Point)point));
            this.SetTilesetCommand = new DelegateCommand(() => SetTileset());
            this.CloseWindowCommand = new DelegateCommand(() => CloseWindow());
            this.AddCustomMetaDataCommand = new DelegateCommand(() => AddCustomProperty());
            this.RemoveCustomMetadataCommand = new DelegateCommand(() => RemoveCustomProperty());
            this.MoveMetadataUpCommand = new DelegateCommand(() => MoveMetadataUp());
            this.MoveMetadataDownCommand = new DelegateCommand(() => MoveMetadataDown());
            this.BrowseSourceCommand = new DelegateCommand(() => BrowseSource());
            this.ShowGridCommand = new DelegateCommand(() => RedrawGrid());
            this.ShowRulerCommand = new DelegateCommand(() => RefreshRuler());
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => SetZoom(zoomLevel));
        }

        #endregion constructor


        #region properties

        public ICommand HandleLeftClickDownCommand { get; private set; }
        public ICommand HandleLeftClickUpCommand { get; private set; }
        public ICommand HandleMouseMoveCommand { get; private set; }
        public ICommand SetTilesetCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand AddCustomMetaDataCommand { get; private set; }
        public ICommand RemoveCustomMetadataCommand { get; private set; }
        public ICommand MoveMetadataUpCommand { get; private set; }
        public ICommand MoveMetadataDownCommand { get; private set; }
        public ICommand BrowseSourceCommand { get; set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand ShowRulerCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }

        private IConfirmation notification;
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as IConfirmation;
                this.TilesetModel.Value = this.notification.Content as TilesetModel;
                UpdateUI();
                RefreshItemImage();
                if (this.TilesetModel.Value != null)
                {
                    UpdateMetadata();
                    if (File.Exists(this.TilesetModel.Value.SourcePath.Value))
                    {
                        this.IsSourceLoaded.Value = true;
                    }
                }
            }
        }

        public Action FinishInteraction { get; set; }

        public BindableProperty<Mat> ItemImage { get; set; } = BindableProperty<Mat>.Prepare();

        public BindableProperty<TilesetModel> TilesetModel { get; set; } = BindableProperty<TilesetModel>.Prepare();

        public BindableProperty<DrawingImage> TileImage { get; set; } = BindableProperty<DrawingImage>.Prepare();

        public BindableProperty<string> WindowTitle { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<string> Name { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<string> SourcePath { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<bool> IsTransparent { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<Color> TransparentColor { get; set; } = BindableProperty<Color>.Prepare();

        public BindableProperty<int> TileWidth { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> TileHeight { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> OffsetX { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> OffsetY { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> PaddingX { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> PaddingY { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<ICollectionView> TilesetMetadata { get; set; } = BindableProperty<ICollectionView>.Prepare();

        public ObservableCollection<MetadataProperty> MetadataList { get; set; }

        public BindableProperty<MetadataProperty> SelectedMetadata { get; set; } = BindableProperty<MetadataProperty>.Prepare();

        public BindableProperty<bool> IsCustomSelected { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<bool> IsGridOn { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty<ScaleType>.Prepare();

        public BindableProperty<string> PositionText { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<int> ZoomIndex { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<Pen> GridPen { get; set; } = BindableProperty<Pen>.Prepare();

        public BindableProperty<Brush> BackgroundBrush { get; set; } = BindableProperty<Brush>.Prepare();

        public BindableProperty<Pen> BackgroundPen { get; set; } = BindableProperty<Pen>.Prepare();

        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }

        public BindableProperty<bool> IsSelectingTransparency { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<bool> IsSourceLoaded { get; set; } = BindableProperty<bool>.Prepare();

        #endregion properties


        #region methods

        private void SetTileset()
        {
            if (this.ItemImage == null)
            {
                return;
            }
            if (!File.Exists(this.SourcePath.Value))
            {
                MessageBox.Show("Error. Source Path is invalid.", "Error");
                return;
            }
            UpdateTilesetModel();
            if (this.notification != null)
            {
                this.notification.Confirmed = true;
            }
            FinishInteraction();
        }

        private void CloseWindow()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = false;
            }
            FinishInteraction();
        }

        private void UpdateUI()
        {
            TilesetModel model = this.TilesetModel.Value;
            this.Name.Value = model.Name.Value;
            this.SourcePath.Value = model.SourcePath.Value;
            this.TileWidth.Value = model.TileWidth.Value;
            this.TileHeight.Value = model.TileHeight.Value;
            this.OffsetX.Value = model.OffsetX.Value;
            this.OffsetY.Value = model.OffsetY.Value;
            this.PaddingX.Value = model.PaddingX.Value;
            this.PaddingY.Value = model.PaddingY.Value;
            this.IsTransparent.Value = model.IsTransparent.Value;
            this.TransparentColor.Value = model.TransparentColor.Value;
            this.tilesetPixelWidth = model.PixelWidth;
            this.tilesetPixelHeight = model.PixelHeight;
        }

        private void UpdateTilesetModel()
        {
            TilesetModel model = this.TilesetModel.Value;
            model.Name.Value = this.Name.Value;
            model.SourcePath.Value = this.SourcePath.Value;
            model.IsTransparent.Value = this.IsTransparent.Value;
            model.TransparentColor.Value = this.TransparentColor.Value;
            model.TileWidth.Value = this.TileWidth.Value;
            model.TileHeight.Value = this.TileHeight.Value;
            model.OffsetX.Value = this.OffsetX.Value;
            model.OffsetY.Value = this.OffsetY.Value;
            model.PaddingX.Value = this.PaddingX.Value;
            model.PaddingY.Value = this.PaddingY.Value;
            model.Columns.Value = this.tilesetPixelWidth / this.TileWidth.Value;
            model.Rows.Value = this.tilesetPixelHeight / this.TileHeight.Value;
        }

        public void HandleLeftClickUp(Point selectPoint)
        {
            this.isMouseDown = false;
            if (this.ItemImage.Value == null)
            {
                return;
            }
            bool wasSelectingTransparency = this.IsSelectingTransparency.Value;
            this.IsSelectingTransparency.Value = false;
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            if (wasSelectingTransparency)
            {
                SelectTransparency(pixelPoint);
            }
        }

        public void HandleLeftClickDown(Point selectPoint)
        {
            this.isMouseDown = true;
            if (this.ItemImage.Value == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            selectPoint = selectToPixel.Transform(selectPoint);
        }

        public void HandleMouseMove(Point selectPoint)
        {
            if (this.ItemImage.Value == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(pixelPoint);
                if (this.IsSelectingTransparency.Value && this.isMouseDown)
                {
                    this.TransparentColor.Value = ImageUtils.ColorAt(this.ItemImage.Value, pixelPoint);
                }
            }
        }

        public void SelectTransparency(Point pixelPoint)
        {
            if (!ImageUtils.Intersects(this.ItemImage.Value, pixelPoint))
            {
                return;
            }
            this.TransparentColor.Value = ImageUtils.ColorAt(this.ItemImage.Value, pixelPoint);
            if (this.IsTransparent.Value)
            {
                Mat transparentImage = ImageUtils.ColorToTransparent(this.ItemImage.Value, this.TransparentColor.Value);
                using (DrawingContext context = this.tilesetImage.Open())
                {
                    context.DrawDrawing(ImageUtils.MatToImageDrawing(transparentImage));
                }
            }
        }

        private void BrowseSource()
        {
            OpenFileDialog openTilesetDilog = new OpenFileDialog();
            openTilesetDilog.Title = "Select a Tileset";
            openTilesetDilog.InitialDirectory = this.session.LastTilesetDirectory;
            openTilesetDilog.Filter = ImageExtension.GetOpenFileImageExtensions();
            if (openTilesetDilog.ShowDialog() == true)
            {
                string tileFilePath = openTilesetDilog.FileName;
                if (File.Exists(tileFilePath))
                {
                    this.IsSourceLoaded.Value = true;
                    this.SourcePath.Value = tileFilePath;
                    this.session.LastTilesetDirectory = Directory.GetParent(tileFilePath).FullName;
                    RefreshItemImage();
                }
            }
        }

        private void TransparencyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshItemImage();
        }

        public void RefreshItemImage()
        {
            if (!File.Exists(this.SourcePath.Value))
            {
                return;
            }
            this.ItemImage.Value = CvInvoke.Imread(this.SourcePath.Value, Emgu.CV.CvEnum.ImreadModes.Unchanged);
            this.tilesetPixelWidth = this.ItemImage.Value.Width;
            this.tilesetPixelHeight = this.ItemImage.Value.Height;

            DrawingGroup newGroup = ImageUtils.MatToDrawingGroup(this.ItemImage.Value);
            this.TilesetModel.Value.TilesetImage = newGroup;
            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.Value.TileWidth.Value, this.TilesetModel.Value.TileHeight.Value);
            this.itemTransform.SetSelectionToPixel(this.TilesetModel.Value.TileWidth.Value / 2, this.TilesetModel.Value.TileHeight.Value / 2);
            Mat drawingMat = this.ItemImage.Value;

            this.IsGridOn.Value = true;
            if (this.IsTransparent.Value)
            {
                drawingMat = ImageUtils.ColorToTransparent(this.ItemImage.Value, this.TransparentColor.Value);
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (DrawingContext context = this.tilesetImage.Open())
                {
                    context.DrawDrawing(ImageUtils.MatToImageDrawing(drawingMat));
                }
            }), DispatcherPriority.Render);
            RedrawBackground();
            RedrawGrid();
        }

        public void RedrawGrid()
        {
            if (this.IsGridOn.Value)
            {
                int columns = this.ItemImage.Value.Width / this.TileWidth.Value;
                int rows = this.ItemImage.Value.Height / this.TileHeight.Value;

                PaddedGridRenderable gridParameters = new PaddedGridRenderable(columns, rows, this.TileWidth.Value, this.TileHeight.Value, this.OffsetX.Value, this.OffsetY.Value, this.PaddingX.Value, this.PaddingY.Value);
                double thickness = 1 / this.ZoomLevels[this.ZoomIndex.Value].zoom;
                gridParameters.DrawingPen.Thickness = thickness < Global.maxGridThickness ? thickness : Global.maxGridThickness;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    DrawingGroup group = gridParameters.CreateGrid();
                    this.gridLines.Children = group.Children;
                }), DispatcherPriority.Render);
            }
            else
            {
                this.gridLines.Children.Clear();
            }
        }

        public void RefreshRuler()
        {
            Console.WriteLine("Draw Ruler");
        }

        public void RedrawBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.ItemImage.Value.Width + this.TileWidth.Value;
            extendedSize.Height = this.ItemImage.Value.Height + this.TileHeight.Value;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.TileWidth.Value / 2;
            extendedPoint.Y = -this.TileHeight.Value / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            Size backgroundSize = new Size();
            backgroundSize.Width = this.ItemImage.Value.Width;
            backgroundSize.Height = this.ItemImage.Value.Height;
            Point backgroundPoint = new Point();
            backgroundPoint.X = 0;
            backgroundPoint.Y = 0;
            Rect backgroundRectangle = new Rect(backgroundPoint, backgroundSize);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (DrawingContext context = this.extendedBorder.Open())
                {
                    context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                    context.DrawRectangle(this.BackgroundBrush.Value, this.BackgroundPen.Value, backgroundRectangle);
                }
            }), DispatcherPriority.Render);
        }

        public void ZoomIn()
        {
            this.ZoomIndex.Value = this.scrollModel.ZoomIn();
        }

        public void ZoomOut()
        {
            this.ZoomIndex.Value = this.scrollModel.ZoomOut();
        }

        public void SetZoom(int zoomIndex)
        {
            this.ZoomIndex.Value = this.scrollModel.SetZoom(zoomIndex);
        }

        public void SetZoom(ZoomLevel zoomLevel)
        {
            this.ZoomIndex.Value = this.scrollModel.SetZoom(zoomLevel);
        }

        private void UpdateZoomIndex(object sender, PropertyChangedEventArgs e)
        {
            this.gridLines.Children.Clear();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                RedrawGrid();
            }),
            DispatcherPriority.Background);
        }

        private void UpdateBackground(object sender, PropertyChangedEventArgs e)
        {
            RedrawBackground();
        }

        private void UpdateGridPen(object sender, PropertyChangedEventArgs e)
        {
            RedrawGrid();
        }

        private void IsSelectingTransparencyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsSelectingTransparency.Value)
            {
                Mouse.OverrideCursor = Cursors.Pen;
            }
            else
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void SelectedMetadataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.SelectedMetadata.Value != null)
            {
                this.IsCustomSelected.Value = this.SelectedMetadata.Value.Type == MetadataType.Custom ? true : false;
            }
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.TileImage.Value != null)
            {
                switch (this.Scale.Value)
                {
                    case ScaleType.Pixel:
                        transformedPosition = position;
                        break;

                    case ScaleType.Tile:
                        GeneralTransform transform = GeometryUtils.CreateTransform(this.itemTransform.pixelToTile);
                        transformedPosition = transform.Transform(position);
                        break;
                }
            }
            transformedPosition = GeometryUtils.CreateIntPoint(transformedPosition);
            this.PositionText.Value = (transformedPosition.X + ", " + transformedPosition.Y);
            updatePositionLabelStopWatch.Restart();
        }

        private void UpdateMetadata()
        {
            // TODO add custom properties
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.TilesetModel.Value);
            this.TilesetMetadata.Value = new ListCollectionView(this.MetadataList);
            this.TilesetMetadata.Value.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            foreach(MetadataProperty property in this.TilesetModel.Value.CustomProperties)
            {
                this.MetadataList.Add(property);
            }
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            string customName = string.Format("Custom #{0}", customCount);
            MetadataProperty property = new MetadataProperty(customName, "", MetadataType.Custom);
            this.TilesetModel.Value.CustomProperties.Add(property);
            this.MetadataList.Add(property);
        }

        private void RemoveCustomProperty()
        {
            if (this.SelectedMetadata.Value.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(this.SelectedMetadata.Value);
            }
        }

        private void MoveMetadataUp()
        {
            int currentIndex = this.TilesetMetadata.Value.CurrentPosition;
            MetadataProperty currentItem = this.TilesetMetadata.Value.CurrentItem as MetadataProperty;
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int lowestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    lowestIndex = propertyIndex;
                    break;

                case MetadataType.Statistic:
                    lowestIndex = statisticIndex;
                    break;

                case MetadataType.Custom:
                    lowestIndex = customIndex;
                    break;

                default:
                    break;
            }
            if (currentIndex > lowestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex - 1);
                this.TilesetMetadata.Value.Refresh();
            }
        }

        private void MoveMetadataDown()
        {
            int currentIndex = this.TilesetMetadata.Value.CurrentPosition;
            MetadataProperty currentItem = this.TilesetMetadata.Value.CurrentItem as MetadataProperty;
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int highestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    highestIndex = statisticIndex - 1;
                    break;

                case MetadataType.Statistic:
                    highestIndex = customIndex - 1;
                    break;

                case MetadataType.Custom:
                    highestIndex = this.MetadataList.Count - 1;
                    break;

                default:
                    break;
            }
            if (currentIndex < highestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex + 1);
                this.TilesetMetadata.Value.Refresh();
            }
        }

        #endregion methods
    }
}
