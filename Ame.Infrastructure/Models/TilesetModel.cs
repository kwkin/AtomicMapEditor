using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class TilesetModel : IItem
    {
        #region fields

        #endregion fields


        #region constructor

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

        public TilesetModel(string name)
        {
            this.Name = name;
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

        #endregion constructor


        #region properties

        // TODO look into changeing the structure of IItems
        // Instead of a tree, just have the list. Declare a property indicating the group
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public DrawingImage ItemImage { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int PaddingX { get; set; }
        public int PaddingY { get; set; }
        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }

        public ObservableCollection<IItem> Items { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
