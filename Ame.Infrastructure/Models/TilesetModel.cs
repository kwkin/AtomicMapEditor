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
            // TODO change to regular constructor call
            this.GridModel = new PaddedGrid()
            {
                TileWidth = 32,
                TileHeight = 32,
                OffsetX = 0,
                OffsetY = 0,
                PaddingX = 0,
                PaddingY = 0,
            };
            this.IsTransparent = false;
            this.TransparentColor = Colors.Transparent;
        }

        public TilesetModel(string name)
        {
            this.Name = name;
            this.SourcePath = "";
            this.GridModel = new PaddedGrid()
            {
                TileWidth = 32,
                TileHeight = 32,
                OffsetX = 0,
                OffsetY = 0,
                PaddingX = 0,
                PaddingY = 0,
            };
            this.IsTransparent = false;
            this.TransparentColor = Colors.Transparent;
        }

        public TilesetModel(string name, PaddedGrid gridModel)
        {
            this.Name = name;
            this.SourcePath = "";
            this.GridModel = gridModel;
            this.IsTransparent = false;
            this.TransparentColor = Colors.Transparent;
        }

        #endregion constructor


        #region properties

        // TODO look into changing the structure of IItems
        // Instead of a tree, just have the list. Declare a property indicating the group
        public string Name { get; set; }

        public string SourcePath { get; set; }
        
        public PaddedGrid GridModel { get; set; }
        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }
        public ObservableCollection<IItem> Items { get; set; }

        public int Width
        {
            get
            {
                return this.GridModel.TileWidth;
            }
            set
            {
                this.GridModel.TileWidth = value;
            }
        }

        public int Height
        {
            get
            {
                return this.GridModel.TileHeight;
            }
            set
            {
                this.GridModel.TileWidth = value;
            }
        }

        public int OffsetX
        {
            get
            {
                return this.GridModel.OffsetX;
            }
            set
            {
                this.GridModel.OffsetX = value;
            }
        }

        public int OffsetY
        {
            get
            {
                return this.GridModel.OffsetY;
            }
            set
            {
                this.GridModel.OffsetY = value;
            }
        }

        public int PaddingX
        {
            get
            {
                return this.GridModel.PaddingX;
            }
            set
            {
                this.GridModel.PaddingX = value;
            }
        }

        public int PaddingY
        {
            get
            {
                return this.GridModel.PaddingY;
            }
            set
            {
                this.GridModel.PaddingY = value;
            }
        }

        #endregion properties


        #region methods

        #endregion methods
    }
}
