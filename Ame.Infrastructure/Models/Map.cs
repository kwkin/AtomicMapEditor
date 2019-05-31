using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Ame.Infrastructure.Serialization;

namespace Ame.Infrastructure.Models
{
    public class Map : INotifyPropertyChanged
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class MapJson : JsonAdapter<Map>
        {
            public MapJson()
            {
            }

            public MapJson(Map map)
            {
                this.Version = map.Version;
                this.Name = map.Name;
                this.Author = map.Author;
                this.Rows = map.Rows;
                this.Columns = map.Columns;
                this.TileWidth = map.TileWidth;
                this.TileHeight = map.TileHeight;
                this.Scale = map.Scale;
                this.BackgroundColor = map.BackgroundColor;
                this.Description = map.Description;
                this.TilesetList = new List<TilesetModel.TilesetJson>();
                foreach (TilesetModel model in map.TilesetList)
                {
                    this.TilesetList.Add(new TilesetModel.TilesetJson(model));
                }
                this.LayerList = new List<Layer.LayerJson>();
                foreach (ILayer layer in map.LayerList)
                {
                    // TODO fix the conversion
                    this.LayerList.Add(new Layer.LayerJson((Layer)layer));
                }
            }
            
            [JsonProperty(PropertyName = "Version")]
            public string Version { get; set; }

            [JsonProperty(PropertyName = "Name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "Author")]
            public string Author { get; set; }

            [JsonProperty(PropertyName = "Rows")]
            public int Rows { get; set; }

            [JsonProperty(PropertyName = "Columns")]
            public int Columns { get; set; }

            [JsonProperty(PropertyName = "TileWidth")]
            public int TileWidth { get; set; }

            [JsonProperty(PropertyName = "TileHeight")]
            public int TileHeight { get; set; }

            [JsonProperty(PropertyName = "Scale")]
            public ScaleType Scale { get; set; }

            [JsonProperty(PropertyName = "Color")]
            public Color BackgroundColor { get; set; }

            [JsonProperty(PropertyName = "Description")]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "Tilesets")]
            public IList<TilesetModel.TilesetJson> TilesetList { get; set; }

            [JsonProperty(PropertyName = "Layers")]
            public IList<Layer.LayerJson> LayerList { get; set; }
            
            public Map Generate()
            {
                Map map = new Map();
                map.Version = this.Version;
                map.Name = this.Name;
                map.Author = this.Author;
                map.Rows = this.Rows;
                map.Columns = this.Columns;
                map.TileWidth = this.TileWidth;
                map.TileHeight = this.TileHeight;
                map.Scale = this.Scale;
                map.BackgroundColor = this.BackgroundColor;
                map.Description = this.Description;
                map.TilesetList = new ObservableCollection<TilesetModel>();
                foreach (TilesetModel.TilesetJson tilesetJson in this.TilesetList)
                {
                    TilesetModel tileset = tilesetJson.Generate();
                    map.TilesetList.Add(tileset);
                    tileset.RefreshTilesetImage();
                }
                map.LayerList = new ObservableCollection<ILayer>();
                foreach (Layer.LayerJson layer in this.LayerList)
                {
                    map.LayerList.Add(layer.Generate(map.TilesetList));
                }
                return map;
            }
        }


        #region fields

        public event PropertyChangedEventHandler PropertyChanged;

        private string name;

        #endregion fields


        #region constructor

        public Map()
        {
            this.Author = "";
            this.Version = Global.Version;
            this.Name = "";
            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();
            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();
        }

