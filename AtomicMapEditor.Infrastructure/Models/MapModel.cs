using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        /// <summary>
        /// Pixel width of the map
        /// </summary>
        private int _Width;
        public int Width
        {
            get
            {
                return this._Width;
            }
            private set
            {
                this._Width = value;
            }
        }

        /// <summary>
        /// Pixel height of the map
        /// </summary>
        private int _Height;
        public int Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                this._Height = value;
            }
        }

        private int _TileWidth;
        public int TileWidth
        {
            get
            {
                return _TileWidth;
            }
            set
            {
                _TileWidth = value;
            }
        }

        private int _TileHeight;
        public int TileHeight
        {
            get
            {
                return _TileHeight;
            }
            set
            {
                _TileHeight = value;
            }
        }

        private int _PixelScale;
        public int PixelScale
        {
            get
            {
                return _PixelScale;
            }
            set
            {
                _PixelScale = value;
            }
        }

        private ScaleType _Scale;
        public ScaleType Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                _Scale = value;
            }
        }

        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

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
            int width = this._Width;
            switch (this.Scale)
            {
                case ScaleType.Pixel:
                    width = this._Width;
                    break;
                case ScaleType.Tile:
                    width = this._Width / this.TileWidth;
                    break;
            }
            return width;
        }

        public int getTileHeight()
        {
            int height = this._Height;
            switch (this.Scale)
            {
                case ScaleType.Pixel:
                    height = this._Height;
                    break;
                case ScaleType.Tile:
                    height = this._Height / this.TileHeight;
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
