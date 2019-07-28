using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    // TODO restrict access to some of these classes
    // TODO add a layer group node
    public class LayerNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public LayerNodeViewModel(IEventAggregator eventAggregator, ILayer layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.Layer = layer ?? throw new ArgumentNullException("layer");

            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.EditLayerPropertiesCommand = new DelegateCommand(() => EditLayerProperties());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
        }

        #endregion constructor


        #region properties

        public ICommand NewLayerCommand { get; private set; }
        public ICommand EditLayerPropertiesCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }

        public ILayer Layer { get; set; }
        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);

        #endregion properties


        #region methods
        private void NewLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction(this.Layer.Map.Value);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        private void EditLayerProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction(this.Layer);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        private void EditTextbox()
        {
            this.IsEditingName.Value = true;
        }

        private void StopEditingTextbox()
        {
            this.IsEditingName.Value = false;
        }

        #endregion methods
    }
}
