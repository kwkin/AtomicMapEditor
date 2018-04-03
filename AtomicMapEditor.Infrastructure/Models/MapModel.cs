namespace Ame.Infrastructure.Models
{
    public class MapModel
    {
        #region fields

        #endregion fields

        #region constructor & destructer

        public MapModel()
        {
            this.Name = "";
            this.Width = 1024;
            this.Height = 1024;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
        }

        public MapModel(string name)
        {
            this.Name = name;
            this.Width = 1024;
            this.Height = 1024;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
        }

        public MapModel(string name, int width, int height)
        {
            this.Name = name;
            this.Width = width;
            this.Height = height;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
        }

        #endregion constructor & destructer

        #region properties

        /// <summary>
        /// Name of the map
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Pixel width of the map
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Pixel height of the map
        /// </summary>
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int PixelScale { get; set; }
        public ScaleType Scale { get; set; }
        public string Description { get; set; }

        #endregion properties

        #region methods

        public void setWidth(int width)
        {
            this.Width = width;
        }

        public void setHeight(int height)
        {
            this.Height = height;
        }

        public int getTileWidth()
        {
            int width = this.Width;
            switch (this.Scale)
            {
                case ScaleType.Pixel:
                    width = this.Width;
                    break;

                case ScaleType.Tile:
                    width = this.Width / this.TileWidth;
                    break;
            }
            return width;
        }

        public int getTileHeight()
        {
            int height = this.Height;
            switch (this.Scale)
            {
                case ScaleType.Pixel:
                    height = this.Height;
                    break;

                case ScaleType.Tile:
                    height = this.Height / this.TileHeight;
                    break;
            }
            return height;
        }

        public void setWidthTiles(int xTiles, int tileWidth)
        {
            this.TileWidth = this.TileWidth;
            this.Width = xTiles * tileWidth;
        }

        public void setHeightTiles(int yTiles, int tileHeight)
        {
            this.TileHeight = this.TileHeight;
            this.Height = yTiles * tileHeight;
        }

        #endregion methods
    }
}
