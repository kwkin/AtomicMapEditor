using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ame.Infrastructure.Models
{
    public class BrushModel
    {
        #region fields

        // TODO combine with tileset model
        public ImageDrawing Image { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        #endregion fields


        #region constructor
        
        public BrushModel()
        {

        }

        public BrushModel(TilesetModel tilesetModel)
        {
            this.TileWidth = (int)tilesetModel.Width;
            this.TileHeight = (int)tilesetModel.Height;
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public int Rows
        {
            get
            {
                return (int)(this.Image.Rect.Height / this.TileHeight);
            }
        }

        public int Columns
        {
            get
            {
                return (int)(this.Image.Rect.Width / this.TileWidth);
            }
        }

        #endregion
    }
}
