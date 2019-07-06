using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

            this.AddLayerCommand = new DelegateCommand(() => AddLayer());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());
        }

        #endregion constructor


        #region properties
        public ICommand AddLayerCommand { get; private set; }
        public ICommand EditMapPropertiesCommand { get; private set; }

        public Map Map { get; set; }

        #endregion properties


        #region methods

        private void AddLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction(this.Map);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        private void EditMapProperties()
        {
            EditMapInteraction interaction = new EditMapInteraction(this.Map);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        #endregion methods
    }
}
