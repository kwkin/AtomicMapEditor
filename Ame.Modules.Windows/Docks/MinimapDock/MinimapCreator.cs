using System;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.MinimapDock
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
