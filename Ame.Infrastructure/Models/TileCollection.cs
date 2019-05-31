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

namespace Ame.Infrastructure.Models
{
    public class TileCollection : IEnumerable<Tile>
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class TileCollectionJson
        {
            public TileCollectionJson()
            {
            }

            public TileCollectionJson(TileCollection collection)
            {
                int index = 0;
                this.Positions = new int[collection.Tiles.Count() * 2];
                foreach (Tile tile in collection.Tiles)
                {
                    this.Positions[index++] = tile.TilesetID;
                    this.Positions[index++] = tile.TileID;
                }
            }

            [JsonProperty(PropertyName = "Positions")]
            public IList<int> Positions { get; set; }

            public TileCollection Generate()
            {
                TileCollection collection = new TileCollection();
                for (int index = 0; index < this.Positions.Count - 1; index += 2)
                {
                    collection.Tiles.Add(new Tile(this.Positions[index], this.Positions[index + 1]));
                }
                return collection;
            }
        }

        #region fields

        #endregion fields


        #region constructor

        public TileCollection()
        {
            this.Group = new DrawingGroup();
            this.Tiles = new ObservableCollection<Tile>();
            this.initializeTiles();
        }

        public TileCollection(ObservableCollection<Tile> tiles)
        {
            this.Group = new DrawingGroup();
            this.Tiles = tiles;
            this.initializeTiles();
        }

        #endregion constructor


        #region properties
        
        [field: NonSerialized]
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
                    // TODO handle error where tileset ID is not found (where.first throws an error)
                    TilesetModel tilesetModel = tilesetList.Where(tileset => tileset.ID == tile.TilesetID).First();
                    ImageDrawing drawing = tilesetModel.GetByID(tile.TileID, topLeft);
                    tile.Image = drawing;
                }
                this.LayerItems[index] = tile.Image;
                index++;
            }
        }

        public void reset(int tileWidth, int tileHeight)
        {
            for (int xIndex = 0; xIndex < tileWidth; ++xIndex)
            {
                for (int yIndex = 0; yIndex < tileHeight; ++yIndex)
                {
                    Point position = new Point(xIndex * 32, yIndex * 32);
                    Tile emptyTile = Tile.emptyTile(position);
                    this.LayerItems.Add(emptyTile.Image);
                }
            }
        }

        private void initializeTiles()
        {
            this.Tiles.CollectionChanged += (sender, e) =>
            {
                int index = e.NewStartingIndex;
                if (((Tile)e.NewItems[0]).Image != null)
                {
                    this.LayerItems[index] = ((Tile)e.NewItems[0]).Image;
                }
            };
            RenderOptions.SetEdgeMode(this.Group, EdgeMode.Aliased);
        }

        #endregion methods
    }
}
