using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class TilesetModel
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public TilesetModel()
        {
            this.Name = "Tileset #1";
            this.SourcePath = "";
            this.Height = 32;
            this.Width = 32;
            this.OffsetX = 0;
            this.OffsetY = 0;
            this.PaddingX = 0;
            this.PaddingY = 0;
            this.IsTransparent = false;
            this.TransparentColor = Colors.Transparent;
        }

        #endregion constructor & destructer


        #region properties

        public string Name { get; set; }
        public string SourcePath { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int PaddingX { get; set; }
        public int PaddingY { get; set; }
        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
