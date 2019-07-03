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
    public class ProjectJsonWriter : IJsonWriter<Project>
    {
        #region fields

        #endregion fields


        #region constructor

        public ProjectJsonWriter()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public void Write(Project project, string file)
        {
            Write(project, new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        public void Write(Project project, Stream stream)
        {
            ProjectJson json = new ProjectJson(project);

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
