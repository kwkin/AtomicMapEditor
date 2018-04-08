using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Ame.Infrastructure.Models
{
    public class Layer : ILayer
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public Layer(int tileWidth, int tileHeight)
        {
            this.LayerName = "";
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Position = LayerPosition.Base;
        }

        public Layer(int tileWidth, int tileHeight, int rows, int columns)
        {
            this.LayerName = "";
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;
        }

        public Layer(string layerName, int tileWidth, int tileHeight)
        {
            this.LayerName = layerName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Position = LayerPosition.Base;
        }

        public Layer(string layerName, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.LayerName = layerName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Rows = rows;
            this.Columns = columns;
            this.Position = LayerPosition.Base;
        }

        #endregion constructor & destructer


        #region properties
        
        public string LayerName { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public bool IsImmutable { get; set; }
        public LayerPosition Position { get; set; }

        #endregion properties


        #region methods

        public void SetTile(BitmapImage imageBitmap, int row, int column)
        {
            Image image = new Image();
            image.Source = imageBitmap;
            Grid.SetRow(image, row);
            Grid.SetColumn(image, column);
            //this.Children.Add(image);
        }

        public void UpdateGrid()
        {
            //for (int rowIndex = 0; rowIndex < this.Rows; ++rowIndex)
            //{
            //    RowDefinition row = new RowDefinition();
            //    row.Height = new System.Windows.GridLength(this.TileHeight, System.Windows.GridUnitType.Pixel);
            //    this.RowDefinitions.Add(row);
            //}
            //for (int colIndex = 0; colIndex < this.Columns; ++colIndex)
            //{
            //    RowDefinition column = new RowDefinition();
            //    column.Height = new System.Windows.GridLength(this.TileWidth, System.Windows.GridUnitType.Pixel);
            //    this.RowDefinitions.Add(column);
            //}
        }

        #endregion methods
    }
}
