using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ClipboardDock
{
    public class ClipboardCreator : IDockCreator
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public ClipboardCreator(IEventAggregator eventAggregator)
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
            return new ClipboardViewModel(this.eventAggregator);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(ClipboardViewModel).Equals(type);
        }

        // TODO look into reflectance to update this (possibly have in extended class)
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
