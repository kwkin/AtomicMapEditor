﻿using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public static class LayerListEntryGenerator
    {
        public static ILayerListEntryViewModel Generate(IEventAggregator eventAggregator, IAmeSession session, ILayer layer)
        {
            ILayerListEntryViewModel entry = null;
            if (typeof(Layer).IsInstanceOfType(layer))
            {
                entry = new LayerListEntryViewModel(eventAggregator, session, layer as Layer);
            }
            else if (typeof(LayerGroup).IsInstanceOfType(layer))
            {
                entry = new LayerListGroupViewModel(eventAggregator, session, layer as LayerGroup);
            }
            return entry;
        }
    }

    public interface ILayerListEntryViewModel
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        ILayer Layer { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
