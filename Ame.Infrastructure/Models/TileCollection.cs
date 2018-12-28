using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
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
            this.Tiles = new List<Tile>();
        }

        public TileCollection(IList<Tile> tiles)
        {
            this.Tiles = tiles;
        }

        #endregion constructor


        #region properties

        public IList<Tile> Tiles { get; set; }

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

        #endregion methods
    }
}
