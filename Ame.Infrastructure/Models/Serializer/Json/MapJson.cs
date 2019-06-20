﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models.Serializer.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MapJson
    {
        public MapJson()
        {
        }

        public MapJson(Map map)
        {
            this.Version = map.Version;
            this.Name = map.Name;
            this.Author = map.Author;
            this.Rows = map.Rows;
            this.Columns = map.Columns;
            this.TileWidth = map.TileWidth;
            this.TileHeight = map.TileHeight;
            this.Scale = map.Scale;
            this.BackgroundColor = map.BackgroundColor;
            this.Description = map.Description;
            this.TilesetList = new List<TilesetJson>();
            foreach (TilesetModel model in map.TilesetList)
            {
                this.TilesetList.Add(new TilesetJson(model));
            }
            this.LayerList = new List<LayerJson>();
            foreach (ILayer layer in map.LayerList)
            {
                // TODO fix the conversion
                this.LayerList.Add(new LayerJson((Layer)layer));
            }
        }

        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "Rows")]
        public int Rows { get; set; }

        [JsonProperty(PropertyName = "Columns")]
        public int Columns { get; set; }

        [JsonProperty(PropertyName = "TileWidth")]
        public int TileWidth { get; set; }

        [JsonProperty(PropertyName = "TileHeight")]
        public int TileHeight { get; set; }

        [JsonProperty(PropertyName = "Scale")]
        public ScaleType Scale { get; set; }

        [JsonProperty(PropertyName = "Color")]
        public Color BackgroundColor { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Tilesets")]
        public IList<TilesetJson> TilesetList { get; set; }

        [JsonProperty(PropertyName = "Layers")]
        public IList<LayerJson> LayerList { get; set; }

        public Map Generate()
        {
            Map map = new Map();
            map.Version = this.Version;
            map.Name = this.Name;
            map.Author = this.Author;
            map.Rows = this.Rows;
            map.Columns = this.Columns;
            map.TileWidth = this.TileWidth;
            map.TileHeight = this.TileHeight;
            map.Scale = this.Scale;
            map.BackgroundColor = this.BackgroundColor;
            map.Description = this.Description;
            foreach (TilesetJson tilesetJson in this.TilesetList)
            {
                TilesetModel tileset = tilesetJson.Generate();
                map.TilesetList.Add(tileset);
                tileset.RefreshTilesetImage();
            }
            foreach (LayerJson layer in this.LayerList)
            {
                map.LayerList.Add(layer.Generate(map));
            }
            return map;
        }
    }


}
