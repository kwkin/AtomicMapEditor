using System.Collections.Generic;

namespace Ame.Infrastructure.Models
{
    public class Map
    {
        #region fields

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
        
        public string Name { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int PixelScale { get; set; }
        public ScaleType Scale { get; set; }
        public int PixelRatio { get; set; }
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

        #endregion methods
    }
}
