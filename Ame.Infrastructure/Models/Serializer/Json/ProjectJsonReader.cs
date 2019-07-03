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
    public class ProjectJsonReader : IJsonReader<Project>
    {
        #region fields

        #endregion fields


        #region constructor

        public ProjectJsonReader()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public Project Read(string file)
        {
            ProjectJson projectJson = JsonConvert.DeserializeObject<ProjectJson>(File.ReadAllText(file));
            return projectJson.Generate();
        }

        #endregion methods
    }
}
