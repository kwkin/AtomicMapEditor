using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ProjectExplorerDock
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
            this.eventAggregator = eventAggregator;
            this.session = session;
            this.Title = "Project Explorer";

            this.RefreshTreeCommand = new DelegateCommand(() => RefreshTree());

            this.Nodes = new ObservableCollection<NodeViewBuilder>();
            this.Nodes.Add(new NodeViewBuilder("Ame Session", this.session));
        }

        #endregion constructor


        #region properties

        public ICommand RefreshTreeCommand { get; set; }

        public ObservableCollection<NodeViewBuilder> Nodes { get; set; }

        #endregion properties


        #region methods

        public void RefreshTree()
        {
            this.Nodes = new ObservableCollection<NodeViewBuilder>();
            this.Nodes.Add(new NodeViewBuilder("Ame Session", this.session));
            RaisePropertyChanged(nameof(this.Nodes));
        }

        #endregion methods
    }
}
