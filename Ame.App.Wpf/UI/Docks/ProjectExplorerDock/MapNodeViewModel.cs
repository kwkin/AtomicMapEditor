using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    public class MapNodeViewModel : IProjectExplorerNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public MapNodeViewModel(IEventAggregator eventAggregator, Map map)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.Map = map ?? throw new ArgumentNullException("layer");

            this.LayerNodes = new ObservableCollection<LayerNodeViewModel>();
            foreach (ILayer layer in map.Layers)
            {
                this.LayerNodes.Add(new LayerNodeViewModel(this.eventAggregator, layer));
            }

            this.AddLayerCommand = new DelegateCommand(() => AddLayer());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
        }

        #endregion constructor


        #region properties

        public ICommand AddLayerCommand { get; private set; }
        public ICommand EditMapPropertiesCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }

        public Map Map { get; set; }

        public ObservableCollection<LayerNodeViewModel> LayerNodes { get; set; }
        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);

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
