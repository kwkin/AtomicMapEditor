using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
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
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializerMap = new XmlSerializer(typeof(Map));
            XmlSerializer serializerTileset = new XmlSerializer(typeof(TilesetModel));
            XmlSerializer serializerLayer = new XmlSerializer(typeof(Layer));

            using (FileStream fileStream = new FileStream(this.FilePath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    using (XmlWriter writer = XmlWriter.Create(streamWriter, settings))
                    {
                        serializerMap.Serialize(writer, this.Map, ns);
                    }
                }
            }
        }

        #endregion methods
    }
}
