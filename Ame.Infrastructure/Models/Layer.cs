using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Ame.Infrastructure.Serialization;
using Prism.Mvvm;

namespace Ame.Infrastructure.Models
{
    public class Layer : BindableBase, ILayer
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class LayerJson : JsonAdapter<Layer>
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
                this.Tiles = new TileCollection.TileCollectionJson(layer.TileIDs);
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
            public TileCollection.TileCollectionJson Tiles { get; set; }

            public Layer Generate()
            {
                throw new NotImplementedException();
            }

            public Layer Generate(ObservableCollection<TilesetModel> tilesetList)
            {
                Layer layer = new Layer();
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
                //layer.TileIDs.RefreshDrawing(tilesetList, layer);
                return layer;
            }
        }

        #region fields
        
        #endregion fields


        #region constructor

        public Layer()
        {
            this.Name = "";
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Rows = 32;
            this.Columns = 32;
            this.Position = LayerPosition.Base;
            this.Scale = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
        }

        public Layer(int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Name = "";
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;
            this.Scale = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
        }

        public Layer(string layerName, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Name = layerName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;
            this.Scale = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
        }

        #endregion constructor


        #region properties
        
        public int ID { get; set; } = -1;
        
        private string name;

        [MetadataProperty(MetadataType.Property, "Name")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.SetProperty(ref this.name, value);
                }
            }
        }
        
        [MetadataProperty(MetadataType.Property)]
        public int Columns { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public int Rows { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public int TileWidth { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Offset X")]
        public int OffsetX { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Offset Y")]
        public int OffsetY { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public LayerPosition Position { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public ScaleType Scale { get; set; }

        [MetadataProperty(MetadataType.Property, "Scroll Rate")]
        public double ScrollRate { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public string Description { get; set; }

        private bool isImmutable;
        public bool IsImmutable
        {
            get
            {
                return this.isImmutable;
            }
            set
            {
                this.SetProperty(ref this.isImmutable, value);
            }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.SetProperty(ref this.isVisible, value);
            }
        }
        
        [IgnoreNodeBuilder]
        public DrawingGroup Group
        {
            get
            {
                return this.TileIDs.Group;
            }

            set
            {
                this.TileIDs.Group = value;
            }
        }
        
        [IgnoreNodeBuilder]
        public DrawingCollection LayerItems
        {
            get
            {
                DrawingCollection children = null;
                if (this.TileIDs.Group != null)
                {
                    children = this.TileIDs.Group.Children;
                }
                return children;
            }
        }
        
        public TileCollection TileIDs { get; set; }

        #endregion properties


        #region methods

        public int GetPixelWidth()
        {
            return this.TileWidth * this.Columns;
        }

        public int GetPixelHeight()
        {
            return this.TileHeight * this.Rows;
        }

        public void Clear()
        {
            this.TileIDs.Clear();
        }
        
        public Point getPointFromIndex(int id)
        {
            int pointX = (id % this.Columns) * this.TileWidth;
            int pointY = (int)Math.Floor((double)(id / this.Rows)) * this.TileHeight;
            return new Point(pointX, pointY);
        }

        #endregion methods
    }
}
