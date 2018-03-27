using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicMapEditor.Infrastructure.Models
{
    public class ZoomLevel
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public ZoomLevel(string description, double zoom)
        {
            this.description = description;
            this.zoom = zoom;
        }

        #endregion constructor & destructer


        #region properties

        public string description { get; set; }
        public double zoom { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
