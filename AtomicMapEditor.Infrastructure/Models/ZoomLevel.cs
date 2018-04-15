using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public class ZoomLevel
    {
        #region fields

        #endregion fields


        #region constructor

        public ZoomLevel(double zoom)
        {
            this.description = String.Format("{0}%", zoom * 100);
            this.zoom = zoom;
        }

        public ZoomLevel(string description, double zoom)
        {
            this.description = description;
            this.zoom = zoom;
        }

        #endregion constructor


        #region properties

        public string description { get; set; }
        public double zoom { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
