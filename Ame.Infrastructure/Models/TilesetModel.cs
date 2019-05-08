using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Files;
using Ame.Infrastructure.Utils;
using Emgu.CV;

namespace Ame.Infrastructure.Models
{
    // TODO use ImageDrawing.ClipGeometry instead of normal cropping
    // TODO create a custom xerializer class to set the ignored parameters
    [XmlRoot("Tileset")]
    public class TilesetModel : PaddedGrid, IItem, IXmlSerializable
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
            if (this.IsTransparent)
            {
                croppedImage = ImageUtils.ColorToTransparent(croppedImage, this.TransparentColor);
            }
            Rect drawingRect = new Rect(startPoint, sizeOfTile);
            ImageDrawing drawing = ImageUtils.MatToImageDrawing(croppedImage, drawingRect);
            return drawing;
        }

        public XmlSchema GetSchema()
        {
            throw null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsStartElement() || reader.Name != XMLTagMethods.GetName(AmeXMLTags.Tileset))
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.Append("Cannot deserialize Tileset. The tag initial tag is set to ");
                errorMessage.Append(reader.Name);
                throw new InvalidOperationException(errorMessage.ToString());
            }
            // TODO check what happens when this is set incorrectly or not set at all
            this.ID = int.Parse(reader.GetAttribute(XMLTagMethods.GetName(AmeXMLTags.ID)));
            int level = 1;
            while (reader.Read() && level > 0)
            {
                if (reader.IsStartElement())
                {
                    level++;
                    AmeXMLTags tag = XMLTagMethods.GetTag(reader.Name);
                    if (reader.Read() && tag != AmeXMLTags.Null)
                    {
                        string value = reader.Value;
                        switch (tag)
                        {
                            case AmeXMLTags.Name:
                                this.Name = value;
                                break;
                            case AmeXMLTags.Source:
                                this.SourcePath = value;
                                break;
                            case AmeXMLTags.TileWidth:
                                this.TileWidth = int.Parse(value);
                                break;
                            case AmeXMLTags.TileHeight:
                                this.TileHeight = int.Parse(value);
                                break;
                            case AmeXMLTags.Scale:
                                ScaleType xmlScale;
                                Enum.TryParse(value, out xmlScale);
                                this.Scale = xmlScale;
                                break;
                            case AmeXMLTags.OffsetX:
                                this.OffsetX = int.Parse(value);
                                break;
                            case AmeXMLTags.OffsetY:
                                this.OffsetY = int.Parse(value);
                                break;
                            case AmeXMLTags.PaddingX:
                                this.PaddingX = int.Parse(value);
                                break;
                            case AmeXMLTags.PaddingY:
                                this.PaddingY = int.Parse(value);
                                break;
                            case AmeXMLTags.IsTransparent:
                                this.IsTransparent = bool.Parse(value);
                                break;
                            case AmeXMLTags.TransparentColor:
                                this.TransparentColor = (Color)ColorConverter.ConvertFromString(value);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    level--;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XMLTagMethods.WriteAttribute(writer, AmeXMLTags.ID, this.ID);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Name, this.Name);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Source, this.SourcePath);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TileWidth, this.TileWidth);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.TileHeight, this.TileHeight);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Scale, this.Scale);
            if (this.OffsetX != 0)
            {
                XMLTagMethods.WriteElement(writer, AmeXMLTags.OffsetX, this.OffsetX);
            }
            if (this.OffsetY != 0)
            {
                XMLTagMethods.WriteElement(writer, AmeXMLTags.OffsetY, this.OffsetY);
            }
            if (this.PaddingX != 0)
            {
                XMLTagMethods.WriteElement(writer, AmeXMLTags.PaddingX, this.PaddingX);
            }
            if (this.PaddingY != 0)
            {
                XMLTagMethods.WriteElement(writer, AmeXMLTags.PaddingY, this.PaddingY);
            }
            XMLTagMethods.WriteElement(writer, AmeXMLTags.IsTransparent, this.IsTransparent);
            if (this.IsTransparent)
            {
                XMLTagMethods.WriteElement(writer, AmeXMLTags.TransparentColor, this.TransparentColor);
            }
        }

        #endregion methods
    }
}
