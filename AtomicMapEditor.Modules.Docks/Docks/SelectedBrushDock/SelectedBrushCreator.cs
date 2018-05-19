using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.SelectedBrushDock
{
    public class SelectedBrushCreator : IDockCreator
    {
        #region fields

        private IEventAggregator eventAggregator;
        private ScrollModel scrollModel;

        #endregion fields


        #region constructors

        public SelectedBrushCreator(IEventAggregator eventAggregator) : this(eventAggregator, null)
        {
        }

        public SelectedBrushCreator(IEventAggregator eventAggregator, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
            this.scrollModel = scrollModel;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            DockViewModelTemplate template;
            if (this.scrollModel != null)
            {
                template = new SelectedBrushViewModel(this.eventAggregator, this.scrollModel);
            }
            else
            {
                template = new SelectedBrushViewModel(this.eventAggregator);
            }
            return template;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(SelectedBrushViewModel).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(IEventAggregator).Equals(type))
            {
                this.eventAggregator = value as IEventAggregator;
            }
            else if (typeof(ScrollModel).Equals(type))
            {
                this.scrollModel = value as ScrollModel;
            }
        }

        #endregion methods
    }
}
