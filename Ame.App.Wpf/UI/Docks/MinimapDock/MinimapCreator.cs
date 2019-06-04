using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.MinimapDock
{
    public class MinimapCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public MinimapCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new MinimapViewModel(this.eventAggregator);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(MinimapViewModel).Equals(type);
        }

        #endregion methods
    }
}
