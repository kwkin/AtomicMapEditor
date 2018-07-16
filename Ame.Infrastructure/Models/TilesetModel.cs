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
            this.GridModel = new GridModel()
            {
                CellWidth = 32,
                CellHeight = 32,
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
            this.GridModel = new GridModel()
            {
                CellWidth = 32,
                CellHeight = 32,
                OffsetX = 0,
                OffsetY = 0,
                PaddingX = 0,
                PaddingY = 0,
            };
            this.IsTransparent = false;
            this.TransparentColor = Colors.Transparent;
        }

        public TilesetModel(string name, GridModel gridModel)
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
        
        public GridModel GridModel { get; set; }
        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }
        public ObservableCollection<IItem> Items { get; set; }

        public double Width
        {
            get
            {
                return this.GridModel.CellWidth;
            }
            set
            {
                this.GridModel.CellWidth = value;
            }
        }

        public double Height
        {
            get
            {
                return this.GridModel.CellHeight;
            }
            set
            {
                this.GridModel.CellWidth = value;
            }
        }

        public double OffsetX
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

        public double OffsetY
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

        public double PaddingX
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

        public double PaddingY
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
