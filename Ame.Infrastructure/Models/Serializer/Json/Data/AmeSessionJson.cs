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
    public class AmeSessionJson
    {
        public AmeSessionJson()
        {
        }

        public AmeSessionJson(IAmeSession session)
        {
            this.Version = session.Version.Value;
            this.CurrentMap = session.CurrentMapIndex;

            this.OpenedProjectFiles = new List<string>();
            foreach (Project project in session.Projects)
            {
                string projectFile = Path.Combine(project.SourcePath.Value, project.ProjectFilename.Value);
                this.OpenedProjectFiles.Add(projectFile);
            }

            this.OpenedMapFiles = new List<string>();
            foreach (Map map in session.Maps)
            {
                if (map.SourcePath.Value != null)
                {
                    this.OpenedMapFiles.Add(map.SourcePath.Value);
                }
            }

            this.OpenedTilesetFiles = new List<string>();
            foreach (TilesetModel tileset in session.CurrentTilesets)
            {
                this.OpenedTilesetFiles.Add(tileset.SourcePath.Value);
            }
            this.WorkspaceDirectory = session.DefaultWorkspaceDirectory.Value;
            this.LastMapDirectory = session.LastMapDirectory.Value;
            this.LastTilesetDirectory = session.LastTilesetDirectory.Value;

        }

        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "OpenedProjects")]
        public IList<string> OpenedProjectFiles { get; set; }

        [JsonProperty(PropertyName = "CurrentMap")]
        public int CurrentMap { get; set; }

        [JsonProperty(PropertyName = "OpenedMaps")]
        public IList<string> OpenedMapFiles { get; set; }

        [JsonProperty(PropertyName = "OpenedTilesets")]
        public IList<string> OpenedTilesetFiles { get; set; }

        [JsonProperty(PropertyName = "WorkspaceDirectory")]
        public string WorkspaceDirectory { get; set; }

        [JsonProperty(PropertyName = "MapDirectory")]
        public string LastMapDirectory { get; set; }

        [JsonProperty(PropertyName = "TilesetDirectory")]
        public string LastTilesetDirectory { get; set; }

        public IAmeSession Generate()
        {
            // TODO figure out what to do about the tileset files. Should these be kept in project or map?
            ResourceLoader loader = ResourceLoader.Instance;

            IList<Project> projects = new List<Project>();
            ProjectJsonReader projectReader = new ProjectJsonReader();
            foreach (string projectPath in this.OpenedProjectFiles)
            {
                try
                {
                    projects.Add(loader.Load<Project>(projectPath, projectReader));
                }
                catch (Exception e)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Error reading project ");
                    builder.Append(projectPath);
                    builder.Append(" ");
                    builder.Append(e.Message);
                    Console.Error.WriteLine(builder);
                }
            }

            IList<Map> maps = new List<Map>();
            MapJsonReader mapReader = new MapJsonReader();
            foreach (string mapPath in this.OpenedMapFiles)
            {
                try
                {
                    maps.Add(loader.Load<Map>(mapPath, mapReader));
                }
                catch (Exception e)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Error reading map ");
                    builder.Append(mapPath);
                    builder.Append(" ");
                    builder.Append(e.Message);
                    Console.Error.WriteLine(builder);
                }
            }
            // TODO Bug: the current map index is incorrect when a map editor is closed.
            AmeSession session = new AmeSession(maps, projects, this.WorkspaceDirectory, this.LastTilesetDirectory, this.LastMapDirectory, this.Version);
            session.CurrentMap.Value = maps[this.CurrentMap];
            return session;
        }
    }
}
