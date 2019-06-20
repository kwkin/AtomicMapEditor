using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LayerJson
    {
        public LayerJson()
        {
        }

        public LayerJson(Layer layer)
        {
            this.ID = layer.ID;
            this.Name = layer.Name;
            this.Columns = layer.Columns;
            this.Rows = layer.Rows;
            this.TileWidth = layer.TileWidth;
            this.TileHeight = layer.TileHeight;
            this.OffsetX = layer.OffsetX;
            this.OffsetY = layer.OffsetY;
            this.Position = layer.Position;
            this.Scale = layer.Scale;
            this.ScrollRate = layer.ScrollRate;
            this.IsImmutable = layer.IsImmutable;
            this.IsVisible = layer.IsVisible;
            this.Tiles = new TileCollectionJson(layer.TileIDs);
        }

        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Columns")]
        public int Columns { get; set; }

        [JsonProperty(PropertyName = "Rows")]
        public int Rows { get; set; }

        [JsonProperty(PropertyName = "TileWidth")]
        public int TileWidth { get; set; }

        [JsonProperty(PropertyName = "TileHeight")]
        public int TileHeight { get; set; }

        [JsonProperty(PropertyName = "OffsetX")]
        public int OffsetX { get; set; }

        [JsonProperty(PropertyName = "OffsetY")]
        public int OffsetY { get; set; }

        [JsonProperty(PropertyName = "Position")]
        public LayerPosition Position { get; set; }

        [JsonProperty(PropertyName = "Scale")]
        public ScaleType Scale { get; set; }

        [JsonProperty(PropertyName = "ScrollRate")]
        public double ScrollRate { get; set; }

        [JsonProperty(PropertyName = "IsImmutable")]
        public bool IsImmutable { get; set; }

        [JsonProperty(PropertyName = "IsVisible")]
        public bool IsVisible { get; set; }

        [JsonProperty(PropertyName = "Tiles")]
        public TileCollectionJson Tiles { get; set; }

        public Layer Generate()
        {
            throw new NotImplementedException();
        }

        public Layer Generate(Map map)
        {
            ObservableCollection<TilesetModel> tilesetList = map.TilesetList;
            Layer layer = new Layer(map);
            layer.ID = this.ID;
            layer.Name = this.Name;
            layer.Columns = this.Columns;
            layer.Rows = this.Rows;
            layer.TileWidth = this.TileWidth;
            layer.TileHeight = this.TileHeight;
            layer.OffsetX = this.OffsetX;
            layer.OffsetY = this.OffsetY;
            layer.Position = this.Position;
            layer.Scale = this.Scale;
            layer.ScrollRate = this.ScrollRate;
            layer.IsImmutable = this.IsImmutable;
            layer.IsVisible = this.IsVisible;
            layer.TileIDs = this.Tiles.Generate(layer, tilesetList);
            return layer;
        }
    }

}
