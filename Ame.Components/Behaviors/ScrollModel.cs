using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Prism.Mvvm;

namespace Ame.Components.Behaviors
{
    public class ScrollModel : BindableBase, IScrollModel
    {
        #region fields

        #endregion fields


        #region constructor

        public static ScrollModel DefaultScrollModel()
        {
            ScrollModel scrollModel = new ScrollModel();
            ObservableCollection<ZoomLevel> ZoomLevels = ZoomLevel.CreateZoomList(0.125, 0.25, 0.5, 1, 2, 4, 8, 16, 32);
            scrollModel.ZoomLevels = ZoomLevels;
            scrollModel.ZoomIndex = 3;
            return scrollModel;
        }

        public ScrollModel()
        {
            this.ZoomLevels = new ObservableCollection<ZoomLevel>();
            this.ZoomIndex = 0;
        }

        #endregion constructor


        #region properties

        private ObservableCollection<ZoomLevel> zoomLevels;
        public ObservableCollection<ZoomLevel> ZoomLevels
        {
            get
            {
                return this.zoomLevels;
            }
            set
            {
                SetProperty(ref this.zoomLevels, value);
            }
        }

        private int zoomIndex;
        public int ZoomIndex
        {
            get
            {
                return this.zoomIndex;
            }
            set
            {
                SetProperty(ref this.zoomIndex, value);
            }
        }

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

        public int SetZoom(ZoomLevel desiredZoomLevel)
        {
            ZoomLevel selectedZoomLevel = this.ZoomLevels.First((zoomLevel) => zoomLevel.zoom == desiredZoomLevel.zoom);
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

        public int SetZoom(int zoomIndex)
        {
            if (zoomIndex < 0)
            {
                return this.ZoomIndex;
            }
            if (zoomIndex > ZoomLevels.Count - 1)
            {
                return this.ZoomIndex;
            }
            this.ZoomIndex = zoomIndex;
            return this.ZoomIndex;
        }

        public double GetZoomLevel()
        {
            return this.ZoomLevels[this.ZoomIndex].zoom;
        }

        #endregion methods
    }
}
