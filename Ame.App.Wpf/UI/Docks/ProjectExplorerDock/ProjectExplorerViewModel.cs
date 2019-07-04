using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
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
            
            this.NewProjectCommand = new DelegateCommand(() => NewProject());
            this.OpenProjectCommand = new DelegateCommand(() => OpenProject());
            this.ViewProjectPropertiesCommand = new DelegateCommand(() => ViewProjectProperties());
            this.RefreshTreeCommand = new DelegateCommand(() => RefreshTree());
        }

        #endregion constructor


        #region properties

        public ICommand NewProjectCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }
        public ICommand ViewProjectPropertiesCommand { get; set; }
        public ICommand RefreshTreeCommand { get; set; }

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
            Console.WriteLine("Adding new project");
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
        }

        #endregion methods
    }
}
