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
    public class MapJsonWriter : IResourceWriter<Map>
    {
        #region fields

        #endregion fields


        #region constructor

        public MapJsonWriter()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public void Write(Map map, string path)
        {
            Write(map, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        public void Write(Map map, Stream stream)
        {
            MapJson json = new MapJson(map);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter streamWriter = new StreamWriter(stream))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonWriter, json);
            }
        }

        #endregion methods
    }
}
