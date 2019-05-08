using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ame.Infrastructure.Files;

namespace Ame.Infrastructure.Models
{
    [XmlRoot("Layers")]
    public class LayerCollection : IEnumerable<ILayer>, IXmlSerializable
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerCollection()
        {
            this.Layers = new ObservableCollection<ILayer>();
        }

        public LayerCollection(ObservableCollection<ILayer> Layers)
        {
            this.Layers = Layers;
        }

        #endregion constructor


        #region properties

        public ObservableCollection<ILayer> Layers { get; set; }

        public int Count { get; set; }

        [XmlIgnore]
        public ILayer this[int index]
        {
            get
            {
                return this.Layers[index];
            }
            set
            {
                this.Layers[index] = value;
            }
        }

        #endregion properties


        #region methods

        public IEnumerator<ILayer> GetEnumerator()
        {
            return this.Layers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Layers.GetEnumerator();
        }

        public void Add(ILayer layer)
        {
            this.Layers.Add(layer);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }
        
        public void RemoveAt(int index)
        {
            this.Layers.RemoveAt(index);
        }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsStartElement() || reader.Name != XMLTagMethods.GetName(AmeXMLTags.Layers))
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.Append("Cannot deserialize Layers. The tag initial tag is set to ");
                errorMessage.Append(reader.Name);
                throw new InvalidOperationException(errorMessage.ToString());
            }

            XmlSerializer layerSerializer = new XmlSerializer(typeof(Layer));
            int level = 1;
            while (reader.Read() && level > 0)
            {
                if (reader.IsStartElement())
                {
                    // TODO layer group
                    Layer layer = (Layer)layerSerializer.Deserialize(reader);
                    layer.TileIDs.reset(layer.TileWidth, layer.TileHeight);
                    this.Layers.Add(layer);
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    level--;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XMLTagMethods.WriteStartElement(writer, AmeXMLTags.Layers);
            XmlSerializer serializerLayer = new XmlSerializer(typeof(Layer));
            foreach (Layer layer in this.Layers)
            {
                serializerLayer.Serialize(writer, layer, ns);
            }
            writer.WriteEndElement();
        }

        #endregion methods
    }
}
