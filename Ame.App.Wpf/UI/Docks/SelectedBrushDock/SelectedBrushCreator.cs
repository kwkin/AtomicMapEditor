using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
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
        private IConstants constants;

        #endregion fields


        #region constructors

        public SelectedBrushCreator(IEventAggregator eventAggregator, IConstants constants) 
            : this(eventAggregator, constants, null)
        {
        }

        public SelectedBrushCreator(IEventAggregator eventAggregator, IConstants constants, ScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.constants = constants ?? throw new ArgumentNullException("constants is null");

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
                template = new SelectedBrushViewModel(this.eventAggregator, this.constants, this.ScrollModel);
            }
            else
            {
                template = new SelectedBrushViewModel(this.eventAggregator, this.constants);
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
