﻿using System.Collections.ObjectModel;
using Ame.Infrastructure.Models;

namespace Ame.Components.Behaviors
{
    public interface IScrollModel
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        #endregion constructor & destructer


        #region properties

        ObservableCollection<ZoomLevel> ZoomLevels { get; set; }
        int ZoomIndex { get; set; }

        #endregion properties


        #region methods

        int ZoomIn();

        int ZoomOut();

        int SetZoomIndex();

        int SetZoom(ZoomLevel selectedZoomLevel);

        #endregion methods
    }
}
