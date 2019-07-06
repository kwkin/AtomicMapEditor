using Ame.App.Wpf.UI.Interactions.ProjectProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
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
    public class ProjectExplorerViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructor

        public ProjectExplorerViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");

            this.Title.Value = "Project Explorer";

            this.ProjectNodes = new ObservableCollection<ProjectNodeViewModel>();

            this.session.Projects.CollectionChanged += ProjectsChanged;

            this.NewProjectCommand = new DelegateCommand(() => NewProject());
            this.OpenProjectCommand = new DelegateCommand(() => OpenProject());
            this.EditProjectPropertiesCommand = new DelegateCommand(() => EditProjectProperties());
            this.RefreshTreeCommand = new DelegateCommand(() => RefreshTree());
            this.CurrentSelectionChangedCommand = new DelegateCommand<object>((entry) => SelectedItemChanged(entry));
        }

        #endregion constructor


        #region properties

        public ICommand NewProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand EditProjectPropertiesCommand { get; private set; }
        public ICommand RefreshTreeCommand { get; private set; }
        public ICommand EditMapPropertiesCommand { get; private set; }
        public ICommand CurrentSelectionChangedCommand { get; private set; }

        public ObservableCollection<ProjectNodeViewModel> ProjectNodes { get; set; }
        public BindableProperty<Project> CurrentProject { get; set; } = BindableProperty.Prepare<Project>();
        public BindableProperty<Map> CurrentMap { get; set; } = BindableProperty.Prepare<Map>();

        #endregion properties


        #region methods

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void NewProject()
        {
            NewProjectInteraction interaction = new NewProjectInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void OpenProject()
        {
            Console.WriteLine("Opening a new project");
        }

        public void RefreshTree()
        {
            Console.WriteLine("Refresh Drop");
        }

        private void SelectedItemChanged(object item)
        {
            if (typeof(ProjectNodeViewModel).IsAssignableFrom(item.GetType()))
            {
                ProjectNodeViewModel projectViewModel = item as ProjectNodeViewModel;
                this.CurrentProject.Value = projectViewModel.Project;
            }
            else if (typeof(MapNodeViewModel).IsAssignableFrom(item.GetType()))
            {
                MapNodeViewModel mapViewModel = item as MapNodeViewModel;
                this.CurrentMap.Value = mapViewModel.Map;
                this.CurrentProject.Value = mapViewModel.Map.Project.Value ?? this.CurrentProject.Value;
            }
        }

        private void EditProjectProperties()
        {
            if (this.CurrentProject == null)
            {
                return;
            }
            EditProjectInteraction interaction = new EditProjectInteraction(this.CurrentProject.Value);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        private void ProjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Project project in e.NewItems)
                    {
                        ProjectNodeViewModel node = new ProjectNodeViewModel(this.eventAggregator, project);
                        this.ProjectNodes.Add(node);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Project project in e.OldItems)
                    {
                        IEnumerable<ProjectNodeViewModel> toRemove = new ObservableCollection<ProjectNodeViewModel>(this.ProjectNodes.Where(entry => entry.Project == project));
                        foreach (ProjectNodeViewModel node in toRemove)
                        {
                            this.ProjectNodes.Remove(node);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        ProjectNodeViewModel entry = this.ProjectNodes[oldIndex];
                        this.ProjectNodes[oldIndex] = this.ProjectNodes[newIndex];
                        this.ProjectNodes[newIndex] = entry;
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion methods
    }
}
