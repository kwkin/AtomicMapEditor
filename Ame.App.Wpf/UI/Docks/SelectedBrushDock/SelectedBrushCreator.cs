using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.SelectedBrushDock
{
    public class SelectedBrushCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

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
            this.ScrollModel = scrollModel;
        }

        #endregion constructors


        #region properties

        public ScrollModel ScrollModel { get; set; }

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            DockViewModelTemplate template;
            if (this.ScrollModel != null)
            {
                template = new SelectedBrushViewModel(this.eventAggregator, this.ScrollModel);
            }
            else
            {
                template = new SelectedBrushViewModel(this.eventAggregator);
            }
            return template;
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(SelectedBrushViewModel).Equals(type);
        }

        #endregion methods
    }
}
