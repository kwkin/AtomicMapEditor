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
using Prism.Mvvm;

namespace Ame.Infrastructure.Models
{
    public class Layer : BindableBase, ILayer
    {
        #region fields
        
        #endregion fields


        #region constructor

        public Layer(Map map)
        {
            this.Map = map;

            this.Name = "";
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Rows = 32;
            this.Columns = 32;
            this.IsVisible = true;
            this.Position = LayerPosition.Base;
            this.Scale = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
        }

        public Layer(Map map, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Map = map;

            this.Name = "";
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.IsVisible = true;
            this.Position = LayerPosition.Base;
            this.Scale = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
        }

        public Layer(Map map, string layerName, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Map = map;

            this.Name = layerName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.IsVisible = true;
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

        public LayerGroup Parent { get; set; }

        public Map Map { get; set; }

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

        public void AddWith(ILayer layer)
        {
            if (this.Parent == null)
            {
                // TODO change to insert
                this.Map.LayerList.Add(layer);
            }
            else
            {
                this.Parent.AddWith(layer);
            }
        }

        #endregion methods
    }
}
