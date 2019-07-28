using Ame.App.Wpf.UI.Interactions.ProjectProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
using Ame.Infrastructure.Utils;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ame.App.Wpf.UI.Interactions.FileChooser;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    // TODO add layers as a node.
    public class ProjectExplorerViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IAmeSession session;

        #endregion fields


        #region constructor

        public ProjectExplorerViewModel(IEventAggregator eventAggregator, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");

            this.Title.Value = "Project Explorer";

            this.ExplorerNodes = new ObservableCollection<IProjectExplorerNodeViewModel>();

            // TODO add change command when a map without a project is added
            this.session.Projects.CollectionChanged += ProjectsChanged;
            RefreshProjects();

            Map map1a = new Map("map1a");
            Map map1b = new Map("map1b");
            map1b.Layers.Add(new Layer(map1a, "layer1bi"));
            Map map1c = new Map("map1c");
            map1c.Layers.Add(new Layer(map1c, "layer1ci"));
            map1c.Layers.Add(new Layer(map1c, "layer1cii"));
            Project project1 = new Project("project1", "1");
            project1.Maps.Add(map1a);
            project1.Maps.Add(map1b);
            project1.Maps.Add(map1c);

            Map map2a = new Map("map2a");
            map2a.Layers.Add(new Layer(map2a, "layer2ai"));
            map2a.Layers.Add(new Layer(map2a, "layer2aii"));
            map2a.Layers.Add(new Layer(map2a, "layer2aiii"));
            Project project2 = new Project("project2", "2");
            project2.Maps.Add(map2a);

            Map map3a = new Map("map3a");
            map3a.Layers.Add(new Layer(map3a, "layer3ai"));
            Map map4a = new Map("map4a");

            this.ExplorerNodes.Add(new ProjectNodeViewModel(eventAggregator, project1));
            this.ExplorerNodes.Add(new ProjectNodeViewModel(eventAggregator, project2));
            this.ExplorerNodes.Add(new MapNodeViewModel(eventAggregator, map3a));
            this.ExplorerNodes.Add(new MapNodeViewModel(eventAggregator, map4a));

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

        public ObservableCollection<IProjectExplorerNodeViewModel> ExplorerNodes { get; set; }
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
            OpenProjectInteraction interaction = new OpenProjectInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void RefreshTree()
        {
            Console.WriteLine("Refresh Tree");
        }

        private void SelectedItemChanged(object item)
        {
            if (typeof(ProjectNodeViewModel).IsAssignableFrom(item.GetType()))
            {
                ProjectNodeViewModel projectViewModel = item as ProjectNodeViewModel;
                this.CurrentProject.Value = projectViewModel.Project;
                this.session.CurrentProject.Value = this.CurrentProject.Value;
            }
            else if (typeof(MapNodeViewModel).IsAssignableFrom(item.GetType()))
            {
                MapNodeViewModel mapViewModel = item as MapNodeViewModel;
                this.CurrentMap.Value = mapViewModel.Map;
                this.CurrentProject.Value = mapViewModel.Map.Project.Value ?? this.CurrentProject.Value;
                this.session.CurrentProject.Value = this.CurrentProject.Value ?? this.session.CurrentProject.Value;
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
                        this.ExplorerNodes.Add(node);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Project project in e.OldItems)
                    {
                        IEnumerable<ProjectNodeViewModel> projectViewModels = this.ExplorerNodes.OfType<ProjectNodeViewModel>();
                        IEnumerable<ProjectNodeViewModel> toRemove = projectViewModels.Where(entry => entry.Project == project);
                        foreach (ProjectNodeViewModel node in toRemove)
                        {
                            this.ExplorerNodes.Remove(node);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;
                    if (oldIndex != -1 && newIndex != -1)
                    {
                        IProjectExplorerNodeViewModel entry = this.ExplorerNodes[oldIndex];
                        this.ExplorerNodes[oldIndex] = this.ExplorerNodes[newIndex];
                        this.ExplorerNodes[newIndex] = entry;
                    }
                    break;

                default:
                    break;
            }
        }

        private void RefreshProjects()
        {
            this.ExplorerNodes.Clear();
            foreach (Project project in this.session.Projects)
            {
                ProjectNodeViewModel node = new ProjectNodeViewModel(this.eventAggregator, project);
                this.ExplorerNodes.Add(node);
            }
        }

        #endregion methods
    }
}
