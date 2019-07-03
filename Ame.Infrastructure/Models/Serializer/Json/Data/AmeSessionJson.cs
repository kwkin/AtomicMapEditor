using Ame.Infrastructure.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AmeSessionJson
    {
        public AmeSessionJson()
        {
        }

        public AmeSessionJson(AmeSession session)
        {
            this.Version = Global.Version;
            this.CurrentMap = session.CurrentMapIndex;
            this.OpenedTilesetFiles = new List<string>();
            foreach (Map map in session.MapList)
            {
                this.OpenedTilesetFiles.Add(map.SourcePath.Value);
            }
            this.OpenedTilesetFiles = new List<string>();
            foreach (TilesetModel tileset in session.CurrentTilesetList)
            {
                this.OpenedTilesetFiles.Add(tileset.SourcePath.Value);
            }
            this.LastMapDirectory = session.LastMapDirectory;
            this.LastTilesetDirectory = session.LastTilesetDirectory;

        }

        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "CurrentMap")]
        public int CurrentMap { get; set; }

        [JsonProperty(PropertyName = "OpenedMaps")]
        public IList<string> OpenedMapFiles { get; set; }

        [JsonProperty(PropertyName = "OpenedTilesets")]
        public IList<string> OpenedTilesetFiles { get; set; }

        [JsonProperty(PropertyName = "MapDirectory")]
        public string LastMapDirectory { get; set; }

        [JsonProperty(PropertyName = "TilesetDirectory")]
        public string LastTilesetDirectory { get; set; }

        public AmeSession Generate()
        {
            // TODO load maps, tilesets, and other properties
            AmeSession session = new AmeSession();
            session.LastMapDirectory = this.LastMapDirectory;
            session.LastTilesetDirectory = this.LastTilesetDirectory;
            return session;
        }
    }
}
