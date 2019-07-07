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
    public class LayerJsonWriter : IResourceWriter<Layer>
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerJsonWriter()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public void Write(Layer layer, string path)
        {
            Write(layer, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        public void Write(Layer layer, Stream stream)
        {
            LayerJson json = new LayerJson(layer);

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
