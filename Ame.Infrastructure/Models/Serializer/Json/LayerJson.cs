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
            this.Name = layer.Name.Value;
            this.Columns = layer.Columns.Value;
            this.Rows = layer.Rows.Value;
            this.TileWidth = layer.TileWidth.Value;
            this.TileHeight = layer.TileHeight.Value;
            this.OffsetX = layer.OffsetX.Value;
            this.OffsetY = layer.OffsetY.Value;
            this.Position = layer.Position.Value;
            this.Scale = layer.Scale.Value;
            this.ScrollRate = layer.ScrollRate.Value;
            this.IsImmutable = layer.IsImmutable.Value;
            this.IsVisible = layer.IsVisible.Value;
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
            layer.Name.Value = this.Name;
            layer.Columns.Value = this.Columns;
            layer.Rows.Value = this.Rows;
            layer.TileWidth.Value = this.TileWidth;
            layer.TileHeight.Value = this.TileHeight;
            layer.OffsetX.Value = this.OffsetX;
            layer.OffsetY.Value = this.OffsetY;
            layer.Position.Value = this.Position;
            layer.Scale.Value = this.Scale;
            layer.ScrollRate.Value = this.ScrollRate;
            layer.IsImmutable.Value = this.IsImmutable;
            layer.IsVisible.Value = this.IsVisible;
            layer.TileIDs = this.Tiles.Generate(layer, tilesetList);
            return layer;
        }
    }

}
