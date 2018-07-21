using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ame.Infrastructure.Models
{
    public class BrushModel : GridModel
    {
        #region fields

        #endregion fields


        #region constructor
        
        public BrushModel()
            : base()
        {

        }

        public BrushModel(int pixelWidth, int pixelHeight)
            : base(pixelWidth, pixelHeight)
        {

        }

        public BrushModel(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {

        }

        public BrushModel(TilesetModel tileset)
            : base(tileset.ColumnCount(), tileset.RowCount(), tileset.TileWidth, tileset.TileHeight)
        {

        }

        #endregion constructor


        #region properties

        public ImageDrawing Image { get; set; }

        #endregion properties


        #region methods

        #endregion
    }
}
