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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Files;

namespace Ame.Infrastructure.Models
{
    [XmlRoot("Map")]
    public class Map : INotifyPropertyChanged, IXmlSerializable
    {
        #region fields

        public event PropertyChangedEventHandler PropertyChanged;

        private string name;

        #endregion fields


        #region constructor

        public Map()
        {
            this.Author = "";
            this.Version = Global.version;
            this.Name = "";
            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new LayerCollection();
            this.TilesetList = new ObservableCollection<TilesetModel>();
            this.UndoQueue = new Stack<DrawAction>();
            this.RedoQueue = new Stack<DrawAction>();
        }

        public Map(string name)
        {
            this.Name = name;

            this.Author = "";
            this.Version = Global.version;
            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new LayerCollection();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.RowCount, this.ColumnCount);
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
            this.Version = Global.version;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new LayerCollection();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.RowCount, this.ColumnCount);
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

        public GridModel Grid { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public int ColumnCount
        {
            get
            {
                return this.Grid.ColumnCount();
            }
            set
            {
                this.Grid.SetWidthWithColumns(value);
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public int RowCount
        {
            get
            {
                return this.Grid.RowCount();
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
        public int Version { get; set; }

        public LayerCollection LayerList { get; set; }
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
            int previousTileIndex = (int)(tile.Bounds.X / this.TileWidth) + (int)(tile.Bounds.Y / this.TileHeight) * this.ColumnCount;
            ImageDrawing previousImage = this.CurrentLayer.LayerItems[previousTileIndex] as ImageDrawing;
            Tile previousTileID = this.CurrentLayer.TileIDs[previousTileIndex];
            //this.CurrentLayer.LayerItems[previousTileIndex] = tile.Image;
            this.CurrentLayer.TileIDs[previousTileIndex] = tile;
            previousImage.Rect = tile.Bounds;
            Tile previousTile = new Tile(previousImage, previousTileID.TilesetID, previousTileID.TileID);
            return previousTile;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer tilesetSerializer = new XmlSerializer(typeof(TilesetModel));
            XmlSerializer layerCollectionSerializer = new XmlSerializer(typeof(LayerCollection));
            int level = 1;
            while (reader.Read() && level > 0)
            {
                // TODO: fix error with reading end tags that denote a start and end <example/>
                if (reader.IsStartElement() && !reader.IsEmptyElement)
                {
                    level++;
                    AmeXMLTags tag = XMLTagMethods.GetTag(reader.Name);
                    if (tag == AmeXMLTags.Layers)
                    {
                        this.LayerList = (LayerCollection)layerCollectionSerializer.Deserialize(reader);
                        foreach (Layer layer in this.LayerList)
                        {
                            layer.TileIDs.refreshDrawing(this.TilesetList, layer);
                        }
                    }
                    else if (reader.Read() && tag != AmeXMLTags.Null)
                    {
                        string value = reader.Value;
                        switch (tag)
                        {
                            case AmeXMLTags.Version:
                                this.Version = int.Parse(value);
                                break;
                            case AmeXMLTags.Name:
                                this.Name = value;
                                break;
                            case AmeXMLTags.Author:
                                this.Author = value;
                                break;
                            case AmeXMLTags.Rows:
                                this.RowCount = int.Parse(value);
                                break;
                            case AmeXMLTags.Columns:
                                this.ColumnCount = int.Parse(value);
                                break;
                            case AmeXMLTags.TileWidth:
                                this.TileWidth = int.Parse(value);
                                break;
                            case AmeXMLTags.TileHeight:
                                this.TileHeight = int.Parse(value);
                                break;
                            case AmeXMLTags.Scale:
                                ScaleType xmlScale;
                                Enum.TryParse(value, out xmlScale);
                                this.Scale = xmlScale;
                                break;
                            case AmeXMLTags.BackgroundColor:
                                this.BackgroundColor = (Color)ColorConverter.ConvertFromString(value);
                                break;
                            case AmeXMLTags.Description:
                                this.Description = value;
                                break;
                            case AmeXMLTags.Tilesets:
                                while (reader.IsStartElement())
                                {
                                    TilesetModel tileset = (TilesetModel)tilesetSerializer.Deserialize(reader);
                                    this.TilesetList.Add(tileset);
                                    tileset.RefreshTilesetImage();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    level--;
                }
            }
            try
            {
                isValid(this);
            }
            catch(FileFormatException exception)
            {
                throw new FileFormatException("Error when reading file. " + exception.Message);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Version, this.Version);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Name, this.Name);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Author, this.Author);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Rows, this.RowCount);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Columns, this.ColumnCount);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TileWidth, this.TileWidth);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TileHeight, this.TileHeight);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Scale, this.Scale);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.BackgroundColor, this.BackgroundColor);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Description, this.Description);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XMLTagMethods.WriteStartElement(writer, AmeXMLTags.Tilesets);
            XmlSerializer serializeTileset = new XmlSerializer(typeof(TilesetModel));
            foreach (TilesetModel tileset in this.TilesetList)
            {
                serializeTileset.Serialize(writer, tileset, ns);
            }
            writer.WriteEndElement();

            XMLTagMethods.WriteStartElement(writer, AmeXMLTags.Layers);
            XmlSerializer serializerLayer = new XmlSerializer(typeof(Layer));
            foreach (Layer layer in this.LayerList)
            {
                serializerLayer.Serialize(writer, layer, ns);
            }
            writer.WriteEndElement();
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
            if (map.Version < 1)
            {
                errorMessage.AppendLine("Version must be be at least 1.");
            }
            else if (map.RowCount < 1)
            {
                errorMessage.AppendLine("Rows must be be at least 1.");
            }
            else if (map.ColumnCount < 1)
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

        #endregion methods
    }
}
