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
    public class ProjectNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public ProjectNodeViewModel(IEventAggregator eventAggregator, Project project)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.Project = project ?? throw new ArgumentNullException("layer");
                       
            this.MapNodes = new ObservableCollection<MapNodeViewModel>();
            foreach(Map map in project.Maps)
            {
                this.MapNodes.Add(new MapNodeViewModel(this.eventAggregator, map));
            }
            this.Project.Maps.CollectionChanged += MapsChanged;

            this.NewMapCommand = new DelegateCommand(() => NewMap());
        }

        #endregion constructor


        #region properties
        public ICommand NewMapCommand { get; private set; }

        public Project Project { get; set; }

        public ObservableCollection<MapNodeViewModel> MapNodes { get; private set; }

        #endregion properties


        #region methods

        private void MapsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Map map in e.NewItems)
                    {
                        MapNodeViewModel node = new MapNodeViewModel(this.eventAggregator, map);
                        this.MapNodes.Add(node);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Map map in e.OldItems)
                    {
                        IEnumerable<MapNodeViewModel> toRemove = new ObservableCollection<MapNodeViewModel>(this.MapNodes.Where(entry => entry.Map == map));
                        foreach (MapNodeViewModel node in toRemove)
                        {
                            this.MapNodes.Remove(node);
                        }
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
            Console.WriteLine("New Map");
        }

        #endregion methods
    }
}
