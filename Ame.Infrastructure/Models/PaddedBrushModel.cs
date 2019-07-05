using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
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
            this.Tiles = new ObservableCollection<Tile>();
        }

        public PaddedBrushModel(int pixelWidth, int pixelHeight)
            : base(pixelWidth, pixelHeight)
        {
            this.Tiles = new ObservableCollection<Tile>();
        }

        public PaddedBrushModel(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.Tiles = new ObservableCollection<Tile>();
        }

        public PaddedBrushModel(int columns, int rows, int tileWidth, int tileHeight, int tileOffsetX, int tileOffsetY)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.Tiles = new ObservableCollection<Tile>();
            this.TileOffsetX.Value = tileOffsetX;
            this.TileOffsetY.Value = tileOffsetY;
        }

        public PaddedBrushModel(TilesetModel tileset)
            : base(tileset.Columns.Value, tileset.Rows.Value, tileset.TileWidth.Value, tileset.TileHeight.Value)
        {
            this.Tiles = new ObservableCollection<Tile>();
        }

        public PaddedBrushModel(TilesetModel tileset, int tileOffsetX, int tileOffsetY)
            : base(tileset.Columns.Value, tileset.Rows.Value, tileset.TileWidth.Value, tileset.TileHeight.Value)
        {
            this.Tiles = new ObservableCollection<Tile>();
            this.TileOffsetX.Value = tileOffsetX;
            this.TileOffsetY.Value = tileOffsetY;
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property, "Tile Offset X")]
        public BindableProperty<int> TileOffsetX { get; set; } = BindableProperty<int>.Prepare(0);

        [MetadataProperty(MetadataType.Property, "Tile Offset Y")]
        public BindableProperty<int> TileOffsetY { get; set; } = BindableProperty<int>.Prepare(0);

        #endregion properties


        #region methods

        #endregion methods
    }
}
