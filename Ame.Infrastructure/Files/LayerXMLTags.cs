using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Files
{
    public class LayerXMLTags
    {
        #region fields

        public const string nodeName = "layer";

        #endregion fields


        #region constructor

        public LayerXMLTags(ILayer layer)
        {
            this.layer = layer;
        }

        #endregion constructor


        #region properties

        public ILayer layer { get; set; }
        public int Id { get; set; }

        #endregion properties


        #region methods

        public void write(XmlWriter writer)
        {
            writer.WriteStartElement(LayerXMLTags.nodeName);
            XMLTagMethods.WriteAttribute(writer, AmeXMLTags.ID, this.Id);
            this.layer.SerializeXML(writer);
            
            writer.WriteEndElement();
        }

        #endregion methods
    }
}
