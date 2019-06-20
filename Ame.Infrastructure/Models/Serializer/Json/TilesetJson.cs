using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models.Serializer.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TilesetJson
    {
        public TilesetJson()
        {
        }

        public TilesetJson(TilesetModel model)
        {
            this.ID = model.ID;
            this.Name = model.Name.Value;
            this.SourcePath = model.SourcePath.Value;
            this.TileWidth = model.TileWidth.Value;
            this.TileHeight = model.TileHeight.Value;
            this.Scale = model.Scale.Value;
            this.IsTransparent = model.IsTransparent.Value;
            this.TransparentColor = model.TransparentColor.Value;
        }

        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Source")]
        public string SourcePath { get; set; }

        [JsonProperty(PropertyName = "TileWidth")]
        public int TileWidth { get; set; }

        [JsonProperty(PropertyName = "TileHeight")]
        public int TileHeight { get; set; }

        [JsonProperty(PropertyName = "Scale")]
        public ScaleType Scale { get; set; }

        [JsonProperty(PropertyName = "IsTransparent")]
        public bool IsTransparent { get; set; }

        [JsonProperty(PropertyName = "TransparentColor")]
        public Color TransparentColor { get; set; }

        public TilesetModel Generate()
        {
            TilesetModel tileset = new TilesetModel();
            tileset.ID = this.ID;
            tileset.Name.Value = this.Name;
            tileset.SourcePath.Value = this.SourcePath;
            tileset.TileWidth.Value = this.TileWidth;
            tileset.TileHeight.Value = this.TileHeight;
            tileset.Scale.Value = this.Scale;
            tileset.IsTransparent.Value = this.IsTransparent;
            tileset.TransparentColor.Value = this.TransparentColor;
            return tileset;
        }
    }


}
