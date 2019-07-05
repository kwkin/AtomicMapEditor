using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class Tile
    {
        // TODO make the image drawing and IDs consistent with their use

        #region fields

        #endregion fields


        #region constructor

        public Tile()
        {
        }

        public Tile(int tilesetID, int tileID)
            : this(null, tilesetID, tileID)
        {
        }

        public Tile(ImageDrawing image, int tilesetID, int tileID)
        {
            this.Image.Value = image;
            this.TilesetID = tilesetID;
            this.TileID = tileID;
        }

        #endregion constructor


        #region properties

        public BindableProperty<ImageDrawing> Image { get; set; } = BindableProperty<ImageDrawing>.Prepare();

        public int TilesetID { get; set; }
        public int TileID { get; set; }

        public Rect Bounds
        {
            get
            {
                return this.Image.Value.Bounds;
            }
        }

        #endregion properties


        #region methods

        public static Tile EmptyTile(Point pixelPosition)
        {
            // TODO remove 32, 32
            Rect rect = new Rect(pixelPosition, new Size(32, 32));
            ImageDrawing emptyTile = new ImageDrawing(new DrawingImage(), rect);
            return new Tile(emptyTile, -1, -1);
        }

        public void UpdatePosition(Point pixelPosition)
        {
            Rect adjustedRect = this.Image.Value.Rect;
            adjustedRect.Location = pixelPosition;
            this.Image.Value.Rect = adjustedRect;
        }

        #endregion methods
    }
}
