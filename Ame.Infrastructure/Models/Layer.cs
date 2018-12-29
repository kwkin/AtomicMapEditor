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
using System.Xml;
using Ame.Infrastructure.Files;
using System.Xml.Serialization;

namespace Ame.Infrastructure.Models
{
    [XmlRoot("Layer")]
    public class Layer : ILayer, INotifyPropertyChanged
    {
        #region fields
        
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

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
            ResetLayerItems();
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
            ResetLayerItems();
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
            ResetLayerItems();
        }

        #endregion constructor


        #region properties
        
        [XmlAttribute]
        public int ID { get; set; } = -1;

        [field: NonSerialized]
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
                    this.name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // TODO have a consistent naming for columns/rows. Sometimes these are named ColumnCount/RowCount
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
        
        [XmlIgnore]
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

        [XmlIgnore]
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

        [XmlElement(ElementName = "tiles")]
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

        public void ResetLayerItems()
        {
            this.TileIDs = new TileCollection();
            this.Group = new DrawingGroup();
            this.TileIDs.reset(this.TileWidth, this.TileHeight);
            RenderOptions.SetEdgeMode(this.Group, EdgeMode.Aliased);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
