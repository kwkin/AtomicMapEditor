﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
            this.Tiles = new List<Tile>();
        }

        public BrushModel(int pixelWidth, int pixelHeight)
            : base(pixelWidth, pixelHeight)
        {
            this.Tiles = new List<Tile>();
        }

        public BrushModel(int columns, int rows, int tileWidth, int tileHeight)
            : base(columns, rows, tileWidth, tileHeight)
        {
            this.Tiles = new List<Tile>();
        }

        public BrushModel(TilesetModel tileset)
            : base(tileset.ColumnCount(), tileset.RowCount(), tileset.TileWidth, tileset.TileHeight)
        {
            this.Tiles = new List<Tile>();
        }

        #endregion constructor


        #region properties

        public List<Tile> Tiles { get; set; }

        #endregion properties


        #region methods

        public void TileImage(Mat tilesImage, int tilesetID, Point pixelPoint, TilesetModel tilesetModel)
        {
            this.Tiles.Clear();
            int colCount = tilesImage.Cols / this.TileWidth;
            int rowCount = tilesImage.Rows / this.TileHeight;
            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
            {
                for (int colIndex = 0; colIndex < colCount; ++colIndex)
                {
                    Size tileSize = this.TileSize;
                    Point topLeftPoint = new Point(colIndex * this.TileWidth, rowIndex * this.TileHeight);
                    Mat tileImage = BrushUtils.CropImage(tilesImage, topLeftPoint, tileSize);
                    Rect drawingRect = new Rect(topLeftPoint, tileSize);
                    ImageDrawing drawing = ImageUtils.MatToImageDrawing(tileImage, drawingRect);
                    
                    Point offsetPoint = Point.Add(pixelPoint, (Vector)topLeftPoint);
                    int tileID = tilesetModel.getID(offsetPoint);
                    Tile tile = new Tile(drawing, tilesetID, tileID);
                    this.Tiles.Add(tile);
                }
            }
        }

        #endregion methods
    }
}
