using Ame.Infrastructure.Core;
using Ame.Infrastructure.Handlers;
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
            this.Version = project.Version.Value;
            this.Name = project.Name.Value;
            this.DefaultPixelScale = project.DefaultPixelScale.Value;
            this.DefaultTileWidth = project.DefaultTileWidth.Value;
            this.DefaultTileHeight = project.DefaultTileHeight.Value;
            this.Description = project.Description.Value;

            this.MapFiles = new List<string>();
            foreach (Map map in project.Maps)
            {
                this.MapFiles.Add(map.SourcePath.Value);
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

        [JsonProperty(PropertyName = "PixelScale")]
        public int DefaultPixelScale { get; set; }

        [JsonProperty(PropertyName = "TileWidth")]
        public int DefaultTileWidth { get; set; }

        [JsonProperty(PropertyName = "TileHeight")]
        public int DefaultTileHeight { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Maps")]
        public IList<string> MapFiles { get; set; }

        [JsonProperty(PropertyName = "Tiles")]
        public IList<string> TilesetFiles { get; set; }

        public Project Generate()
        {
            Project project = new Project(this.Name, this.Version);
            project.DefaultPixelScale.Value = this.DefaultPixelScale;
            project.DefaultTileWidth.Value = this.DefaultTileWidth;
            project.DefaultTileHeight.Value = this.DefaultTileHeight;
            project.Description.Value = this.Description;

            ResourceLoader loader = ResourceLoader.Instance;
            MapJsonReader mapReader = new MapJsonReader();
            foreach (string mapPath in this.MapFiles)
            {
                Map map = loader.Load<Map>(mapPath, mapReader);
                project.Maps.Add(map);
            }

            return project;
        }
    }
}
