using Ame.Infrastructure.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectJson
    {
        public ProjectJson()
        {
        }

        public ProjectJson(Project project)
        {
            this.Version = Global.Version;
            this.Name = project.Name.Value;
            this.MapFiles = new List<string>();
            foreach (Map map in project.Maps)
            {
                this.MapFiles.Add(map.SourcePath.Name);
            }
            this.TilesetFiles = new List<string>();
            foreach (TilesetModel tileset in project.Tilesets)
            {
                this.TilesetFiles.Add(tileset.SourcePath.Value);
            }
        }

        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Maps")]
        public IList<string> MapFiles { get; set; }

        [JsonProperty(PropertyName = "Tiles")]
        public IList<string> TilesetFiles { get; set; }

        public Project Generate()
        {
            Project project = new Project();
            project.Name.Value = this.Name;

            // Finish
            return project;
        }
    }
}
