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
    public class XMLImporter
    {
        #region fields
        
        #endregion fields


        #region constructor

        public XMLImporter(string file)
        {
            this.File = file;
        }

        #endregion constructor


        #region properties
        
        public string File { get; set; }

        #endregion properties


        #region methods
        
        public Map Import()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Map));
            StreamReader reader = new StreamReader(this.File);
            Map map = (Map)serializer.Deserialize(reader);
            reader.Close();
            return map;
        }

        #endregion methods
    }
}
