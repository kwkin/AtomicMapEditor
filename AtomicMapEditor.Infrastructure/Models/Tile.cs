using System.Windows;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class Tile
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public Tile(Point position, ImageDrawing tileImage)
        {
            this.Position = position;
            this.TileImage = tileImage;
        }

        #endregion constructor & destructer


        #region properties

        public Point Position { get; set; }
        public ImageDrawing TileImage { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
