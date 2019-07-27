using Ame.Infrastructure.Models.Serializer.Json.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json
{
    public class MapJsonReader : IResourceReader<Map>
    {
        #region fields

        #endregion fields


        #region constructor

        public MapJsonReader()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public Map Read(string path)
        {
            MapJson json = JsonConvert.DeserializeObject<MapJson>(File.ReadAllText(path));
            return json.Generate(path);
        }

        #endregion methods
    }
}
