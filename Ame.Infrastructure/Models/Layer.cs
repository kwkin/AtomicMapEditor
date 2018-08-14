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

namespace Ame.Infrastructure.Models
{
    [Serializable]
    public class Layer : ILayer, INotifyPropertyChanged
    {
        #region fields

        private List<Tile> occupiedTiles;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

        public Layer(int tileWidth, int tileHeight, int rows, int columns)
        {
            this.LayerName = "";
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;
            this.Scale = ScaleType.Tile;
            this.occupiedTiles = new List<Tile>();
            ResetLayerItems();
        }

        public Layer(string layerName, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.LayerName = layerName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;
            this.Scale = ScaleType.Tile;
            this.occupiedTiles = new List<Tile>();
            ResetLayerItems();
        }

        #endregion constructor


        #region properties

        private string layerName { get; set; }

        [MetadataProperty(MetadataType.Property, "Name")]
        public string LayerName
        {
            get
            {
                return this.layerName;
            }
            set
            {
                if (this.layerName != value)
                {
                    this.layerName = value;
                    NotifyPropertyChanged();
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

        public bool IsImmutable { get; set; }
        public bool IsVisible { get; set; }

        [NonSerialized]
        private DrawingGroup layerGroup;

        [IgnoreNodeBuilder]
        public DrawingGroup LayerGroup
        {
            get
            {
                return this.layerGroup;
            }
            set
            {
                this.layerGroup = value;
            }
        }

        [IgnoreNodeBuilder]
        public DrawingCollection LayerItems
        {
            get
            {
                return this.layerGroup.Children;
            }
        }

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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ResetLayerItems()
        {
            this.LayerGroup = new DrawingGroup();
            RenderOptions.SetEdgeMode(this.LayerGroup, EdgeMode.Aliased);
        }

        #endregion methods
    }
}
