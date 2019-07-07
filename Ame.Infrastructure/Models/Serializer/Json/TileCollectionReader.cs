using Ame.Infrastructure.Models.Serializer.Json.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json
{
    public class TileCollectionReader : IResourceReader<TileCollection>
    {
        #region fields

        private Layer layer;
        private ObservableCollection<TilesetModel> tilesets;

        #endregion fields


        #region constructor

        public TileCollectionReader(Layer layer, ObservableCollection<TilesetModel> tilesets)
        {
            this.layer = layer;
            this.tilesets = tilesets;
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public TileCollection Read(string path)
        {
            TileCollectionJson json = JsonConvert.DeserializeObject<TileCollectionJson>(File.ReadAllText(path));
            return json.Generate(this.layer, this.tilesets);
        }
        
        #endregion methods
    }
}
