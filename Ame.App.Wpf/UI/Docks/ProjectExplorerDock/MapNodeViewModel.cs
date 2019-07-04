using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    public class MapNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public MapNodeViewModel(IEventAggregator eventAggregator, Map map)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.Map = map ?? throw new ArgumentNullException("layer");
        }

        #endregion constructor


        #region properties

        public Map Map { get; set; }
        
        #endregion properties


        #region methods
        
        #endregion methods
    }
}
