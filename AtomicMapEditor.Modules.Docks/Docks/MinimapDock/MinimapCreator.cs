using System;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.MinimapDock
{
    public class MinimapCreator : IDockCreator
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

        public DockViewModelTemplate CreateDock()
        {
            return new MinimapViewModel(this.eventAggregator);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(MinimapViewModel).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(IEventAggregator).Equals(type))
            {
                this.eventAggregator = value as IEventAggregator;
            }
        }

        #endregion methods
    }
}
