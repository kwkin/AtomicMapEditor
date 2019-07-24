﻿using Ame.Infrastructure.Core;
using Ame.Infrastructure.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IAmeSessionJson
    {
        public IAmeSessionJson()
        {
        }

        public IAmeSessionJson(IAmeSession session)
        {
            this.Version = new Constants().Version;
            this.CurrentMap = session.CurrentMapIndex;

            this.OpenedProjectFiles = new List<string>();
            foreach (Project project in session.Projects)
            {
                this.OpenedProjectFiles.Add(project.SourcePath.Value);
            }

            this.OpenedMapFiles = new List<string>();
            foreach (Map map in session.Maps)
            {
                this.OpenedMapFiles.Add(map.SourcePath.Value);
            }

            this.OpenedTilesetFiles = new List<string>();
            foreach (TilesetModel tileset in session.CurrentTilesets)
            {
                this.OpenedTilesetFiles.Add(tileset.SourcePath.Value);
            }
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

        [JsonProperty(PropertyName = "MapDirectory")]
        public string LastMapDirectory { get; set; }

        [JsonProperty(PropertyName = "TilesetDirectory")]
        public string LastTilesetDirectory { get; set; }

        public IAmeSession Generate()
        {
            // TODO figure out what to do about the tileset files. Should these be kept in project or map?
            IConstants constants = new Constants();
            IAmeSession session = new AmeSession(constants);

            ResourceLoader loader = ResourceLoader.Instance;
            ProjectJsonReader projectReader = new ProjectJsonReader();
            foreach(string projectPath in this.OpenedProjectFiles)
            {
                Project project = loader.Load<Project>(projectPath, projectReader);
                session.Projects.Add(project);
            }

            MapJsonReader mapReader = new MapJsonReader();
            foreach (string mapPath in this.OpenedMapFiles)
            {
                try
                {
                    Map map = loader.Load<Map>(mapPath, mapReader);
                    session.Maps.Add(map);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            session.LastMapDirectory.Value = this.LastMapDirectory;
            session.LastTilesetDirectory.Value = this.LastTilesetDirectory;
            return session;
        }
    }
}
