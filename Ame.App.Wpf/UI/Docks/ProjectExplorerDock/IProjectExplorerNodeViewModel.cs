using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    public static class ProjectExplorerMethods
    {
        #region properties

        #endregion properties


        #region methods
        public static IProjectExplorerNodeViewModel GenerateLayer(IEventAggregator eventAggregator, IActionHandler handler, ILayer layer)
        {
            IProjectExplorerNodeViewModel entry = null;
            if (typeof(Layer).IsInstanceOfType(layer))
            {
                entry = new LayerNodeViewModel(eventAggregator, handler, layer as Layer);
            }
            else if (typeof(LayerGroup).IsInstanceOfType(layer))
            {
                entry = new LayerGroupViewModel(eventAggregator, handler, layer as LayerGroup);
            }
            return entry;
        }

        #endregion methods
    }

    public interface IProjectExplorerNodeViewModel
    {
        #region fields

        #endregion fields


        #region properties

        #endregion properties


        #region methods

        #endregion methods
    }
}
