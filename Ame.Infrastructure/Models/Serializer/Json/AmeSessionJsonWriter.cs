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
    public class IAmeSessionJsonWriter : IResourceWriter<IAmeSession>
    {
        #region fields

        #endregion fields


        #region constructor

        public IAmeSessionJsonWriter()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public void Write(IAmeSession session, string path)
        {
            Write(session, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        public void Write(IAmeSession session, Stream stream)
        {
            // TODO open dialog asking user to save the map files before closing
            AmeSessionJson json = new AmeSessionJson(session);

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
