using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Models.Serializer.Json.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    // TODO add additional interfaces, such as INameable
    public class Map : GridModel, IContainsCustomProperties
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
            : base(columns, rows, 32, 32)
        {
            this.Project.Value = project;
            this.Name.Value = name;

            this.Author.Value = "";
            this.Version.Value = Global.Version;
            this.Scale.Value = ScaleType.Tile;
            this.PixelScale.Value = 1;
            this.Description.Value = "";
            this.Layers = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();
            this.CustomProperties = new ObservableCollection<MetadataProperty>();

            Layer initialLayer = new Layer(this, "Layer #0", this.TileWidth.Value, this.TileHeight.Value, this.Rows.Value, this.Columns.Value);
            this.Layers.Add(initialLayer);

            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();

            this.Layers.CollectionChanged += LayersChanged;
            UpdatePixelWidth();
            UpdatePixelHeight();
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

        public Layer CurrentLayer
        {
            get
            {
                return this.Layers[this.SelectedLayerIndex.Value] as Layer;
            }
        }

        public int LayerCount
        {
            get
            {
                return this.Layers.Count;
            }
        }

        public ObservableCollection<TilesetModel> TilesetList { get; set; }

        public int TilesetCount
        {
            get
            {
                return this.TilesetList.Count;
            }
        }

        public Stack<DrawAction> UndoQueue { get; set; }

        public Stack<DrawAction> RedoQueue { get; set; }

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
                applyAction(this.undoHover);
            }
            DrawAction undoAction = applyAction(action);
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
                applyAction(this.undoHover);
            }
            this.undoHover = applyAction(action);
            return this.undoHover;
        }

        public void Undo()
        {
            if (this.UndoQueue.Count == 0)
            {
                return;
            }
            DrawAction undoAction = this.UndoQueue.Pop();
            DrawAction redoAction = applyAction(undoAction);
            this.RedoQueue.Push(redoAction);
        }

        public void Redo()
        {
            if (this.RedoQueue.Count == 0)
            {
                return;
            }
            DrawAction redoAction = this.RedoQueue.Pop();
            DrawAction undoAction = applyAction(redoAction);
            this.UndoQueue.Push(undoAction);
        }

        private DrawAction applyAction(DrawAction action)
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
            int previousTileIndex = (int)(tile.Bounds.X / this.TileWidth.Value) + (int)(tile.Bounds.Y / this.TileHeight.Value) * this.Columns.Value;
            ImageDrawing previousImage = this.CurrentLayer.LayerItems[previousTileIndex] as ImageDrawing;
            Tile previousTileID = this.CurrentLayer.TileIDs[previousTileIndex];

            this.CurrentLayer.TileIDs[previousTileIndex] = tile;
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
        public static bool isValid(Map map)
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

        public void SerializeFile(string file)
        {
            MapJson json = new MapJson(this);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter stream = new StreamWriter(file))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                serializer.Serialize(writer, json);
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

        #endregion methods
    }
}
