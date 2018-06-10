using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    [Serializable]
    public class Tile
    {
        #region fields

        #endregion fields


        #region constructor

        public Tile(Point position, ImageDrawing tileImage)
        {
            this.Position = position;
            this.TileImage = tileImage;
        }

        #endregion constructor


        #region properties

        public Point Position { get; set; }

        [NonSerialized]
        private ImageDrawing tileImage;
        public ImageDrawing TileImage
        {
            get
            {
                return this.tileImage;
            }
            set
            {
                this.tileImage = value;
            }
        }

        #endregion properties


        #region methods

        #endregion methods
    }
}
