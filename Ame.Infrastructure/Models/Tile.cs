using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Ame.Infrastructure.Models
{
    [XmlRoot("Tiles")]
    public class Tile : INotifyPropertyChanged
    {
        // TODO make the image drawing and IDs consistent with their use
        #region fields

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

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
        
        private ImageDrawing image;

        [XmlIgnore]
        public ImageDrawing Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
                NotifyPropertyChanged();
            }
        }

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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion methods
    }
}
