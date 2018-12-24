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

namespace Ame.Infrastructure.Models
{
    [Serializable]
    public class Layer : ILayer, INotifyPropertyChanged
    {
        #region fields
        
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

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

        private string name { get; set; }

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
        private DrawingGroup group;

        [IgnoreNodeBuilder]
        public DrawingGroup Group
        {
            get
            {
                return this.group;
            }
            set
            {
                this.group = value;
            }
        }

        [IgnoreNodeBuilder]
        public DrawingCollection LayerItems
        {
            get
            {
                return this.group.Children;
            }
        }
        
        private IList<Tile> tileIDs;

        [IgnoreNodeBuilder]
        public IList<Tile> TileIDs
        {
            get
            {
                return this.tileIDs;
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

        public void SerializeXML(XmlWriter writer)
        {
            XMLTagMethods.WriteElement(writer, AmeXMLTags.NAME, this.Name);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.GROUP, this.Group);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.ROWS, this.Rows);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.COLUMNS, this.Columns);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TILEWIDTH, this.TileWidth);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TILEHEIGHT, this.TileHeight);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.XOFFSET, this.OffsetX);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.YOFFSET, this.OffsetY);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.POSITION, this.Position);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.SCROLLRATE, this.ScrollRate);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.DESCRIPTION, this.Description);

            writer.WriteStartElement("tiles");
            //string[] tilePositions = new string[this.Group.Children.Count() * 2];
            //int index = 0;
            //foreach (Drawing collection in this.Group.Children)
            //{
            //    tilePositions[index++] = collection.Bounds.X.ToString();
            //    tilePositions[index++] = collection.Bounds.Y.ToString();
            //}
            //XMLTagMethods.WriteElement(writer, AmeXMLTags.POSITIONS, string.Join(",", tilePositions));

            int[] tileIDs = new int[this.TileIDs.Count() * 2];
            int index = 0;
            foreach (Tile tile in this.TileIDs)
            {
                tileIDs[index++] = tile.TilesetID;
                tileIDs[index++] = tile.TileID;
            }
            XMLTagMethods.WriteElement(writer, AmeXMLTags.POSITIONS, string.Join(",", tileIDs));

            writer.WriteEndElement();
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
            this.Group = new DrawingGroup();
            this.tileIDs = new List<Tile>();
            RenderOptions.SetEdgeMode(this.Group, EdgeMode.Aliased);
        }
        
        #endregion methods
    }
}
