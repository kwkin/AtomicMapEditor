using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.LayerListDock
{
    public static class LayerListMethods
    {
        public static ILayerListNodeViewModel Generate(IEventAggregator eventAggregator, IAmeSession session, IActionHandler handler, ILayer layer)
        {
            ILayerListNodeViewModel entry = null;
            if (typeof(Layer).IsInstanceOfType(layer))
            {
                entry = new LayerListNodeViewModel(eventAggregator, session, handler, layer as Layer);
            }
            else if (typeof(LayerGroup).IsInstanceOfType(layer))
            {
                LayerGroup layerGroup = layer as LayerGroup;
                LayerListGroupViewModel groupEntry = new LayerListGroupViewModel(eventAggregator, session, handler, layerGroup);
                foreach(ILayer childLayer in layerGroup.Layers)
                {
                    ILayerListNodeViewModel childEntry = Generate(eventAggregator, session, handler, childLayer);
                    groupEntry.LayerNodes.Add(childEntry);
                }
                entry = groupEntry;
            }
            return entry;
        }

        public static string DragDataName
        {
            get
            {
                return "LayerListNode";
            }
        }
    }

    public interface ILayerListNodeViewModel
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        ILayer Layer { get; set; }
        BindableProperty<bool> IsSelected { get; set; }
        BindableProperty<bool> IsDragAbove { get; set; }
        BindableProperty<bool> IsDragBelow { get; set; }

        #endregion properties


        #region methods

        IEnumerable<ILayerListNodeViewModel> GetNodeFromLayer(ILayer layer);

        #endregion methods
    }
}
