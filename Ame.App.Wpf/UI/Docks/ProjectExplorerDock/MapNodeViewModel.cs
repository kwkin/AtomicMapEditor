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
using System.Collections.Specialized;
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

            this.Map.Layers.CollectionChanged += LayersChanged;

            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
        }

        #endregion constructor


        #region properties

        public ICommand NewLayerCommand { get; private set; }
        public ICommand EditMapPropertiesCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }

        public Map Map { get; set; }

        public ObservableCollection<LayerNodeViewModel> LayerNodes { get; set; }
        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);

        #endregion properties


        #region methods

        private void LayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Layer layer in e.NewItems)
                    {
                        LayerNodeViewModel node = new LayerNodeViewModel(this.eventAggregator, layer);
                        this.LayerNodes.Add(node);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Map map in e.OldItems)
                    {
                        IEnumerable<LayerNodeViewModel> toRemove = this.LayerNodes.Where(entry => entry.Layer == map);
                        foreach (LayerNodeViewModel node in toRemove)
                        {
                            this.LayerNodes.Remove(node);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        LayerNodeViewModel entry = this.LayerNodes[oldIndex];
                        this.LayerNodes[oldIndex] = this.LayerNodes[newIndex];
                        this.LayerNodes[newIndex] = entry;
                    }
                    break;

                default:
                    break;
            }
        }

        // TODO change to action handler class
        private void NewLayer()
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
