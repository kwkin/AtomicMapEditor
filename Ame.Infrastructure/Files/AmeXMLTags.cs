using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ame.Infrastructure.Files
{
    public enum AmeXMLTags
    {
        [XMLTagAttribute("Map")]
        Map,

        [XMLTagAttribute("Tilesets")]
        Tilesets,

        [XMLTagAttribute("Tileset")]
        Tileset,

        [XMLTagAttribute("Tiles")]
        Tiles,

        [XMLTagAttribute("Layers")]
        Layers,

        [XMLTagAttribute("Layer")]
        Layer,

        [XMLTagAttribute("Version")]
        Version,

        [XMLTagAttribute("Name")]
        Name,

        [XMLTagAttribute("Author")]
        Author,

        [XMLTagAttribute("Rows")]
        Rows,

        [XMLTagAttribute("Columns")]
        Columns,

        [XMLTagAttribute("TileWidth")]
        TileWidth,

        [XMLTagAttribute("TileHeight")]
        TileHeight,

        [XMLTagAttribute("Scale")]
        Scale,

        [XMLTagAttribute("BackgroundColor")]
        BackgroundColor,

        [XMLTagAttribute("Description")]
        Description,

        [XMLTagAttribute("ID")]
        ID,

        [XMLTagAttribute("Source")]
        Source,

        [XMLTagAttribute("IsTransparent")]
        IsTransparent,

        [XMLTagAttribute("TransparentColor")]
        TransparentColor,

        [XMLTagAttribute("Width")]
        Width,

        [XMLTagAttribute("Height")]
        Height,

        [XMLTagAttribute("XOffset")]
        XOffset,

        [XMLTagAttribute("YOffset")]
        YOffset,

        [XMLTagAttribute("XPadding")]
        XPadding,

        [XMLTagAttribute("YPadding")]
        YPadding,

        [XMLTagAttribute("Group")]
        Group,

        [XMLTagAttribute("Position")]
        Position,

        [XMLTagAttribute("ScrollRate")]
        ScrollRate,

        [XMLTagAttribute("TilesetIds")]
        TilesetIDs,

        [XMLTagAttribute("Positions")]
        Positions,

        [XMLTagAttribute("Null")]
        Null,
    }

    public class XMLTagAttribute : Attribute
    {
        #region constructor

        internal XMLTagAttribute(string name)
        {
            this.Name = name;
        }

        #endregion constructor


        #region properties

        public string Name { get; private set; }

        #endregion properties
    }

    public static class XMLTagMethods
    {
        public static AmeXMLTags GetTag(string tag)
        {
            AmeXMLTags ameTag = AmeXMLTags.Null;
            foreach (AmeXMLTags currentTag in Enum.GetValues(typeof(AmeXMLTags)))
            {
                if (GetName(currentTag) == tag)
                {
                    ameTag = currentTag;
                    break;
                }
            }
            return ameTag;
        }

        public static string GetName(this AmeXMLTags tag)
        {
            XMLTagAttribute attr = GetAttr(tag);
            return string.Format("{0}", attr.Name);
        }

        public static void WriteStartElement(XmlWriter xmlWriter, AmeXMLTags tag)
        {
            xmlWriter.WriteStartElement(GetName(tag));
        }

        public static void WriteElement(XmlWriter xmlWriter, AmeXMLTags tag, object value)
        {
            if (value != null)
            {
                xmlWriter.WriteElementString(GetName(tag), value.ToString());
            }
        }

        public static void WriteAttribute(XmlWriter xmlWriter, AmeXMLTags tag, object value)
        {
            if (value != null)
            {
                xmlWriter.WriteAttributeString(GetName(tag), value.ToString());
            }
        }

        private static XMLTagAttribute GetAttr(AmeXMLTags tag)
        {
            return (XMLTagAttribute)Attribute.GetCustomAttribute(ForValue(tag), typeof(XMLTagAttribute));
        }

        private static MemberInfo ForValue(AmeXMLTags p)
        {
            return typeof(AmeXMLTags).GetField(Enum.GetName(typeof(AmeXMLTags), p));
        }
    }
}
