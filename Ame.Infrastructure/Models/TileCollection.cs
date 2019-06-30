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

        private int columns;
        private int rows;

        #endregion fields


        #region constructor

        public TileCollection(Layer layer)
        {
            this.layer = layer ?? throw new ArgumentNullException("layer is null");

            this.Group = new DrawingGroup();
            this.Tiles = new ObservableCollection<Tile>();
            this.Tiles.CollectionChanged += TilesCollectionChanged;
            RenderOptions.SetEdgeMode(this.Group, EdgeMode.Aliased);

            this.Initialize();
        }

        #endregion constructor


        #region properties
        
        public Layer layer { get; set; }

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
            int index = 0;
            foreach (Tile tile in this.Tiles)
            {
                Point topLeft = layer.GetPointFromIndex(index);
                if (tile.TilesetID == -1)
                {
                    tile.Image.Value = Tile.EmptyTile(topLeft).Image.Value;
                }
                else
                {
                    IEnumerable<TilesetModel> models = tilesetList.Where(tileset => tileset.ID == tile.TilesetID);
                    if (models.Count() != 0)
                    {
                        tile.Image.Value = models.First().GetByID(tile.TileID, topLeft);
                    }
                    else
                    {
                        tile.Image.Value = Tile.EmptyTile(topLeft).Image.Value;
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
            for (int xIndex = 0; xIndex < this.layer.Columns.Value; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.layer.Rows.Value; ++yIndex)
                {
                    Point position = new Point(xIndex * this.layer.TileWidth.Value, yIndex * this.layer.TileHeight.Value);
                    Tile emptyTile = Tile.EmptyTile(position);
                    this.Tiles[index++] = emptyTile;
                }
            }
        }

        public void Resize(int offsetX, int offsetY)
        {
            ObservableCollection<Tile> oldTiles = new ObservableCollection<Tile>();

            for (int yIndex = offsetY; yIndex < this.layer.Rows.Value + offsetY; ++yIndex)
            {
                for (int xIndex = offsetX; xIndex < this.layer.Columns.Value + offsetX; ++xIndex)
                {
                    Tile oldTile;
                    Point position = new Point(xIndex * this.layer.TileWidth.Value, yIndex * this.layer.TileHeight.Value);
                    if (xIndex < this.columns && yIndex < this.rows)
                    {
                        int tileIndex = yIndex * this.columns + xIndex;
                        oldTile = this.Tiles[tileIndex];
                        oldTile.UpdatePosition(position);
                    }
                    else
                    {
                        oldTile = Tile.EmptyTile(position);
                    }
                    oldTiles.Add(oldTile);
                }
            }

            this.Tiles.Clear();
            foreach (Tile oldTile in oldTiles)
            {
                this.Tiles.Add(oldTile);
            }

            this.columns = this.layer.Columns.Value;
            this.rows = this.layer.Rows.Value;
        }

        public void Move(int offsetX, int offsetY)
        {
            for (int yIndex = 0; yIndex < this.layer.Rows.Value; ++yIndex)
            {
                for (int xIndex = 0; xIndex < this.layer.Columns.Value; ++xIndex)
                {
                    Tile tile;
                    Point position = new Point(xIndex * this.layer.TileWidth.Value + offsetX, yIndex * this.layer.TileHeight.Value + offsetY);
                    if (xIndex < this.columns && yIndex < this.rows)
                    {
                        int tileIndex = yIndex * this.columns + xIndex;
                        tile = this.Tiles[tileIndex];
                        tile.UpdatePosition(position);
                    }
                }
            }
        }

        private void Initialize()
        {
            this.Tiles.Clear();
            for (int xIndex = 0; xIndex < this.layer.Columns.Value; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.layer.Rows.Value; ++yIndex)
                {
                    Point position = new Point(xIndex * this.layer.TileWidth.Value, yIndex * this.layer.TileHeight.Value);
                    Tile emptyTile = Tile.EmptyTile(position);
                    this.Tiles.Add(emptyTile);
                }
            }
            this.columns = this.layer.Columns.Value;
            this.rows = this.layer.Rows.Value;
        }

        private void TilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Tile tile in e.NewItems)
                    {
                        this.LayerItems.Add(tile.Image.Value);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Tile tile in e.OldItems)
                    {
                        this.LayerItems.Remove(tile.Image.Value);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.LayerItems[e.NewStartingIndex] = ((Tile)e.NewItems[0]).Image.Value;
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
