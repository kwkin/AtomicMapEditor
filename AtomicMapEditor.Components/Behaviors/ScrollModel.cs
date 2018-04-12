using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;

namespace Ame.Components.Behaviors
{
    public class ScrollModel : IScrollModel
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        #endregion constructor & destructer


        #region properties

        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }

        public int ZoomIndex { get; set; } = -1;

        #endregion properties


        #region methods

        public int ZoomIn()
        {
            if (this.ZoomIndex < this.ZoomLevels.Count - 1)
            {
                this.ZoomIndex += 1;
            }
            return this.ZoomIndex;
        }

        public int ZoomOut()
        {
            if (this.ZoomIndex > 0)
            {
                this.ZoomIndex -= 1;
            }
            return this.ZoomIndex;
        }

        public int SetZoomIndex()
        {
            int zoomIndex = this.ZoomIndex;
            if (zoomIndex > ZoomLevels.Count - 1)
            {
                zoomIndex = ZoomLevels.Count - 1;
            }
            else if (zoomIndex < 0)
            {
                zoomIndex = 0;
            }
            this.ZoomIndex = zoomIndex;
            return this.ZoomIndex;
        }

        public int SetZoom(ZoomLevel selectedZoomLevel)
        {
            int zoomIndex = this.ZoomLevels.IndexOf(selectedZoomLevel);
            if (zoomIndex == -1)
            {
                this.ZoomLevels.Add(selectedZoomLevel);
                zoomIndex = this.ZoomLevels.IndexOf(selectedZoomLevel);
            }
            if (zoomIndex > ZoomLevels.Count - 1)
            {
                zoomIndex = ZoomLevels.Count - 1;
            }
            else if (zoomIndex < 0)
            {
                zoomIndex = 0;
            }
            this.ZoomIndex = zoomIndex;
            return this.ZoomIndex;
        }

        #endregion methods
    }
}
