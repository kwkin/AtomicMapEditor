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
using Prism.Mvvm;
using Ame.Infrastructure.Models.Serializer.Json;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models
{
    public class Map : BindableBase
    {
        #region fields

        #endregion fields


        #region constructor

        public Map()
        {
            this.Author.Value = "";
            this.Version.Value = Global.Version;
            this.Name.Value = string.Empty;
            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale.Value = ScaleType.Tile;
            this.PixelScale.Value = 1;
            this.Description.Value = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();
            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();
        }

        public Map(string name)
        {
            this.Name.Value = name;

            this.Author.Value = "";
            this.Version.Value = Global.Version;
            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale.Value = ScaleType.Tile;
            this.PixelScale.Value = 1;
            this.Description.Value = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer(this, "Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns);
            this.LayerList.Add(initialLayer);

            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();
        }

        public Map(string name, int width, int height)
        {
            this.Name.Value = name;
            this.Grid = new GridModel(width, height, 32, 32);

            this.Author.Value = "";
            this.Version.Value = Global.Version;
            this.Scale.Value = ScaleType.Tile;
            this.PixelScale.Value = 1;
            this.Description.Value = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer(this, "Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns);
            this.LayerList.Add(initialLayer);

            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<string> SourcePath { get; set; } = BindableProperty.Prepare<string>(string.Empty);

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
                return this.Grid.TileWidth.Value;
            }
            set
            {
                this.Grid.TileWidth.Value = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight
        {
            get
            {
                return this.Grid.TileHeight.Value;
            }
            set
            {
                this.Grid.TileHeight.Value = value;
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty.Prepare<ScaleType>();

        [MetadataProperty(MetadataType.Property, "Pixel Width")]
        public int PixelWidth
        {
            get
            {
                return this.Grid.PixelWidth.Value;
            }
            set
            {
                this.Grid.PixelWidth.Value = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Height")]
        public int PixelHeight
        {
            get
            {
                return this.Grid.PixelHeight.Value;
            }
            set
            {
                this.Grid.PixelHeight.Value = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Ratio")]
        public BindableProperty<int> PixelRatio { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Pixel Scale")]
        public BindableProperty<int> PixelScale { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<string> Description { get; set; } = BindableProperty.Prepare<string>(string.Empty);
        
        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<string> Author { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<string> Version { get; set; } = BindableProperty.Prepare<string>(string.Empty);
        public ObservableCollection<ILayer> LayerList { get; set; }

        public BindableProperty<int> SelectedLayerIndex { get; set; } = BindableProperty.Prepare<int>();
        
        public Layer CurrentLayer
        {
            get
            {
                return this.LayerList[this.SelectedLayerIndex.Value] as Layer;
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
            this.LayerList.RemoveAt(this.SelectedLayerIndex.Value);
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

        public int GetLayerCount()
        {
            int totalLayers = 0;
            IEnumerable<ILayer> layers = this.LayerList.Where(layer => typeof(Layer).IsInstanceOfType(layer));
            totalLayers += layers.Count();

            IEnumerable<ILayer> groups = this.LayerList.Where(layer => typeof(LayerGroup).IsInstanceOfType(layer));
            foreach (LayerGroup group in groups)
            {
                totalLayers += group.GetLayerCount();
            }
            return totalLayers;
        }

        public int GetLayerGroupCount()
        {
            int totalGroups = 0;;

            IEnumerable<ILayer> groups = this.LayerList.Where(layer => typeof(LayerGroup).IsInstanceOfType(layer));
            totalGroups += groups.Count();
            foreach (LayerGroup group in groups)
            {
                totalGroups += group.GetLayerCount();
            }
            return totalGroups;
        }

        #endregion methods
    }
}
