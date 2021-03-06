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
    public class TilesetJsonWriter : IResourceWriter<TilesetModel>
    {
        #region fields

        #endregion fields


        #region constructor

        public TilesetJsonWriter()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public void Write(TilesetModel tilesetModel, string path)
        {
            Write(tilesetModel, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        public void Write(TilesetModel tilesetModel, Stream stream)
        {
            TilesetJson json = new TilesetJson(tilesetModel);

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
