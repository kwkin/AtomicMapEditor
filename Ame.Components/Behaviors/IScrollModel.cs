﻿using System.Collections.ObjectModel;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;

namespace Ame.Components.Behaviors
{
    public interface IScrollModel
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        ObservableCollection<ZoomLevel> ZoomLevels { get; set; }
        int ZoomIndex { get; set; }

        #endregion properties


        #region methods

        int ZoomIn();

        int ZoomOut();

        int SetZoomIndex();

        int SetZoom(ZoomLevel selectedZoomLevel);

        int SetZoom(int zoomIndex);

        #endregion methods
    }
}
