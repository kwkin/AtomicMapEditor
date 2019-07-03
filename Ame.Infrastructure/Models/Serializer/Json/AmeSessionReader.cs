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
    public class AmeSessionReader : IJsonReader<AmeSession>
    {
        #region fields
        
        #endregion fields


        #region constructor

        public AmeSessionReader()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public AmeSession Read(string file)
        {
            AmeSessionJson json = JsonConvert.DeserializeObject<AmeSessionJson>(File.ReadAllText(file));
            return json.Generate();
        }

        #endregion methods
    }
}
