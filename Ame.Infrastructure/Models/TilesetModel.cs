using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public class TilesetModel : PaddedGrid, IItem
    {
        #region fields

        #endregion fields


        #region constructor

        public TilesetModel()
        {
            this.Name = "Tileset #1";
            this.SourcePath = "";
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.IsTransparent = false;
            this.TransparentColor = Colors.Transparent;
        }

        public TilesetModel(string name)
        {
            this.Name = name;
            this.SourcePath = "";
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.IsTransparent = false;
            this.TransparentColor = Colors.Transparent;
        }

        #endregion constructor


        #region properties

        // TODO look into changing the structure of IItems
        // TODO Instead of a tree, just have the list. Declare a property indicating the group
        public string Name { get; set; }

        public string SourcePath { get; set; }
        
        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }
        public ObservableCollection<IItem> Items { get; set; }
        
        #endregion properties


        #region methods

        #endregion methods
    }
}
