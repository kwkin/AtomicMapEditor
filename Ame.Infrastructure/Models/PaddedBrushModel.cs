﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Utils;
using Emgu.CV;

namespace Ame.Infrastructure.Models
{
    public class PaddedBrushModel : BrushModel
    {
        #region fields

        #endregion fields


        #region constructor

        public PaddedBrushModel()
            : base()
        {
            this.Tiles = new List<ImageDrawing>();
        }

        public PaddedBrushModel(int pixelWidth, int pixelHeight)
            : base(pixelWidth, pixelHeight)
        {
            this.Tiles = new List<ImageDrawing>();
        }

        public PaddedBrushModel(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.Tiles = new List<ImageDrawing>();
        }

        public PaddedBrushModel(int columns, int rows, int tileWidth, int tileHeight, int tileOffsetX, int tileOfsetY)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.TileOffsetX = tileOffsetX;
            this.TileOffsetY = TileOffsetY;
        }

        public PaddedBrushModel(TilesetModel tileset)
            : base(tileset.ColumnCount(), tileset.RowCount(), tileset.TileWidth, tileset.TileHeight)
        {
            this.Tiles = new List<ImageDrawing>();
        }

        public PaddedBrushModel(TilesetModel tileset, int tileOffsetX, int tileOffsetY)
            : base(tileset.ColumnCount(), tileset.RowCount(), tileset.TileWidth, tileset.TileHeight)
        {
            this.Tiles = new List<ImageDrawing>();
            this.TileOffsetX = tileOffsetX;
            this.TileOffsetY = tileOffsetY;
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property, "Tile Offset X")]
        public int TileOffsetX { get; set; } = 0;

        [MetadataProperty(MetadataType.Property, "Tile Offset Y")]
        public int TileOffsetY { get; set; } = 0;

        #endregion properties


        #region methods

        #endregion methods
    }
}