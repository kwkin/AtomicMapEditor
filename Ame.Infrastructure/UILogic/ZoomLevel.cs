using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.UILogic
{
    public class ZoomLevel
    {
        #region fields

        #endregion fields


        #region constructor

        public ZoomLevel(double zoom)
        {
            this.description = string.Format("{0}%", zoom * 100);
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

        public static ObservableCollection<ZoomLevel> CreateZoomList(params double[] levels)
        {
            ObservableCollection<ZoomLevel> zoomLevels = new ObservableCollection<ZoomLevel>();
            foreach (double level in levels)
            {
                ZoomLevel zoomLevel = new ZoomLevel(level);
                zoomLevels.Add(zoomLevel);
            }
            zoomLevels.OrderBy(zoomLevel => zoomLevel.zoom);
            return zoomLevels;
        }

        #endregion methods
    }
}
