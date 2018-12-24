using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Files
{
    public class TilesetXMLTags
    {
        #region fields

        public const string nodeName = "tileset";

        #endregion fields


        #region constructor

        public TilesetXMLTags(TilesetModel tileset)
        {
            this.tileset = tileset;
        }

        #endregion constructor


        #region properties

        public TilesetModel tileset { get; set; }
        public int Id { get; set; }
        
        #endregion properties


        #region methods

        public void write(XmlWriter writer)
        {
            writer.WriteStartElement(TilesetXMLTags.nodeName);
            XMLTagMethods.WriteAttribute(writer, AmeXMLTags.ID, this.Id);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.NAME, this.tileset.Name);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.SOURCE, this.tileset.SourcePath);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.ISTRANSPARENT, this.tileset.IsTransparent);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TRANSPARENTCOLOR, this.tileset.TransparentColor);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.WIDTH, this.tileset.TileWidth);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.HEIGHT, this.tileset.TileHeight);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.XOFFSET, this.tileset.OffsetX);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.YOFFSET, this.tileset.OffsetY);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.XPADDING, this.tileset.PaddingX);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.YPADDING, this.tileset.PaddingY);
            writer.WriteEndElement();
        }

        #endregion methods
    }
}
