using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models.DrawingBrushes
{
    public class BrushAction
    {
        #region fields

        #endregion fields


        #region constructor

        public BrushAction(string name, List<ImageDrawing> tiles)
        {
            this.Name = name;
            this.Tiles = tiles;
        }

        #endregion constructor


        #region properties

        public string Name { get; set; }
        public List<ImageDrawing> Tiles { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
