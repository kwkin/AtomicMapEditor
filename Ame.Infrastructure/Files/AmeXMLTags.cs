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
        [XMLTagAttribute("version")]
        VERSION,

        [XMLTagAttribute("name")]
        NAME,

        [XMLTagAttribute("author")]
        AUTHOR,

        [XMLTagAttribute("rows")]
        ROWS,

        [XMLTagAttribute("columns")]
        COLUMNS,

        [XMLTagAttribute("tileWidth")]
        TILEWIDTH,

        [XMLTagAttribute("tileHeight")]
        TILEHEIGHT,

        [XMLTagAttribute("scale")]
        SCALE,

        [XMLTagAttribute("backgroundColor")]
        BACKGROUNDCOLOR,

        [XMLTagAttribute("description")]
        DESCRIPTION,

        [XMLTagAttribute("id")]
        ID,

        [XMLTagAttribute("source")]
        SOURCE,

        [XMLTagAttribute("isTransparent")]
        ISTRANSPARENT,

        [XMLTagAttribute("transparentColor")]
        TRANSPARENTCOLOR,

        [XMLTagAttribute("width")]
        WIDTH,

        [XMLTagAttribute("height")]
        HEIGHT,

        [XMLTagAttribute("xOffset")]
        XOFFSET,

        [XMLTagAttribute("yOffset")]
        YOFFSET,

        [XMLTagAttribute("xPadding")]
        XPADDING,

        [XMLTagAttribute("yPadding")]
        YPADDING,

        [XMLTagAttribute("group")]
        GROUP,

        [XMLTagAttribute("position")]
        POSITION,

        [XMLTagAttribute("scrollRate")]
        SCROLLRATE,

        [XMLTagAttribute("tilesetIds")]
        TILESETIDS,

        [XMLTagAttribute("positions")]
        POSITIONS,
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

        public static string GetName(this AmeXMLTags tag)
        {
            XMLTagAttribute attr = GetAttr(tag);
            return string.Format("{0}", attr.Name);
        }

        public static void WriteElement(XmlWriter xmlWriter, AmeXMLTags tags, object value)
        {
            if (value != null)
            {
                xmlWriter.WriteElementString(GetName(tags), value.ToString());
            }
        }

        public static void WriteAttribute(XmlWriter xmlWriter, AmeXMLTags tags, object value)
        {
            if (value != null)
            {
                xmlWriter.WriteAttributeString(GetName(tags), value.ToString());
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
