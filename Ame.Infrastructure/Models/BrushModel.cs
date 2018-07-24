using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ame.Infrastructure.Utils;
using Emgu.CV;

namespace Ame.Infrastructure.Models
{
    public class BrushModel : GridModel
    {
        #region fields

        #endregion fields


        #region constructor
        
        public BrushModel()
            : base()
        {
            this.Tiles = new List<ImageDrawing>();
        }

        public BrushModel(int pixelWidth, int pixelHeight)
            : base(pixelWidth, pixelHeight)
        {
            this.Tiles = new List<ImageDrawing>();
        }

        public BrushModel(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.Tiles = new List<ImageDrawing>();
        }

        public BrushModel(TilesetModel tileset)
            : base(tileset.ColumnCount(), tileset.RowCount(), tileset.TileWidth, tileset.TileHeight)
        {
            this.Tiles = new List<ImageDrawing>();
        }

        #endregion constructor


        #region properties

        public List<ImageDrawing> Tiles { get; set; }

        #endregion properties


        #region methods

        public void TileImage(Mat image)
        {
            this.Tiles.Clear();
            int colCount = image.Cols / this.TileWidth;
            int rowCount = image.Rows / this.TileHeight;
            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
            {
                for (int colIndex = 0; colIndex < colCount; ++colIndex)
                {
                    Size tileSize = this.TileSize;
                    Point topLeftPoint = new Point(colIndex * this.TileWidth, rowIndex * this.TileHeight);
                    Mat tile = BrushUtils.CropImage(image, topLeftPoint, tileSize);
                    Rect drawingRect = new Rect(topLeftPoint, tileSize);
                    ImageDrawing drawing = ImageUtils.MatToImageDrawing(tile, drawingRect);
                    this.Tiles.Add(drawing);
                }
            }
        }

        #endregion
    }
}
