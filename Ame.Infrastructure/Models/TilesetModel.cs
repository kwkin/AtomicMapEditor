using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Utils;
using Emgu.CV;
using Newtonsoft.Json;
using Ame.Infrastructure.Serialization;

namespace Ame.Infrastructure.Models
{
    // TODO use ImageDrawing.ClipGeometry instead of normal cropping
    // TODO create a custom xerializer class to set the ignored parameters
    public class TilesetModel : PaddedGrid, IItem
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class TilesetJson : JsonAdapter<TilesetModel>
        {
            public TilesetJson()
            {
            }

            public TilesetJson(TilesetModel model)
            {
                this.ID = model.ID;
                this.Name = model.Name;
                this.SourcePath = model.SourcePath;
                this.TileWidth = model.TileWidth;
                this.TileHeight = model.TileHeight;
                this.Scale = model.Scale;
                this.IsTransparent = model.IsTransparent;
                this.TransparentColor = model.TransparentColor;
            }

            [JsonProperty(PropertyName = "ID")]
            public int ID { get; set; }

            [JsonProperty(PropertyName = "Name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "Source")]
            public string SourcePath { get; set; }

            [JsonProperty(PropertyName = "TileWidth")]
            public int TileWidth { get; set; }

            [JsonProperty(PropertyName = "TileHeight")]
            public int TileHeight { get; set; }

            [JsonProperty(PropertyName = "Scale")]
            public ScaleType Scale { get; set; }

            [JsonProperty(PropertyName = "IsTransparent")]
            public bool IsTransparent { get; set; }

            [JsonProperty(PropertyName = "TransparentColor")]
            public Color TransparentColor { get; set; }

            public TilesetModel Generate()
            {
                TilesetModel tileset = new TilesetModel();
                tileset.ID = this.ID;
                tileset.Name = this.Name;
                tileset.SourcePath = this.SourcePath;
                tileset.TileWidth = this.TileWidth;
                tileset.TileHeight = this.TileHeight;
                tileset.Scale = this.Scale;
                tileset.IsTransparent = this.IsTransparent;
                tileset.TransparentColor = this.TransparentColor;
                return tileset;
            }
        }


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
        public int ID { get; set; } = -1;

        [MetadataProperty(MetadataType.Property, "Name")]
        public string Name { get; set; }

        [MetadataProperty(MetadataType.Property, "Source Path")]
        public string SourcePath { get; set; }
        
        public Mat MatImage;
        public DrawingGroup TilesetImage;
        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }
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
            if (this.IsTransparent)
            {
                croppedImage = ImageUtils.ColorToTransparent(croppedImage, this.TransparentColor);
            }
            Rect drawingRect = new Rect(startPoint, sizeOfTile);
            ImageDrawing drawing = ImageUtils.MatToImageDrawing(croppedImage, drawingRect);
            return drawing;
        }

        #endregion methods
    }
}
