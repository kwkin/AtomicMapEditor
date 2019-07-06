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
            this.ViewProjectPropertiesCommand = new DelegateCommand(() => ViewProjectProperties());
            this.RefreshTreeCommand = new DelegateCommand(() => RefreshTree());
            this.EditProjectPropertiesCommand = new DelegateCommand(() => EditProjectProperties());
        }

        #endregion constructor


        #region properties

        public ICommand NewProjectCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }
        public ICommand ViewProjectPropertiesCommand { get; set; }
        public ICommand RefreshTreeCommand { get; set; }
        public ICommand EditProjectPropertiesCommand { get; private set; }
        public ICommand EditMapPropertiesCommand { get; private set; }

        public ObservableCollection<ProjectNodeViewModel> ProjectNodes { get; set; }

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

        public void ViewProjectProperties()
        {
            Console.WriteLine("View project properties");
        }

        public void RefreshTree()
        {
            Console.WriteLine("Refresh Drop");
        }

        private void EditProjectProperties()
        {
            Console.WriteLine("Edit Project Properties");
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
