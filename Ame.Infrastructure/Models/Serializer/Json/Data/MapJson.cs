using Ame.Infrastructure.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models.Serializer.Json.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MapJson
    {
        public MapJson()
        {
        }

        public MapJson(Map map)
        {
            this.Version = map.Version.Value;
            this.Name = map.Name.Value;
            this.Author = map.Author.Value;
            this.Rows = map.Rows.Value;
            this.Columns = map.Columns.Value;
            this.TileWidth = map.TileWidth.Value;
            this.TileHeight = map.TileHeight.Value;
            this.Scale = map.Scale.Value;
            this.BackgroundColor = map.BackgroundColor.Value;
            this.Description = map.Description.Value;
            this.TilesetJsons = new List<TilesetJson>();
            foreach (TilesetModel model in map.Tilesets)
            {
                this.TilesetJsons.Add(new TilesetJson(model));
            }
            this.LayerJsons = new List<LayerJson>();
            foreach (ILayer layer in map.Layers)
            {
                // TODO fix the conversion
                this.LayerJsons.Add(new LayerJson((Layer)layer));
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
        public IList<TilesetJson> TilesetJsons { get; set; }

        [JsonProperty(PropertyName = "Layers")]
        public IList<LayerJson> LayerJsons { get; set; }

        public Map Generate()
        {
            // TODO user a loader for tileset models
            IList<TilesetModel> tilesets = new List<TilesetModel>();
            foreach (TilesetJson tilesetJson in this.TilesetJsons)
            {
                TilesetModel tileset = tilesetJson.Generate();
                tilesets.Add(tileset);
                tileset.RefreshTilesetImage();
            }

            IList<ILayer> layers = new List<ILayer>();
            foreach (LayerJson layer in this.LayerJsons)
            {
                layers.Add(layer.Generate(tilesets));
            }

            Map map = new Map(this.Name, this.Columns, this.Rows, layers, tilesets);
            map.Version.Value = this.Version;
            map.Author.Value = this.Author;
            map.TileWidth.Value = this.TileWidth;
            map.TileHeight.Value = this.TileHeight;
            map.Scale.Value = this.Scale;
            map.BackgroundColor.Value = this.BackgroundColor;
            map.Description.Value = this.Description;
            return map;
        }
    }


}
