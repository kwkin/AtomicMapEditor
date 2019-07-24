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
    public class IAmeSessionJsonReader : IResourceReader<IAmeSession>
    {
        #region fields
        
        #endregion fields


        #region constructor

        public IAmeSessionJsonReader()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public IAmeSession Read(string file)
        {
            IAmeSessionJson json = JsonConvert.DeserializeObject<IAmeSessionJson>(File.ReadAllText(file));
            return json.Generate();
        }

        #endregion methods
    }
}
