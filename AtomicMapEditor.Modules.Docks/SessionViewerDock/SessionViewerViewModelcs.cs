using System.Collections.ObjectModel;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Docks.SessionViewerDock
{
    public class SessionViewerViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region Constructor & destructor

        public SessionViewerViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator;
            this.session = session;
            this.Title = "Session Viewer";

            this.RefreshTreeCommand = new DelegateCommand(() => RefreshTree());

            this.Nodes = new ObservableCollection<NodeViewBuilder>();
            this.Nodes.Add(new NodeViewBuilder("Ame Session", this.session));
        }

        #endregion Constructor & destructor


        #region properties

        public ICommand RefreshTreeCommand { get; set; }

        public ObservableCollection<NodeViewBuilder> Nodes { get; set; }

        public override DockType DockType
        {
            get
            {
                return DockType.SessionView;
            }
        }

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
