using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ame.Components.Behaviors;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Emgu.CV;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.Interactions.TilesetProperties
{
    public class TilesetPropertiesViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IScrollModel scrollModel;

        private int tilesetPixelWidth;
        private int tilesetPixelHeight;
        private bool isMouseDown;
        private CoordinateTransform itemTransform;
        private long updatePositionLabelDelay = Global.defaultUpdatePositionLabelDelay;
        private Stopwatch updatePositionLabelStopWatch;

        private DrawingGroup drawingGroup;
        private DrawingGroup tilesetImage;
        private DrawingGroup gridLines;

        #endregion fields


        #region constructor

        public TilesetPropertiesViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.eventAggregator = eventAggregator;
            this.scrollModel = ScrollModel.DefaultScrollModel();

            this.WindowTitle = "New Map";
            this.drawingGroup = new DrawingGroup();
            this.tilesetImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.drawingGroup.Children.Add(this.tilesetImage);
            this.drawingGroup.Children.Add(this.gridLines);
            this.TileImage = new DrawingImage(this.drawingGroup);
            this.ZoomLevels = this.scrollModel.ZoomLevels;
            this.ZoomIndex = this.scrollModel.ZoomIndex;
            this.updatePositionLabelStopWatch = Stopwatch.StartNew();

            this.HandleLeftClickDownCommand = new DelegateCommand<object>((point) =>
            {
                HandleLeftClickDown((Point)point);
            });
            this.HandleLeftClickUpCommand = new DelegateCommand<object>((point) =>
            {
                HandleLeftClickUp((Point)point);
            });
            this.HandleMouseMoveCommand = new DelegateCommand<object>((point) =>
            {
                HandleMouseMove((Point)point);
            });
            this.SetTilesetCommand = new DelegateCommand(() =>
            {
                SetTileset();
            });
            this.CloseWindowCommand = new DelegateCommand(() =>
            {
                CloseWindow();
            });
            this.AddCustomMetaDataCommand = new DelegateCommand(() =>
            {
                AddCustomProperty();
            });
            this.RemoveCustomMetadataCommand = new DelegateCommand(() =>
            {
                RemoveCustomProperty();
            });
            this.MoveMetadataUpCommand = new DelegateCommand(() =>
            {
                MoveMetadataUp();
            });
            this.MoveMetadataDownCommand = new DelegateCommand(() =>
            {
                MoveMetadataDown();
            });
            this.BrowseSourceCommand = new DelegateCommand(() =>
            {
                BrowseSource();
            });
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

        public string WindowTitle { get; set; }
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int PaddingX { get; set; }
        public int PaddingY { get; set; }

        public IConfirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as IConfirmation;
                this.TilesetModel = this.notification.Content as TilesetModel;
                UpdateUI();
                UpdateItemImage();
                if (this.TilesetModel != null)
                {
                    UpdateMetadata();
                }
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public TilesetModel TilesetModel { get; set; }
        public DrawingImage TileImage { get; set; }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView TilesetMetadata { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }

        public MetadataProperty selectedMetadata;
        public MetadataProperty SelectedMetadata
        {
            get
            {
                return selectedMetadata;
            }
            set
            {
                this.IsCustomSelected = value.Type == MetadataType.Custom ? true : false;
                SetProperty(ref this.selectedMetadata, value);
            }
        }

        public bool isCustomSelected;
        public bool IsCustomSelected
        {
            get
            {
                return isCustomSelected;
            }
            set
            {
                SetProperty(ref this.isCustomSelected, value);
            }
        }

        public bool IsGridOn { get; set; }
        public ScaleType Scale { get; set; }
        public string PositionText { get; set; }
        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }

        private int zoomIndex;
        public int ZoomIndex
        {
            get { return this.zoomIndex; }
            set
            {
                if (SetProperty(ref this.zoomIndex, value))
                {
                    this.gridLines.Children.Clear();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DrawGrid();
                    }),
                    DispatcherPriority.Background);
                }
            }
        }

        private bool isSelectingTransparency;
        public bool IsSelectingTransparency
        {
            get
            {
                return this.isSelectingTransparency;
            }
            set
            {
                if (SetProperty(ref this.isSelectingTransparency, value))
                {
                    if (this.isSelectingTransparency)
                    {
                        Mouse.OverrideCursor = Cursors.Pen;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                    }
                }
            }
        }

        private Mat itemImage;
        public Mat ItemImage
        {
            get
            {
                return this.itemImage;
            }
            set
            {
                SetProperty(ref this.itemImage, value);
            }
        }

        public Action FinishInteraction { get; set; }

        #endregion properties


        #region methods

        public void HandleLeftClickUp(Point selectPoint)
        {
            if (this.ItemImage == null)
            {
                return;
            }
            this.isMouseDown = false;
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
            if (this.IsSelectingTransparency)
            {
                SelectTransparency(pixelPoint);
                this.IsSelectingTransparency = false;
            }
        }

        public void HandleLeftClickDown(Point selectPoint)
        {
            if (this.ItemImage == null)
            {
                return;
            }
            this.isMouseDown = true;
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            selectPoint = selectToPixel.Transform(selectPoint);
            if (!ImageUtils.Intersects(this.ItemImage, selectPoint))
            {
                return;
            }
        }

        public void HandleMouseMove(Point selectPosition)
        {
            if (this.ItemImage == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPosition);
            if (this.updatePositionLabelStopWatch.ElapsedMilliseconds > this.updatePositionLabelDelay)
            {
                UpdatePositionLabel(pixelPoint);
                if (this.IsSelectingTransparency && this.isMouseDown)
                {
                    this.TransparentColor = ImageUtils.ColorAt(this.ItemImage, pixelPoint);
                }
            }
        }

        public void SelectTransparency(Point pixelPoint)
        {
            if (!ImageUtils.Intersects(this.ItemImage, pixelPoint))
            {
                return;
            }
            this.TransparentColor = ImageUtils.ColorAt(this.ItemImage, pixelPoint);
            Mat transparentImage = ImageUtils.ColorToTransparent(this.ItemImage, this.TransparentColor);
            using (DrawingContext context = this.tilesetImage.Open())
            {
                context.DrawDrawing(ImageUtils.MatToImageDrawing(transparentImage));
            }
            RaisePropertyChanged(nameof(this.TransparentColor));
        }

        private void SetTileset()
        {
            if (this.ItemImage == null)
            {
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
            this.Name = this.TilesetModel.Name;
            this.SourcePath = this.TilesetModel.SourcePath;
            this.IsTransparent = this.TilesetModel.IsTransparent;
            this.TransparentColor = this.TilesetModel.TransparentColor;
            this.TileWidth = this.TilesetModel.TileWidth;
            this.TileHeight = this.TilesetModel.TileHeight;
            this.OffsetX = this.TilesetModel.OffsetX;
            this.OffsetY = this.TilesetModel.OffsetY;
            this.PaddingX = this.TilesetModel.PaddingX;
            this.PaddingY = this.TilesetModel.PaddingY;
            this.tilesetPixelWidth = this.TilesetModel.PixelWidth;
            this.tilesetPixelHeight = this.TilesetModel.PixelHeight;
        }

        private void UpdateTilesetModel()
        {
            this.TilesetModel.Name = this.Name;
            this.TilesetModel.SourcePath = this.SourcePath;
            this.TilesetModel.IsTransparent = this.IsTransparent;
            this.TilesetModel.TransparentColor = this.TransparentColor;
            this.TilesetModel.TileWidth = this.TileWidth;
            this.TilesetModel.TileHeight = this.TileHeight;
            this.TilesetModel.OffsetX = this.OffsetX;
            this.TilesetModel.OffsetY = this.OffsetY;
            this.TilesetModel.PaddingX = this.PaddingX;
            this.TilesetModel.PaddingY = this.PaddingY;
            this.TilesetModel.PixelHeight = this.tilesetPixelHeight;
            this.TilesetModel.PixelWidth = this.tilesetPixelWidth;
        }

        private void UpdateMetadata()
        {
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.TilesetModel);
            this.TilesetMetadata = new ListCollectionView(this.MetadataList);
            this.TilesetMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            string customName = string.Format("Custom #{0}", customCount);
            this.MetadataList.Add(new MetadataProperty(customName, "", MetadataType.Custom));
        }

        private void RemoveCustomProperty()
        {
            if (this.SelectedMetadata.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(this.SelectedMetadata);
            }
        }

        private void MoveMetadataUp()
        {
            int currentIndex = this.TilesetMetadata.CurrentPosition;
            MetadataProperty currentItem = this.TilesetMetadata.CurrentItem as MetadataProperty;
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
                this.TilesetMetadata.Refresh();
            }
        }

        private void MoveMetadataDown()
        {
            int currentIndex = this.TilesetMetadata.CurrentPosition;
            MetadataProperty currentItem = this.TilesetMetadata.CurrentItem as MetadataProperty;
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
                this.TilesetMetadata.Refresh();
            }
        }

        private void BrowseSource()
        {
            OpenFileDialog openTilesetDilog = new OpenFileDialog();
            openTilesetDilog.Title = "Select a Tileset";
            openTilesetDilog.Filter = ImageExtension.GetOpenFileImageExtensions();
            if (openTilesetDilog.ShowDialog() == true)
            {
                string tileFilePath = openTilesetDilog.FileName;
                if (File.Exists(tileFilePath))
                {
                    this.SourcePath = tileFilePath;
                    UpdateItemImage();
                    RaisePropertyChanged(nameof(this.SourcePath));
                }
            }
        }

        public void UpdateItemImage()
        {
            if (string.IsNullOrEmpty(this.SourcePath))
            {
                return;
            }
            this.ItemImage = CvInvoke.Imread(this.SourcePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
            this.tilesetPixelWidth = this.ItemImage.Width;
            this.tilesetPixelHeight = this.ItemImage.Height;

            DrawingGroup newGroup = ImageUtils.MatToDrawingGroup(this.ItemImage);
            this.TilesetModel.TilesetImage = newGroup;
            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.TileWidth, this.TilesetModel.TileHeight);
            this.itemTransform.SetSlectionToPixel(this.TilesetModel.TileWidth / 2, this.TilesetModel.TileHeight / 2);

            using (DrawingContext context = this.tilesetImage.Open())
            {
                context.DrawDrawing(this.TilesetModel.TilesetImage);
            }
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
                PaddedGridRenderable gridParameters = new PaddedGridRenderable(this.TilesetModel);
                double thickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
                gridParameters.DrawingPen.Thickness = thickness < Global.maxGridThickness ? thickness : Global.maxGridThickness;
                DrawingGroup group = gridParameters.CreateGrid();
                this.gridLines.Children = group.Children;
            }
            else
            {
                this.gridLines.Children.Clear();
            }
            RaisePropertyChanged(nameof(this.IsGridOn));
            RaisePropertyChanged(nameof(this.TileImage));
        }

        private void UpdatePositionLabel(Point position)
        {
            Point transformedPosition = new Point(0, 0);
            if (this.TileImage != null)
            {
                switch (Scale)
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
            this.PositionText = (transformedPosition.X + ", " + transformedPosition.Y);
            RaisePropertyChanged(nameof(this.PositionText));
            updatePositionLabelStopWatch.Restart();
        }

        #endregion methods
    }
}
