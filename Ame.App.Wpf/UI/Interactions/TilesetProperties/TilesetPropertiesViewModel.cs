using Ame.Components.Behaviors;
using Ame.Infrastructure.Attributes;
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
    public class TilesetPropertiesViewModel : BindableBase, IInteractionRequestAware
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
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.eventAggregator = eventAggregator;
            this.scrollModel = ScrollModel.DefaultScrollModel();
            this.session = session;

            this.WindowTitle = "New Map";
            this.drawingGroup = new DrawingGroup();
            this.tilesetImage = new DrawingGroup();
            this.gridLines = new DrawingGroup();
            this.extendedBorder = new DrawingGroup();
            this.drawingGroup.Children.Add(this.extendedBorder);
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
            this.ShowGridCommand = new DelegateCommand(() =>
            {
                RedrawGrid();
            });
            this.ShowRulerCommand = new DelegateCommand(() =>
            {
                RefreshRuler();
            });
            this.ZoomInCommand = new DelegateCommand(() =>
            {
                ZoomIn();
            });
            this.ZoomOutCommand = new DelegateCommand(() =>
            {
                ZoomOut();
            });
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) =>
            {
                SetZoom(zoomLevel);
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
                this.TilesetModel = this.notification.Content as TilesetModel;
                UpdateUI();
                RefreshItemImage();
                if (this.TilesetModel != null)
                {
                    UpdateMetadata();
                    if (File.Exists(this.TilesetModel.SourcePath.Value))
                    {
                        this.IsSourceLoaded = true;
                    }
                }
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        // TODO remove bindable base and use bindable property
        // TODO change pixel steppers to labels and use columns/rows to set the size
        public Action FinishInteraction { get; set; }

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

        private TilesetModel tilesetModel;
        public TilesetModel TilesetModel
        {
            get
            {
                return this.tilesetModel;
            }
            set
            {
                SetProperty(ref this.tilesetModel, value);
            }
        }

        private DrawingImage tileImage;
        public DrawingImage TileImage
        {
            get
            {
                return this.tileImage;
            }
            set
            {
                SetProperty(ref this.tileImage, value);
            }
        }

        private string windowTitle;
        public string WindowTitle
        {
            get
            {
                return this.windowTitle;
            }
            set
            {
                SetProperty(ref this.windowTitle, value);
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                SetProperty(ref this.name, value);
            }
        }

        private string sourcePath;
        public string SourcePath
        {
            get
            {
                return this.sourcePath;
            }
            set
            {
                SetProperty(ref this.sourcePath, value);
            }
        }

        private bool isTransparent;
        public bool IsTransparent
        {
            get
            {
                return this.isTransparent;
            }
            set
            {
                if (SetProperty(ref this.isTransparent, value))
                {
                    RefreshItemImage();
                }
            }
        }

        private Color transparentColor;
        public Color TransparentColor
        {
            get
            {
                return this.transparentColor;
            }
            set
            {
                if (SetProperty(ref this.transparentColor, value))
                {
                    RefreshItemImage();
                }
            }
        }

        private int tileWidth;
        public int TileWidth
        {
            get
            {
                return this.tileWidth;
            }
            set
            {
                SetProperty(ref this.tileWidth, value);
            }
        }

        private int tileHeight;
        public int TileHeight
        {
            get
            {
                return this.tileHeight;
            }
            set
            {
                SetProperty(ref this.tileHeight, value);
            }
        }

        private int offsetX;
        public int OffsetX
        {
            get
            {
                return this.offsetX;
            }
            set
            {
                SetProperty(ref this.offsetX, value);
            }
        }

        private int offsetY;
        public int OffsetY
        {
            get
            {
                return this.offsetY;
            }
            set
            {
                SetProperty(ref this.offsetY, value);
            }
        }

        private int paddingX;
        public int PaddingX
        {
            get
            {
                return this.paddingX;
            }
            set
            {
                SetProperty(ref this.paddingX, value);
            }
        }

        private int paddingY;
        public int PaddingY
        {
            get
            {
                return this.paddingY;
            }
            set
            {
                SetProperty(ref this.paddingY, value);
            }
        }

        private ICollectionView groupedProperties;
        public ICollectionView GroupedProperties
        {
            get
            {
                return this.groupedProperties;
            }
            set
            {
                SetProperty(ref this.groupedProperties, value);
            }
        }

        private ICollectionView tilesetMetadata;
        public ICollectionView TilesetMetadata
        {
            get
            {
                return this.tilesetMetadata;
            }
            set
            {
                SetProperty(ref this.tilesetMetadata, value);
            }
        }

        private ObservableCollection<MetadataProperty> metadataList;
        public ObservableCollection<MetadataProperty> MetadataList
        {
            get
            {
                return this.metadataList;
            }
            set
            {
                SetProperty(ref this.metadataList, value);
            }
        }

        private MetadataProperty selectedMetadata;
        public MetadataProperty SelectedMetadata
        {
            get
            {
                return this.selectedMetadata;
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
                return this.isCustomSelected;
            }
            set
            {
                SetProperty(ref this.isCustomSelected, value);
            }
        }

        private bool isGridOn;
        public bool IsGridOn
        {
            get
            {
                return this.isGridOn;
            }
            set
            {
                SetProperty(ref this.isGridOn, value);
            }
        }

        private ScaleType scale;
        public ScaleType Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                SetProperty(ref this.scale, value);
            }
        }

        private string positionText;
        public string PositionText
        {
            get
            {
                return this.positionText;
            }
            set
            {
                SetProperty(ref this.positionText, value);
            }
        }

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
                        RedrawGrid();
                    }),
                    DispatcherPriority.Background);
                }
            }
        }

        private Pen gridPen;
        public Pen GridPen
        {
            get
            {
                return gridPen;
            }
            set
            {
                if (SetProperty(ref this.gridPen, value))
                {
                    RedrawGrid();
                }
            }
        }

        private Brush backgroundBrush;
        public Brush BackgroundBrush
        {
            get
            {
                return backgroundBrush;
            }
            set
            {
                if (SetProperty(ref this.backgroundBrush, value))
                {
                    RefreshBackground();
                }
            }
        }

        private Pen backgroundPen;
        public Pen BackgroundPen
        {
            get
            {
                return backgroundPen;
            }
            set
            {
                if (SetProperty(ref this.backgroundPen, value))
                {
                    RefreshBackground();
                }
            }
        }

        private ObservableCollection<ZoomLevel> zoomLevels;
        public ObservableCollection<ZoomLevel> ZoomLevels
        {
            get
            {
                return zoomLevels;
            }
            set
            {
                SetProperty(ref this.zoomLevels, value);
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

        private bool isSourceLoaded;
        public bool IsSourceLoaded
        {
            get
            {
                return this.isSourceLoaded;
            }
            set
            {
                SetProperty(ref this.isSourceLoaded, value);
            }
        }

        #endregion properties


        #region methods

        private void SetTileset()
        {
            if (this.ItemImage == null)
            {
                return;
            }
            if (!File.Exists(this.SourcePath))
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
            this.Name = this.TilesetModel.Name.Value;
            this.SourcePath = this.TilesetModel.SourcePath.Value;
            this.IsTransparent = this.TilesetModel.IsTransparent.Value;
            this.TransparentColor = this.TilesetModel.TransparentColor.Value;
            this.TileWidth = this.TilesetModel.TileWidth.Value;
            this.TileHeight = this.TilesetModel.TileHeight.Value;
            this.OffsetX = this.TilesetModel.OffsetX.Value;
            this.OffsetY = this.TilesetModel.OffsetY.Value;
            this.PaddingX = this.TilesetModel.PaddingX.Value;
            this.PaddingY = this.TilesetModel.PaddingY.Value;
            this.tilesetPixelWidth = this.TilesetModel.PixelWidth;
            this.tilesetPixelHeight = this.TilesetModel.PixelHeight;
        }

        private void UpdateTilesetModel()
        {
            this.TilesetModel.Name.Value = this.Name;
            this.TilesetModel.SourcePath.Value = this.SourcePath;
            this.TilesetModel.IsTransparent.Value = this.IsTransparent;
            this.TilesetModel.TransparentColor.Value = this.TransparentColor;
            this.TilesetModel.TileWidth.Value = this.TileWidth;
            this.TilesetModel.TileHeight.Value = this.TileHeight;
            this.TilesetModel.OffsetX.Value = this.OffsetX;
            this.TilesetModel.OffsetY.Value = this.OffsetY;
            this.TilesetModel.PaddingX.Value = this.PaddingX;
            this.TilesetModel.PaddingY.Value = this.PaddingY;
            this.TilesetModel.Columns.Value = this.tilesetPixelWidth / this.TileWidth;
            this.TilesetModel.Rows.Value = this.tilesetPixelHeight / this.TileHeight;
        }

        public void HandleLeftClickUp(Point selectPoint)
        {
            this.isMouseDown = false;
            if (this.ItemImage == null)
            {
                return;
            }
            bool wasSelectingTransparency = this.IsSelectingTransparency;
            this.IsSelectingTransparency = false;
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
            if (this.ItemImage == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            selectPoint = selectToPixel.Transform(selectPoint);
        }

        public void HandleMouseMove(Point selectPoint)
        {
            if (this.ItemImage == null)
            {
                return;
            }
            GeneralTransform selectToPixel = GeometryUtils.CreateTransform(this.itemTransform.pixelToSelect.Inverse);
            Point pixelPoint = selectToPixel.Transform(selectPoint);
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
            if (this.IsTransparent)
            {
                Mat transparentImage = ImageUtils.ColorToTransparent(this.ItemImage, this.TransparentColor);
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
                    this.IsSourceLoaded = true;
                    this.SourcePath = tileFilePath;
                    this.session.LastTilesetDirectory = Directory.GetParent(tileFilePath).FullName;
                    RefreshItemImage();
                    RaisePropertyChanged(nameof(this.SourcePath));
                }
            }
        }

        public void RefreshItemImage()
        {
            if (!File.Exists(this.SourcePath))
            {
                return;
            }
            this.ItemImage = CvInvoke.Imread(this.SourcePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
            this.tilesetPixelWidth = this.ItemImage.Width;
            this.tilesetPixelHeight = this.ItemImage.Height;

            DrawingGroup newGroup = ImageUtils.MatToDrawingGroup(this.ItemImage);
            this.TilesetModel.TilesetImage = newGroup;
            this.itemTransform = new CoordinateTransform();
            this.itemTransform.SetPixelToTile(this.TilesetModel.TileWidth.Value, this.TilesetModel.TileHeight.Value);
            this.itemTransform.SetSelectionToPixel(this.TilesetModel.TileWidth.Value / 2, this.TilesetModel.TileHeight.Value / 2);
            Mat drawingMat = this.ItemImage;

            this.isGridOn = true;
            if (this.IsTransparent)
            {
                drawingMat = ImageUtils.ColorToTransparent(this.ItemImage, this.TransparentColor);
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (DrawingContext context = this.tilesetImage.Open())
                {
                    context.DrawDrawing(ImageUtils.MatToImageDrawing(drawingMat));
                }
            }), DispatcherPriority.Render);
            RefreshBackground();
            RedrawGrid();
        }

        public void RedrawGrid()
        {
            if (this.IsGridOn)
            {
                int columns = this.ItemImage.Width / this.TileWidth;
                int rows = this.ItemImage.Height / this.TileHeight;

                PaddedGridRenderable gridParameters = new PaddedGridRenderable(columns, rows, this.TileWidth, this.TileHeight, this.OffsetX, this.OffsetY, this.PaddingX, this.PaddingY);
                double thickness = 1 / this.ZoomLevels[this.ZoomIndex].zoom;
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

        public void RefreshBackground()
        {
            Size extendedSize = new Size();
            extendedSize.Width = this.ItemImage.Width + this.TileWidth;
            extendedSize.Height = this.ItemImage.Height + this.TileHeight;
            Point extendedPoint = new Point();
            extendedPoint.X = -this.TileWidth / 2;
            extendedPoint.Y = -this.TileHeight / 2;
            Rect drawingRect = new Rect(extendedPoint, extendedSize);

            Size backgroundSize = new Size();
            backgroundSize.Width = this.ItemImage.Width;
            backgroundSize.Height = this.ItemImage.Height;
            Point backgroundPoint = new Point();
            backgroundPoint.X = 0;
            backgroundPoint.Y = 0;
            Rect backgroundRectangle = new Rect(backgroundPoint, backgroundSize);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (DrawingContext context = this.extendedBorder.Open())
                {
                    context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), drawingRect);
                    context.DrawRectangle(backgroundBrush, backgroundPen, backgroundRectangle);
                }
            }), DispatcherPriority.Render);
        }

        public void ZoomIn()
        {
            this.ZoomIndex = this.scrollModel.ZoomIn();
        }

        public void ZoomOut()
        {
            this.ZoomIndex = this.scrollModel.ZoomOut();
        }

        public void SetZoom(int zoomIndex)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomIndex);
        }

        public void SetZoom(ZoomLevel zoomLevel)
        {
            this.ZoomIndex = this.scrollModel.SetZoom(zoomLevel);
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

        #endregion methods
    }
}
