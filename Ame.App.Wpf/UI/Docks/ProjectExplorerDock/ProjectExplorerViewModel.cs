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
using System.ComponentModel;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    // TODO add layers as a node.
    public class ProjectExplorerViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;
        private IAmeSession session;

        #endregion fields


        #region constructor

        public ProjectExplorerViewModel(IEventAggregator eventAggregator, IActionHandler actionHandler, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("actionHandler is null");

            this.Title.Value = "Project Explorer";

            this.ExplorerNodes = new ObservableCollection<IProjectExplorerNodeViewModel>();

            // TODO add change command when a map without a project is added
            this.session.Projects.CollectionChanged += ProjectsChanged;
            RefreshProjects();

            //Map map1a = new Map("map1a");

            //List<ILayer> map1bLayers = new List<ILayer>();
            //map1bLayers.Add(new Layer("layer1bi"));
            //Map map1b = new Map("map1b", 32, 32, map1bLayers, new List<TilesetModel>());

            //List<ILayer> map1cLayers = new List<ILayer>();
            //map1cLayers.Add(new Layer("layer1ci"));
            //map1cLayers.Add(new Layer("layer1cii"));

            //List<ILayer> map1cGroup1Layers = new List<ILayer>();
            //map1cGroup1Layers.Add(new Layer("layer1cGroup1i"));
            //map1cGroup1Layers.Add(new Layer("layer1cGroup1ii"));
            //LayerGroup group1c = new LayerGroup(null, "layer1cGroup1", map1cGroup1Layers);
            //map1cLayers.Add(group1c);

            //Map map1c = new Map("map1c", 32, 32, map1cLayers, new List<TilesetModel>());

            //List<Map> project1Maps = new List<Map>();
            //project1Maps.Add(map1a);
            //project1Maps.Add(map1b);
            //project1Maps.Add(map1c);
            //Project project1 = new Project("project1", "1", project1Maps);


            //List<ILayer> map2aLayers = new List<ILayer>();
            //map2aLayers.Add(new Layer("layer2ai"));
            //map2aLayers.Add(new Layer("layer2aii"));
            //map2aLayers.Add(new Layer("layer2aiii"));
            //map2aLayers.Add(new LayerGroup("layer2aGroup1"));
            //Map map2a = new Map("map2a", 32, 32, map2aLayers, new List<TilesetModel>());

            //List<Map> project2Maps = new List<Map>();
            //project2Maps.Add(map2a);
            //Project project2 = new Project("project2", "2", project2Maps);

            //List<ILayer> map3aLayers = new List<ILayer>();
            //map3aLayers.Add(new Layer("layer3ai"));
            //Map map3a = new Map("map3a", 32, 32, map3aLayers, new List<TilesetModel>());

            //Map map4a = new Map("map4a");

            //this.ExplorerNodes.Add(new ProjectNodeViewModel(eventAggregator, actionHandler, project1));
            //this.ExplorerNodes.Add(new ProjectNodeViewModel(eventAggregator, actionHandler, project2));
            //this.ExplorerNodes.Add(new MapNodeViewModel(eventAggregator, actionHandler, map3a));
            //this.ExplorerNodes.Add(new MapNodeViewModel(eventAggregator, actionHandler, map4a));


            this.session.CurrentProject.PropertyChanged += ProjectChanged;
            this.session.CurrentMap.PropertyChanged += MapChanged;

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

        private void ProjectChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CurrentProject.Value = GetNewValue(sender, e) as Project;
        }

        private void MapChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CurrentMap.Value = GetNewValue(sender, e) as Map;
        }

        private void SelectedItemChanged(object item)
        {
            if (typeof(ProjectNodeViewModel).IsAssignableFrom(item.GetType()))
            {
                ProjectNodeViewModel projectViewModel = item as ProjectNodeViewModel;
                this.session.CurrentProject.Value = projectViewModel.Project;
            }
            else if (typeof(MapNodeViewModel).IsAssignableFrom(item.GetType()))
            {
                MapNodeViewModel mapViewModel = item as MapNodeViewModel;
                this.session.CurrentMap.Value = mapViewModel.Map;
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
                        ProjectNodeViewModel node = new ProjectNodeViewModel(this.eventAggregator, actionHandler, project);
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
                ProjectNodeViewModel node = new ProjectNodeViewModel(this.eventAggregator, actionHandler, project);
                this.ExplorerNodes.Add(node);
            }
        }

        #endregion methods
    }
}
