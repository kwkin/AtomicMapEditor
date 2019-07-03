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
    public class LayerJsonReader : IJsonReader<Layer>
    {
        #region fields

        private Map map;

        #endregion fields


        #region constructor

        public LayerJsonReader(Map map)
        {
            this.map = map;
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public Layer Read(string file)
        {
            LayerJson json = JsonConvert.DeserializeObject<LayerJson>(File.ReadAllText(file));
            return json.Generate(this.map);
        }

        #endregion methods
    }
}
