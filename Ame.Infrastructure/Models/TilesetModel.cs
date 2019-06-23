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
using Newtonsoft.Json;

namespace Ame.Infrastructure.Models
{
    // TODO use ImageDrawing.ClipGeometry instead of normal cropping
    // TODO create a custom xerializer class to set the ignored parameters
    public class TilesetModel : PaddedGrid, IItem, IContainsCustomProperties
    {
        #region fields

        #endregion fields


        #region constructor

        public TilesetModel()
        {
            this.ID = -1;
            this.Name.Value = "Tileset #1";
            this.SourcePath.Value = "";
            this.TileWidth.Value = 32;
            this.TileHeight.Value = 32;
            this.IsTransparent.Value = false;
            this.TilesetImage = new DrawingGroup();
            this.TransparentColor.Value = Colors.Transparent;
            this.CustomProperties = new ObservableCollection<MetadataProperty>();
        }

        public TilesetModel(int id, string name)
        {
            this.ID = id;
            this.Name.Value = name;
            this.SourcePath.Value = "";
            this.TileWidth.Value = 32;
            this.TileHeight.Value = 32;
            this.IsTransparent.Value = false;
            this.TilesetImage = new DrawingGroup();
            this.TransparentColor.Value = Colors.Transparent;
            this.CustomProperties = new ObservableCollection<MetadataProperty>();
        }

        public TilesetModel(int id, string name, string sourcePath)
        {
            this.ID = id;
            this.Name.Value = name;
            this.SourcePath.Value = sourcePath;
            this.TileWidth.Value = 32;
            this.TileHeight.Value = 32;
            this.IsTransparent.Value = false;
            this.TilesetImage = new DrawingGroup();
            this.TransparentColor.Value = Colors.Transparent;
            this.CustomProperties = new ObservableCollection<MetadataProperty>();
        }

        #endregion constructor


        #region properties

        // TODO look into changing the structure of IItems
        // TODO Instead of a tree, just have the list. Declare a property indicating the group
        public int ID { get; set; } = -1;

        [MetadataProperty(MetadataType.Property, "Name")]
        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Source Path")]
        public BindableProperty<string> SourcePath { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        public Mat MatImage { get; set; }
        public DrawingGroup TilesetImage { get; set; }

        public BindableProperty<bool> IsTransparent { get; set; } = BindableProperty.Prepare<bool>();

        public BindableProperty<Color> TransparentColor { get; set; } = BindableProperty.Prepare<Color>();

        public ObservableCollection<IItem> Items { get; set; }
        
        public ObservableCollection<MetadataProperty> CustomProperties { get; set; }


        #endregion properties


        #region methods

        public DrawingContext Open()
        {
            return this.TilesetImage.Open();
        }

        public void RefreshTilesetImage()
        {
            this.MatImage = CvInvoke.Imread(this.SourcePath.Value, Emgu.CV.CvEnum.ImreadModes.Unchanged);
            this.TilesetImage = ImageUtils.MatToDrawingGroup(this.MatImage);

            this.Columns.Value = this.MatImage.Size.Width / this.TileWidth.Value;
            this.Rows.Value = this.MatImage.Size.Height / this.TileHeight.Value;
        }

        public ImageDrawing GetByID(int id)
        {
            return GetByID(id, new Point(0, 0));
        }

        public ImageDrawing GetByID(int id, Point startPoint)
        {
            Point topLeftTile = GetPointByID(id);
            Size sizeOfTile = GetTileSize();

            RectangleGeometry geometry = new RectangleGeometry(new Rect(topLeftTile, sizeOfTile));
            
            Mat croppedImage = BrushUtils.CropImage(this.MatImage, topLeftTile, sizeOfTile);
            if (this.IsTransparent.Value)
            {
                croppedImage = ImageUtils.ColorToTransparent(croppedImage, this.TransparentColor.Value);
            }
            Rect drawingRect = new Rect(startPoint, sizeOfTile);
            ImageDrawing drawing = ImageUtils.MatToImageDrawing(croppedImage, drawingRect);
            return drawing;
        }

        #endregion methods
    }
}