        public Map(string name)
        {
            this.Name = name;

            this.Author = "";
            this.Version = Global.Version;
            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns);
            this.LayerList.Add(initialLayer);

            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();
            for (int xIndex = 0; xIndex < this.TileWidth; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.TileHeight; ++yIndex)
                {
                    Point position = new Point(xIndex * 32, yIndex * 32);
                    Tile emptyTile = Tile.emptyTile(position);
                    this.CurrentLayer.LayerItems.Add(emptyTile.Image);
                    this.CurrentLayer.TileIDs.Add(emptyTile);
                }
            }
        }

        public Map(string name, int width, int height)
        {
            this.Name = name;
            this.Grid = new GridModel(width, height, 32, 32);

            this.Author = "";
            this.Version = Global.Version;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns);
            this.LayerList.Add(initialLayer);

            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();
            for (int xIndex = 0; xIndex < this.TileWidth; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.TileHeight; ++yIndex)
                {
                    Point position = new Point(xIndex * 32, yIndex * 32);
                    Tile emptyTile = Tile.emptyTile(position);
                    this.CurrentLayer.LayerItems.Add(emptyTile.Image);
                    this.CurrentLayer.TileIDs.Add(emptyTile);
                }
            }
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [MetadataProperty(MetadataType.Property)]
        private string file;
        public string SourcePath
        {
            get
            {
                return this.file;
            }
            set
            {
                this.file = value;
            }
        }

        public GridModel Grid { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public int Columns
        {
            get
            {
                return this.Grid.Columns();
            }
            set
            {
                this.Grid.SetWidthWithColumns(value);
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public int Rows
        {
            get
            {
                return this.Grid.Rows();
            }
            set
            {
                this.Grid.SetHeightWithRows(value);
            }
        }

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public int TileWidth
        {
            get
            {
                return this.Grid.TileWidth;
            }
            set
            {
                this.Grid.TileWidth = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight
        {
            get
            {
                return this.Grid.TileHeight;
            }
            set
            {
                this.Grid.TileHeight = value;
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public ScaleType Scale
        {
            get
            {
                return this.Grid.Scale;
            }
            set
            {
                this.Grid.Scale = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Width")]
        public int PixelWidth
        {
            get
            {
                return this.Grid.PixelWidth;
            }
            set
            {
                this.Grid.PixelWidth = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Height")]
        public int PixelHeight
        {
            get
            {
                return this.Grid.PixelHeight;
            }
            set
            {
                this.Grid.PixelHeight = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Ratio")]
        public int PixelRatio { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Scale")]
        public int PixelScale { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public string Description { get; set; }

        // TODO move these two properties to another class
        [MetadataProperty(MetadataType.Property)]
        public string Author { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public string Version { get; set; }

        // TODO change to an observable list
        public ObservableCollection<ILayer> LayerList { get; set; }
        public int SelectedLayerIndex { get; set; }
        
        public Layer CurrentLayer
        {
            get
            {
                return this.LayerList[this.SelectedLayerIndex] as Layer;
            }
        }

        public int LayerCount
        {
            get
            {
                return this.LayerList.Count;
            }
        }

        public Size PixelSize
        {
            get
            {
                return new Size(this.PixelWidth, this.PixelHeight);
            }
            set
            {
                this.PixelWidth = (int)value.Width;
                this.PixelHeight = (int)value.Height;
            }
        }

        // TODO create a tilesetModelCollection and a LayerCollection
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
        
        public Color BackgroundColor { get; set; } = (Color)ColorConverter.ConvertFromString("#b8e5ed");

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
            this.LayerList.RemoveAt(this.SelectedLayerIndex);
        }

        public void NewLayerGroup()
        {
            int layerGroupCount = GetLayerGroupCount();
            string newLayerGroupName = string.Format("Layer Group #{0}", layerGroupCount);
            ILayer newLayerGroup = new LayerGroup(newLayerGroupName);
            this.LayerList.Add(newLayerGroup);
        }

        public void DuplicateCurrentLayer()
        {
            ILayer copiedLayer = Utils.Utils.DeepClone<ILayer>(this.CurrentLayer);
            this.LayerList.Add(copiedLayer);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int GetLayerGroupCount()
        {
            IEnumerable<LayerGroup> groups = this.LayerList.OfType<LayerGroup>();
            return groups.Count<LayerGroup>();
        }

        private int GetLayerCount()
        {
            IEnumerable<Layer> groups = this.LayerList.OfType<Layer>();
            return groups.Count<Layer>();
        }

        public void Draw(DrawAction action)
        {
            DrawAction undoAction = applyAction(action);
            this.UndoQueue.Push(undoAction);
            this.RedoQueue.Clear();
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
                || tile.Bounds.X >= this.PixelWidth 
                || tile.Bounds.Y >= this.PixelHeight)
            {
                return null;
            }
            int previousTileIndex = (int)(tile.Bounds.X / this.TileWidth) + (int)(tile.Bounds.Y / this.TileHeight) * this.Columns;
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
            if (map.Rows < 1)
            {
                errorMessage.AppendLine("Rows must be be at least 1.");
            }
            else if (map.Columns < 1)
            {
                errorMessage.AppendLine("Columns must be be at least 1.");
            }
            else if (map.TileWidth < 1)
            {
                errorMessage.AppendLine("Tile Width must be be at least 1.");
            }
            else if (map.TileHeight < 1)
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
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter stream = new StreamWriter(file))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                serializer.Serialize(writer, json);
            }
        }

        #endregion methods
    }
}
