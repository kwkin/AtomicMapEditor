using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.App.Wpf.UI.Interactions.ProjectProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Handlers;
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
    public class ProjectNodeViewModel : IProjectExplorerNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;

        #endregion fields


        #region constructor

        public ProjectNodeViewModel(IEventAggregator eventAggregator, IActionHandler actionHandler, Project project)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Project = project ?? throw new ArgumentNullException("layer is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("actionHandler is null");

            this.MapNodes = new ObservableCollection<MapNodeViewModel>();
            project.Maps.ToList().ForEach(map => AddMap(map));

            this.Project.Maps.CollectionChanged += MapsChanged;

            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.EditProjectPropertiesCommand = new DelegateCommand(() => EditProjectProperties());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
        }

        #endregion constructor


        #region properties

        public ICommand NewMapCommand { get; private set; }
        public ICommand EditProjectPropertiesCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }

        public Project Project { get; set; }

        public ObservableCollection<MapNodeViewModel> MapNodes { get; private set; }
        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);

        #endregion properties


        #region methods

        public void AddMap(Map map)
        {
            MapNodeViewModel node = new MapNodeViewModel(this.eventAggregator, this.actionHandler, map);
            this.MapNodes.Add(node);
        }

        public void RemoveMap(Map map)
        {
            IEnumerable<MapNodeViewModel> toRemove = this.MapNodes.Where(entry => entry.Map == map);
            foreach (MapNodeViewModel node in toRemove)
            {
                this.MapNodes.Remove(node);
            }
        }

        private void MapsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Map map in e.NewItems)
                    {
                        AddMap(map);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Map map in e.OldItems)
                    {
                        RemoveMap(map);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        MapNodeViewModel entry = this.MapNodes[oldIndex];
                        this.MapNodes[oldIndex] = this.MapNodes[newIndex];
                        this.MapNodes[newIndex] = entry;
                    }
                    break;

                default:
                    break;
            }
        }

        private void NewMap()
        {
            NewMapInteraction interaction = new NewMapInteraction(this.Project);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void EditProjectProperties()
        {
            EditProjectInteraction interaction = new EditProjectInteraction(this.Project);
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
