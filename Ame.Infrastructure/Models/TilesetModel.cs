using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Utils;
using Emgu.CV;

namespace Ame.Infrastructure.Models
{
    // TODO use ImageDrawing.ClipGeometry instead of normal cropping
    // TODO create a custom xerializer class to set the ignored parameters
    public class TilesetModel : PaddedGrid, IItem
    {
        #region fields

        #endregion fields


        #region constructor

        public TilesetModel()
        {
            this.ID = -1;
            this.Name = "Tileset #1";
            this.SourcePath = "";
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.IsTransparent = false;
            this.TilesetImage = new DrawingGroup();
            this.TransparentColor = Colors.Transparent;
        }

        public TilesetModel(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.SourcePath = "";
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.IsTransparent = false;
            this.TilesetImage = new DrawingGroup();
            this.TransparentColor = Colors.Transparent;
        }

        public TilesetModel(int id, string name, string sourcePath)
        {
            this.ID = id;
            this.Name = name;
            this.SourcePath = sourcePath;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.IsTransparent = false;
            this.TilesetImage = new DrawingGroup();
            this.TransparentColor = Colors.Transparent;
        }

        #endregion constructor


        #region properties

        // TODO look into changing the structure of IItems
        // TODO Instead of a tree, just have the list. Declare a property indicating the group
        [XmlAttribute]
        public int ID { get; set; } = -1;

        [MetadataProperty(MetadataType.Property, "Name")]
        public string Name { get; set; }

        [MetadataProperty(MetadataType.Property, "Source Path")]
        public string SourcePath { get; set; }

        [XmlIgnore]
        public Mat MatImage;

        [XmlIgnore]
        public DrawingGroup TilesetImage;

        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }

        [XmlIgnore]
        public ObservableCollection<IItem> Items { get; set; }
                        
        #endregion properties


        #region methods

        public DrawingContext Open()
        {
            return this.TilesetImage.Open();
        }

        public void RefreshTilesetImage()
        {
            this.MatImage = CvInvoke.Imread(this.SourcePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
            this.TilesetImage = ImageUtils.MatToDrawingGroup(this.MatImage);
            this.PixelSize = GeometryUtils.DrawingToWindowSize(this.MatImage.Size);
        }

        public ImageDrawing GetByID(int id, Point startPoint)
        {
            Point topLeftTile = GetPointByID(id);
            Size sizeOfTile = this.TileSize;

            RectangleGeometry geometry = new RectangleGeometry(new Rect(topLeftTile, sizeOfTile));
            
            Mat croppedImage = BrushUtils.CropImage(this.MatImage, topLeftTile, sizeOfTile);
            Rect drawingRect = new Rect(startPoint, sizeOfTile);
            ImageDrawing drawing = ImageUtils.MatToImageDrawing(croppedImage, drawingRect);
            return drawing;
        }

        #endregion methods
    }
}
