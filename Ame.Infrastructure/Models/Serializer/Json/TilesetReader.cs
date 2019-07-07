﻿using Ame.Infrastructure.Models.Serializer.Json.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json
{
    public class TilesetReader : IResourceReader<TilesetModel>
    {
        #region fields

        #endregion fields


        #region constructor

        public TilesetReader()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public TilesetModel Read(string path)
        {
            TilesetJson json = JsonConvert.DeserializeObject<TilesetJson>(File.ReadAllText(path));
            return json.Generate();
        }

        #endregion methods
    }
}
