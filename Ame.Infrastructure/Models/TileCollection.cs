using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Files;

namespace Ame.Infrastructure.Models
{
    [XmlRoot("Tiles")]
    public class TileCollection : IEnumerable<Tile>, IXmlSerializable
    {
        #region fields

        #endregion fields


        #region constructor

        public TileCollection()
        {
            this.Group = new DrawingGroup();
            this.Tiles = new ObservableCollection<Tile>();
            this.initializeTiles();
        }

        public TileCollection(ObservableCollection<Tile> tiles)
        {
            this.Group = new DrawingGroup();
            this.Tiles = tiles;
            this.initializeTiles();
        }

        #endregion constructor


        #region properties
        
        [field: NonSerialized]
        private DrawingGroup group;

        [XmlIgnore]
        [IgnoreNodeBuilder]
        public DrawingGroup Group
        {
            get
            {
                return this.group;
            }
            set
            {
                this.group = value;
            }
        }
        
        [XmlIgnore]
        [IgnoreNodeBuilder]
        public DrawingCollection LayerItems
        {
            get
            {
                DrawingCollection children = null;
                if (this.group != null)
                {
                    children = this.group.Children;
                }
                return children;
            }
        }

        public ObservableCollection<Tile> Tiles { get; set; }

        [XmlIgnore]
        public Tile this[int index]
        {
            get
            {
                return this.Tiles[index];
            }
            set
            {
                this.Tiles[index] = value;
            }
        }

        #endregion properties


        #region methods

        public void Add(Tile tile)
        {
            this.Tiles.Add(tile);
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return this.Tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Tiles.GetEnumerator();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            int[] iDs = new int[0];
            if (reader.IsStartElement())
            {
                AmeXMLTags tag = XMLTagMethods.GetTag(reader.Name);
                if (reader.Read() && tag == AmeXMLTags.Positions)
                {
                    string value = reader.Value;
                    iDs = value.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                }
            }
            for (int index = 0; index < iDs.Length - 1; index += 2)
            {
                this.Tiles.Add(new Tile(iDs[index], iDs[index + 1]));
            }
            reader.Read();
            reader.Read();
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            int[] tileIDs = new int[this.Tiles.Count() * 2];
            int index = 0;
            foreach (Tile tile in this.Tiles)
            {
                tileIDs[index++] = tile.TilesetID;
                tileIDs[index++] = tile.TileID;
            }
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Positions, string.Join(",", tileIDs));
        }

        public void refreshDrawing(ObservableCollection<TilesetModel> tilesetList, Layer layer)
        {
            // TODO get layer dimensions and add tiles
            int index = 0;
            foreach (Tile tile in this.Tiles)
            {
                Point topLeft = layer.getPointFromIndex(index);
                if (tile.TilesetID == -1)
                {
                    tile.Image = Tile.emptyTile(topLeft).Image;
                }
                else
                {
                    // TODO handle error where tileset ID is not found (where.first throws an error)
                    TilesetModel tilesetModel = tilesetList.Where(tileset => tileset.ID == tile.TilesetID).First();
                    ImageDrawing drawing = tilesetModel.GetByID(tile.TileID, topLeft);
                    tile.Image = drawing;
                }
                this.LayerItems[index] = tile.Image;
                index++;
            }
        }

        public void reset(int tileWidth, int tileHeight)
        {
            for (int xIndex = 0; xIndex < tileWidth; ++xIndex)
            {
                for (int yIndex = 0; yIndex < tileHeight; ++yIndex)
                {
                    Point position = new Point(xIndex * 32, yIndex * 32);
                    Tile emptyTile = Tile.emptyTile(position);
                    this.LayerItems.Add(emptyTile.Image);
                }
            }
        }

        private void initializeTiles()
        {
            this.Tiles.CollectionChanged += (sender, e) =>
            {
                int index = e.NewStartingIndex;
                if (((Tile)e.NewItems[0]).Image != null)
                {
                    this.LayerItems[index] = ((Tile)e.NewItems[0]).Image;
                }
            };
            RenderOptions.SetEdgeMode(this.Group, EdgeMode.Aliased);
        }

        #endregion methods
    }
}
