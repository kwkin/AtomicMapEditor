using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ClipboardDock
{
    public class ClipboardCreator : DockCreatorTemplate
    {
        #region fields

        #endregion fields


        #region constructors

        public ClipboardCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.EventAggregator = eventAggregator;
        }

        #endregion constructors


        #region properties

        public IEventAggregator EventAggregator { get; set; }

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new ClipboardViewModel(this.EventAggregator);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(ClipboardViewModel).Equals(type);
        }

        #endregion methods
    }
}
