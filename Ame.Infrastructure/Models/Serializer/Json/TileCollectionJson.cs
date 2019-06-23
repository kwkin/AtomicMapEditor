﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ame.Infrastructure.Models.Serializer.Json
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
            // TODO reevaluate the interface
            throw new NotImplementedException();
        }

        public TileCollection Generate(Layer layer, ObservableCollection<TilesetModel> tilesetList)
        {
            TileCollection collection = new TileCollection(layer);
            for (int index = 0; index < this.Positions.Count - 1; index += 2)
            {
                Tile tile = new Tile(this.Positions[index], this.Positions[index + 1]);

                Point topLeft = layer.getPointFromIndex(index / 2);
                IEnumerable<TilesetModel> models = tilesetList.Where(tileset => tileset.ID == tile.TilesetID);
                if (models.Count() != 0)
                {
                    tile.Image = models.First().GetByID(tile.TileID, topLeft);
                }
                else
                {
                    tile.Image = Tile.emptyTile(topLeft).Image;
                }

                collection.Tiles.Add(tile);
            }
            return collection;
        }
    }

}