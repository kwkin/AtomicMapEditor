using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models
{
    public class Map : INotifyPropertyChanged
    {
        #region fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

        public Map()
        {
            this.Name = "";
            this.Columns = 32;
            this.Rows = 32;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new List<Layer>();
            this.LayerList.Add(new Layer("Layer #1", this.TileWidth, this.TileHeight, this.Rows, this.Columns));
        }

        public Map(string name)
        {
            this.Name = name;
            this.Columns = 32;
            this.Rows = 32;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new List<Layer>();
            this.LayerList.Add(new Layer("Layer #1", this.TileWidth, this.TileHeight, this.Rows, this.Columns));
        }

        public Map(string name, int width, int height)
        {
            this.Name = name;
            this.Columns = width;
            this.Rows = height;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new List<Layer>();
            this.LayerList.Add(new Layer("Layer #1", this.TileWidth, this.TileHeight, this.Rows, this.Columns));
        }

        #endregion constructor


        #region properties
        
        private string name { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public int Columns { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public int Rows { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public ScaleType Scale { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public int TileWidth { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Ratio")]
        public int PixelRatio { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Scale")]
        public int PixelScale { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public string Description { get; set; }

        public IList<Layer> LayerList { get; set; }
        
        #endregion properties


        #region methods

        public void SetWidth(int width)

        {

            this.Columns = width;

        }
        
        public void SetHeight(int height)

        {

            this.Rows = height;

        }
        
        public int GetPixelWidth()
        {
            int width = this.Columns;
            switch (this.Scale)
            {
                case ScaleType.Tile:
                    width *= this.TileWidth;
                    break;

                case ScaleType.Pixel:
                default:
                    break;
            }
            return width;
        }

        public int GetPixelHeight()
        {
            int height = this.Rows;
            switch (this.Scale)
            {
                case ScaleType.Tile:
                    height *= this.TileHeight;
                    break;
            
                case ScaleType.Pixel:
                default:
                    break;
            }
            return height;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion methods
    }
}
