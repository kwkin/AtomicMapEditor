﻿using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
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

namespace Ame.App.Wpf.UI.Docks.SessionViewerDock
{
    public class SessionViewerViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IAmeSession session;

        #endregion fields


        #region constructor

        public SessionViewerViewModel(IEventAggregator eventAggregator, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");

            this.Title.Value = "Session Viewer";

            this.Nodes = new ObservableCollection<NodeViewBuilder>();
            this.Nodes.Add(new NodeViewBuilder("Ame Session", this.session));

            this.RefreshTreeCommand = new DelegateCommand(() => RefreshTree());
        }

        #endregion constructor


        #region properties

        public ICommand RefreshTreeCommand { get; set; }

        public ObservableCollection<NodeViewBuilder> Nodes { get; set; }

        #endregion properties


        #region methods

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void RefreshTree()
        {
            this.Nodes = new ObservableCollection<NodeViewBuilder>();
            this.Nodes.Add(new NodeViewBuilder("Ame Session", this.session));
        }

        #endregion methods
    }
}
