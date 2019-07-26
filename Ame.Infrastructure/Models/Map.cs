using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Models.Serializer.Json;
using Ame.Infrastructure.Models.Serializer.Json.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ame.Infrastructure.Models
{
    // TODO implement an observable stack and queue
    public class Map : GridModel, IContainsMetadata
    {
        #region fields

        private DrawAction undoHover;

        #endregion fields


        #region constructor

        public Map()
            : this(null, string.Empty, 32, 32)
        {
        }

        public Map(string name)
            : this(null, name, 32, 32)
        {
        }

        public Map(Project project, string name)
            : this(project, name, 32, 32)
        {
        }

        public Map(string name, int columns, int rows)
            : this(null, name, columns, rows)
        {
        }

        public Map(Project project, string name, int columns, int rows)
            : this(project, name, columns, rows, new List<ILayer>(), new List<TilesetModel>())
        {
        }

        public Map(string name, int columns, int rows, IList<ILayer> layers, IList<TilesetModel> tilesets)
            : this(null, name, columns, rows, layers, tilesets)
        {

        }

        public Map(Project project, string name, int columns, int rows, IList<ILayer> layers, IList<TilesetModel> tilesets)
            : base(columns, rows, 32, 32)
        {
            this.Project.Value = project;
            this.Name.Value = name;

            this.Author.Value = "";
            this.Version.Value = new Constants().Version;
            this.Scale.Value = ScaleType.Tile;
            this.PixelScale.Value = 1;
            this.Description.Value = "";
            this.Layers = new ObservableCollection<ILayer>(layers);
            this.Tilesets = new ObservableCollection<TilesetModel>(tilesets);
            this.CustomProperties = new ObservableCollection<MetadataProperty>();
            
            this.UndoQueue = new ObservableStack<DrawAction>();
            this.RedoQueue = new ObservableStack<DrawAction>();

            InitializeLayers();

            UpdatePixelWidth();
            UpdatePixelHeight();
            UpdateIsModified();
            UpdateIsStored();

            this.SourcePath.PropertyChanged += SourcePathChanged;
            this.Layers.CollectionChanged += LayersChanged;
            this.UndoQueue.CollectionChanged += UndoQueueChanged;
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property, "Name")]
        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Source Path")]
        public BindableProperty<string> SourcePath { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Pixel Scale")]
        public BindableProperty<int> PixelScale { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Description")]
        public BindableProperty<string> Description { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Author")]
        public BindableProperty<string> Author { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Version")]
        public BindableProperty<string> Version { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        public ObservableCollection<ILayer> Layers { get; set; }

        public BindableProperty<int> SelectedLayerIndex { get; set; } = BindableProperty.Prepare<int>();

        public BindableProperty<Project> Project { get; set; } = BindableProperty.Prepare<Project>();

        private BindableProperty<bool> isModified = BindableProperty.Prepare<bool>();
        private ReadOnlyBindableProperty<bool> isModifiedReadOnly;
        public ReadOnlyBindableProperty<bool> IsModified
        {
            get
            {
                this.isModifiedReadOnly = this.isModifiedReadOnly ?? this.isModified.ReadOnlyProperty();
                return this.isModifiedReadOnly;
            }
        }

        private BindableProperty<bool> isStored = BindableProperty.Prepare<bool>();
        private ReadOnlyBindableProperty<bool> isStoredReadOnly;
        public ReadOnlyBindableProperty<bool> IsStored
        {
            get
            {
                this.isStoredReadOnly = this.isStoredReadOnly ?? this.isStored.ReadOnlyProperty();
                return this.isStoredReadOnly;
            }
        }

        public ILayer CurrentLayer
        {
            get
            {
                return this.Layers[this.SelectedLayerIndex.Value] as ILayer;
            }
        }

        public int LayerCount
        {
            get
            {
                return this.Layers.Count;
            }
        }

        public ObservableCollection<TilesetModel> Tilesets { get; set; }

        public int TilesetCount
        {
            get
            {
                return this.Tilesets.Count;
            }
        }

        public ObservableStack<DrawAction> UndoQueue { get; set; }

        public ObservableStack<DrawAction> RedoQueue { get; set; }

        public BindableProperty<Color> BackgroundColor { get; set; } = BindableProperty.Prepare<Color>((Color)ColorConverter.ConvertFromString("#b8e5ed"));

        public ObservableCollection<MetadataProperty> CustomProperties { get; set; }

        #endregion properties


        #region methods

        public void MergeCurrentLayerDown()
        {
            Console.WriteLine("Merge Current Layer Down");
        }

        public void MergeCurrentLayerUp()
        {
            Console.WriteLine("Merge Current Layer Up");
        }

        public void MergeVisibleLayers()
        {
            Console.WriteLine("Merge Visible Layers");
        }

        public void DeleteCurrentLayer()
        {
            this.Layers.RemoveAt(this.SelectedLayerIndex.Value);
        }

        public void NewLayerGroup()
        {
            int layerGroupCount = GetLayerGroupCount();
            string newLayerGroupName = string.Format("Layer Group #{0}", layerGroupCount);
            ILayer newLayerGroup = new LayerGroup(newLayerGroupName);
            this.Layers.Add(newLayerGroup);
        }

        public void DuplicateCurrentLayer()
        {
            ILayer copiedLayer = Utils.Utils.DeepClone<ILayer>(this.CurrentLayer);
            this.Layers.Add(copiedLayer);
        }

        public void Draw(DrawAction action)
        {
            if (this.undoHover != null)
            {
                ApplyAction(this.undoHover);
            }
            DrawAction undoAction = ApplyAction(action);
            this.UndoQueue.Push(undoAction);
            this.RedoQueue.Clear();
            this.undoHover = null;
        }

        /// <summary>
        /// Draws the requestded action and clears the previously drawn sample
        /// </summary>
        /// <param name="action"></param>
        public DrawAction DrawSample(DrawAction action)
        {
            if (this.undoHover != null)
            {
                ApplyAction(this.undoHover);
            }
            this.undoHover = ApplyAction(action);
            return this.undoHover;
        }

        public void Undo()
        {
            if (this.UndoQueue.Count == 0)
            {
                return;
            }
            DrawAction undoAction = this.UndoQueue.Pop();
            DrawAction redoAction = ApplyAction(undoAction);
            this.RedoQueue.Push(redoAction);
        }

        public void Redo()
        {
            if (this.RedoQueue.Count == 0)
            {
                return;
            }
            DrawAction redoAction = this.RedoQueue.Pop();
            DrawAction undoAction = ApplyAction(redoAction);
            this.UndoQueue.Push(undoAction);
        }

        private DrawAction ApplyAction(DrawAction action)
        {
            Stack<Tile> previousTiles = new Stack<Tile>();
            foreach (Tile tile in action.Tiles)
            {
                Tile previousTile = Draw(tile);
                if (previousTile != null)
                {
                    previousTiles.Push(previousTile);
                }
            }
            DrawAction revertAction = new DrawAction(action.Name, previousTiles);
            return revertAction;
        }

        private Tile Draw(Tile tile)
        {
            if (tile.Bounds.X < 0
                || tile.Bounds.Y < 0
                || tile.Bounds.X >= this.PixelWidth.Value
                || tile.Bounds.Y >= this.PixelHeight.Value)
            {
                return null;
            }
            if (!typeof(Layer).IsAssignableFrom(this.CurrentLayer.GetType()))
            {
                return null;
            }
            Layer currentLayer = this.CurrentLayer as Layer;
            int previousTileIndex = (int)(tile.Bounds.X / this.TileWidth.Value) + (int)(tile.Bounds.Y / this.TileHeight.Value) * this.Columns.Value;
            ImageDrawing previousImage = currentLayer.LayerItems[previousTileIndex] as ImageDrawing;
            Tile previousTileID = currentLayer.TileIDs[previousTileIndex];

            currentLayer.TileIDs[previousTileIndex] = tile;
            previousImage.Rect = tile.Bounds;
            Tile previousTile = new Tile(previousImage, previousTileID.TilesetID, previousTileID.TileID);
            return previousTile;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        /// <exception cref="FileFormatException">Thrown when there is an invalid parameter in the map</exception>
        public static bool IsValid(Map map)
        {
            StringBuilder errorMessage = new StringBuilder("");
            if (map.Rows.Value < 1)
            {
                errorMessage.AppendLine("Rows must be be at least 1.");
            }
            else if (map.Columns.Value < 1)
            {
                errorMessage.AppendLine("Columns must be be at least 1.");
            }
            else if (map.TileWidth.Value < 1)
            {
                errorMessage.AppendLine("Tile Width must be be at least 1.");
            }
            else if (map.TileHeight.Value < 1)
            {
                errorMessage.AppendLine("Tile Height must be be at least 1.");
            }
            if (errorMessage.ToString() != string.Empty)
            {
                throw new FileFormatException(errorMessage.ToString());
            }
            return true;
        }

        public void WriteFile(string file)
        {
            this.SourcePath.Value = file;

            MapJsonWriter writer = new MapJsonWriter();
            writer.Write(this, file);
            if (this.Project.Value != null)
            {
                this.Project.Value.UpdateFile();
            }
        }

        public void UpdateFile()
        {
            if (this.SourcePath != null)
            {
                WriteFile(this.SourcePath.Value);
            }
        }

        public void ExportAs(string path, BitmapEncoder encoder)
        {
            if (encoder == null)
            {
                return;
            }
            int width = GetPixelWidth();
            int height = GetPixelHeight();
            var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingGroup mapBackground = new DrawingGroup();
            DrawingGroup layerItems = new DrawingGroup();
            drawingGroup.Children.Add(mapBackground);
            drawingGroup.Children.Add(layerItems);

            Point backgroundLocation = new Point(0, 0);
            Size backgroundSize = new Size(width, height);
            Rect backgroundRect = new Rect(backgroundLocation, backgroundSize);
            using (DrawingContext context = mapBackground.Open())
            {
                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0), backgroundRect);
            }

            foreach (ILayer layer in this.Layers)
            {
                layerItems.Children.Add(layer.Group);
            }
            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            var image = new System.Windows.Controls.Image
            {
                Source = drawingImage
            };
            image.Arrange(new Rect(0, 0, width, height));
            bitmap.Render(image);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }

        public int GetLayerCount()
        {
            int totalLayers = 0;
            IEnumerable<ILayer> layers = this.Layers.Where(layer => typeof(Layer).IsInstanceOfType(layer));
            totalLayers += layers.Count();

            IEnumerable<ILayer> groups = this.Layers.Where(layer => typeof(LayerGroup).IsInstanceOfType(layer));
            foreach (LayerGroup group in groups)
            {
                totalLayers += group.GetLayerCount();
            }
            return totalLayers;
        }

        public int GetLayerGroupCount()
        {
            int totalGroups = 0;
            ;

            IEnumerable<ILayer> groups = this.Layers.Where(layer => typeof(LayerGroup).IsInstanceOfType(layer));
            totalGroups += groups.Count();
            foreach (LayerGroup group in groups)
            {
                totalGroups += group.GetLayerCount();
            }
            return totalGroups;
        }

        public override int GetPixelWidth()
        {
            int leftmost = GetOffsetX();
            int rightmost = leftmost;
            if (this.Layers != null)
            {
                foreach (ILayer layer in this.Layers)
                {
                    int width = layer.PixelWidth.Value;
                    rightmost = Math.Max(rightmost, width + layer.OffsetX.Value);
                }
            }
            int pixelWidth = rightmost - leftmost;
            return pixelWidth;
        }

        public override int GetPixelHeight()
        {
            int topmost = GetOffsetY();
            int bottommost = topmost;
            if (this.Layers != null)
            {
                foreach (ILayer layer in this.Layers)
                {
                    int height = layer.PixelHeight.Value;
                    bottommost = Math.Max(bottommost, height + layer.OffsetY.Value);
                }
            }
            int pixelHeight = bottommost - topmost;
            return pixelHeight;
        }

        private void InitializeLayers()
        {
            if (this.Layers.Count == 0)
            {
                Layer initialLayer = new Layer(this, "Layer #0", this.TileWidth.Value, this.TileHeight.Value, this.Rows.Value, this.Columns.Value);
                this.Layers.Add(initialLayer);
            }
            else
            {
                foreach (ILayer layer in this.Layers)
                {
                    layer.Map = this;
                }
            }
        }

        private void SourcePathChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateIsStored();
        }

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                default:
                    UpdatePixelWidth();
                    UpdatePixelHeight();
                    break;
            }
        }

        private void UndoQueueChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                default:
                    UpdateIsModified();
                    break;
            }
        }

        private int GetOffsetX()
        {
            int offsetX = 0;
            if (this.Layers != null)
            {
                if (this.Layers.Count > 0)
                {
                    offsetX = this.Layers[0].OffsetX.Value;
                    for (int index = 1; index < this.Layers.Count; ++index)
                    {
                        ILayer layer = this.Layers[index];
                        offsetX = Math.Min(offsetX, layer.OffsetX.Value);
                    }
                }
            }
            return offsetX;
        }

        private int GetOffsetY()
        {
            int offsetY = 0;
            if (this.Layers != null)
            {
                if (this.Layers.Count > 0)
                {
                    offsetY = this.Layers[0].OffsetY.Value;
                    for (int index = 1; index < this.Layers.Count; ++index)
                    {
                        ILayer layer = this.Layers[index];
                        offsetY = Math.Min(offsetY, layer.OffsetY.Value);
                    }
                }
            }
            return offsetY;
        }

        private void UpdateIsModified()
        {
            this.isModified.Value = this.UndoQueue.Count != 0;
        }

        private void UpdateIsStored()
        {
            this.isStored.Value = this.SourcePath.Value != null;
        }

        #endregion methods
    }
}
