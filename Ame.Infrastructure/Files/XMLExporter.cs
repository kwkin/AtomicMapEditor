using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Files
{
    /// <summary>
    /// Exports the map as a .ane file
    /// </summary>
    public class XMLExporter
    {
        #region fields
        

        #endregion fields


        #region constructor

        public XMLExporter(string file, Map map)
        {
            this.FilePath = file;
            this.Map = map;
        }

        #endregion constructor


        #region properties

        public string FilePath { get; set; }
        public Map Map { get; set; }

        #endregion properties


        #region methods

        public void Export()
        {
            XmlDocument document = new XmlDocument();
            StringBuilder sringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (FileStream fileStream = new FileStream(this.FilePath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    using (XmlWriter writer = XmlWriter.Create(streamWriter, settings))
                    {
                        writer.WriteStartElement(MapXMLTags.nodeName);
                        MapXMLTags mapXML = new MapXMLTags(this.Map);
                        mapXML.write(writer);
                        writer.WriteStartElement("tilesets");
                        foreach (TilesetModel tileset in this.Map.TilesetList)
                        {
                            TilesetXMLTags tilesetXML = new TilesetXMLTags(tileset);
                            tilesetXML.write(writer);
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("layers");
                        foreach (ILayer layer in this.Map.LayerList)
                        {
                            LayerXMLTags layerXML = new LayerXMLTags(layer);
                            layerXML.write(writer);
                        }
                        writer.WriteEndElement();
                        
                        writer.WriteEndElement();
                    }
                }
            }
        }

        #endregion methods
    }
}
