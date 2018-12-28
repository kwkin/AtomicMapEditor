using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Ame.Infrastructure.Models
{
    [XmlRoot("Tiles")]
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
        {
            this.TilesetID = tilesetID;
            this.TileID = tileID;
        }

        public Tile(ImageDrawing image, int tilesetID, int tileID)
        {
            this.Image = image;
            this.TilesetID = tilesetID;
            this.TileID = tileID;
        }

        #endregion constructor


        #region properties

        [XmlIgnore]
        public ImageDrawing Image { get; set; }

        public int TilesetID { get; set; }
        public int TileID { get; set; }

        [XmlIgnore]
        public Rect Bounds
        {
            get
            {
                return this.Image.Bounds;
            }
        }

        #endregion properties


        #region methods

        public static Tile emptyTile(Point pixelPosition)
        {
            Rect rect = new Rect(pixelPosition, new Size(32, 32));
            ImageDrawing emptyTile = new ImageDrawing(new DrawingImage(), rect);
            return new Tile(emptyTile, -1, -1);
        }

        #endregion methods
    }
}
