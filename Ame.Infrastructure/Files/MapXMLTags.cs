using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Files
{
    public class MapXMLTags
    {
        #region fields

        public const string nodeName = "map";

        #endregion fields


        #region constructor

        public MapXMLTags(Map map)
        {
            this.Map = map;
        }

        #endregion constructor


        #region properties

        public Map Map { get; set; }
        public int Version { get; set; }
        public int Author { get; set; }

        #endregion properties


        #region methods

        public void write(XmlWriter writer)
        {
            XMLTagMethods.WriteElement(writer, AmeXMLTags.VERSION, this.Version);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.NAME, this.Map.Name);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.AUTHOR, this.Author);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.ROWS, this.Map.RowCount);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.COLUMNS, this.Map.ColumnCount);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TILEWIDTH, this.Map.TileWidth);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TILEHEIGHT, this.Map.TileHeight);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.SCALE, this.Map.Scale);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.BACKGROUNDCOLOR, this.Map.BackgroundColor);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.DESCRIPTION, this.Map.Description);
        }

        #endregion methods
    }
}
