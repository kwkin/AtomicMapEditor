using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace Ame.Infrastructure.Models
{
    public class TileCollection : IEnumerable<Tile>
    {
        #region fields

        #endregion fields


        #region constructor

        public TileCollection(Layer layer)
            : this(layer.TileWidth.Value, layer.TileHeight.Value, layer.Rows.Value, layer.Columns.Value)
        {
        }

        public TileCollection(int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Group = new DrawingGroup();
            this.Tiles = new ObservableCollection<Tile>();
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Colummns = columns;

            this.Tiles.CollectionChanged += TilesCollectionChanged;
            RenderOptions.SetEdgeMode(this.Group, EdgeMode.Aliased);

            this.Initialize();
        }

        public TileCollection(ObservableCollection<Tile> tiles, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Group = new DrawingGroup();
            this.Tiles = tiles;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Colummns = columns;

            this.Tiles.CollectionChanged += TilesCollectionChanged;
            RenderOptions.SetEdgeMode(this.Group, EdgeMode.Aliased);
        }

        #endregion constructor


        #region properties
        
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
                DrawingCollection children = null;
                if (this.group != null)
                {
                    children = this.group.Children;
                }
                return children;
            }
        }

        public ObservableCollection<Tile> Tiles { get; set; }
        
        public Tile this[int index]
        {
            get
            {
                return this.Tiles[index];
            }
            set
            {
                this.Tiles[index] = value;
            }
        }

        // TODO maybe keep reference to layer instead?
        private int tileWidth;
        public int TileWidth
        {
            get
            {
                return this.tileWidth;
            }
            set
            {
                this.tileWidth = value;
            }
        }

        private int tileHeight;
        public int TileHeight
        {
            get
            {
                return this.tileHeight;
            }
            set
            {
                this.tileHeight = value;
            }
        }

        private int rows;
        public int Rows
        {
            get
            {
                return this.rows;
            }
            set
            {
                this.rows = value;
            }
        }

        private int columns;
        public int Colummns
        {
            get
            {
                return this.columns;
            }
            set
            {
                this.columns = value;
            }
        }

        #endregion properties


        #region methods

        public void Add(Tile tile)
        {
            this.Tiles.Add(tile);
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return this.Tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Tiles.GetEnumerator();
        }

        public void RefreshDrawing(ObservableCollection<TilesetModel> tilesetList, Layer layer)
        {
            // TODO get layer dimensions and add tiles
            int index = 0;
            foreach (Tile tile in this.Tiles)
            {
                Point topLeft = layer.getPointFromIndex(index);
                if (tile.TilesetID == -1)
                {
                    tile.Image = Tile.emptyTile(topLeft).Image;
                }
                else
                {
                    IEnumerable<TilesetModel> models = tilesetList.Where(tileset => tileset.ID == tile.TilesetID);
                    if (models.Count() != 0)
                    {
                        tile.Image = models.First().GetByID(tile.TileID, topLeft);
                    }
                    else
                    {
                        tile.Image = Tile.emptyTile(topLeft).Image;
                    }
                }
                index++;
            }
        }

        /// <summary>
        /// Replaces all tiles with empty tiles
        /// </summary>
        public void Clear()
        {
            int index = 0;
            for (int xIndex = 0; xIndex < this.Colummns; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.Rows; ++yIndex)
                {
                    Point position = new Point(xIndex * this.tileWidth, yIndex * this.TileHeight);
                    Tile emptyTile = Tile.emptyTile(position);
                    this.Tiles[index++] = emptyTile;
                }
            }
        }

        public void Resize(int rows, int cols)
        {
            // TODO implement
        }

        private void Initialize()
        {
            this.Tiles.Clear();
            for (int xIndex = 0; xIndex < this.Colummns; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.Rows; ++yIndex)
                {
                    Point position = new Point(xIndex * this.tileWidth, yIndex * this.TileHeight);
                    Tile emptyTile = Tile.emptyTile(position);
                    this.Tiles.Add(emptyTile);
                }
            }
        }

        private void TilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Tile tile in e.NewItems)
                    {
                        this.LayerItems.Add(tile.Image);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Tile tile in e.OldItems)
                    {
                        this.LayerItems.Remove(tile.Image);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.LayerItems[e.NewStartingIndex] = ((Tile)e.NewItems[0]).Image;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.LayerItems.Clear();
                    break;
                default:
                    break;
            }
        }

        #endregion methods
    }
}
