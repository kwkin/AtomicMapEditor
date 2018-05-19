using System;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ToolboxDock
{
    public class ToolboxCreator : DockCreatorTemplate
    {
        #region fields

        public IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public ToolboxCreator(IEventAggregator eventAggregator)
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
            return new ToolboxViewModel(this.eventAggregator);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(ToolboxViewModel).Equals(type);
        }

        #endregion methods
    }
}
