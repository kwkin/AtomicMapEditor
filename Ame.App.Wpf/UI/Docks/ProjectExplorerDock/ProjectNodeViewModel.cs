using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.App.Wpf.UI.Interactions.ProjectProperties;
using Ame.Infrastructure.Attributes;
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
using System.Windows;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    public class ProjectNodeViewModel : IProjectExplorerNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;

        private bool isDragging;
        private Point startDragPoint;

        #endregion fields


        #region constructor

        public ProjectNodeViewModel(IEventAggregator eventAggregator, IActionHandler actionHandler, Project project)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.project = project ?? throw new ArgumentNullException("layer is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("actionHandler is null");

            this.MapNodes = new ObservableCollection<MapNodeViewModel>();
            this.Project = project;

            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.EditProjectPropertiesCommand = new DelegateCommand(() => EditProjectProperties());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
            this.MouseLeftButtonDownCommand = new DelegateCommand<object>((point) => HandleLeftClickDown((MouseEventArgs)point));
            this.MouseMoveCommand = new DelegateCommand<object>((point) => HandleMouseMove((MouseEventArgs)point));
            this.DropCommand = new DelegateCommand<object>((point) => HandleDropCommand((DragEventArgs)point));
            this.DragOverCommand = new DelegateCommand<object>((args) => HandleDragOverCommand((DragEventArgs)args));
            this.DragEnterCommand = new DelegateCommand<object>((args) => HandleDragEnterCommand((DragEventArgs)args));
            this.DragLeaveCommand = new DelegateCommand<object>((args) => HandleDragLeaveCommand((DragEventArgs)args));
        }

        #endregion constructor


        #region properties

        public ICommand NewMapCommand { get; private set; }
        public ICommand EditProjectPropertiesCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }
        public ICommand MouseLeftButtonDownCommand { get; private set; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand DropCommand { get; private set; }
        public ICommand DragOverCommand { get; private set; }
        public ICommand DragEnterCommand { get; private set; }
        public ICommand DragLeaveCommand { get; private set; }

        private Project project;
        public Project Project
        {
            get
            {
                return this.project;
            }
            set
            {
                LoadProject(value);
            }
        }

        public ObservableCollection<MapNodeViewModel> MapNodes { get; private set; }
        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragAbove { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragOnto { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragBelow { get; set; } = BindableProperty<bool>.Prepare(false);

        #endregion properties


        #region methods

        public void AddMap(Map map)
        {
            MapNodeViewModel node = new MapNodeViewModel(this.eventAggregator, this.actionHandler, map);
            this.MapNodes.Add(node);
        }

        public void RemoveMap(Map map)
        {
            MapNodeViewModel toRemove = this.MapNodes.FirstOrDefault(entry => entry.Map == map);
            this.MapNodes.Remove(toRemove);
        }

        private void LoadProject(Project project)
        {
            if (this.project != null)
            {
                this.Project.Maps.CollectionChanged -= MapsChanged;
            }
            this.project = project;

            this.MapNodes.Clear();
            this.Project.Maps.ToList().ForEach(map => AddMap(map));
            this.Project.Maps.CollectionChanged += MapsChanged;
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

        private void HandleLeftClickDown(MouseEventArgs args)
        {
            this.startDragPoint = args.GetPosition(null);
        }

        private void HandleMouseMove(MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed && !this.isDragging)
            {
                Point position = args.GetPosition(null);
                if (Math.Abs(position.X - this.startDragPoint.X) > SystemParameters.MinimumHorizontalDragDistance
                        || Math.Abs(position.Y - this.startDragPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    HandleStartDrag(args);
                }
            }
        }

        private void HandleDropCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.ExplorerMapNode)))
            {
                Map draggedMap = data.GetData(SerializableNameUtils.GetName(DragDataType.ExplorerMapNode)) as Map;
                this.Project.Maps.Insert(0, draggedMap);
            }
            this.IsDragAbove.Value = false;
            this.IsDragOnto.Value = false;
            this.IsDragBelow.Value = false;
            args.Handled = true;
        }

        private void HandleDragOverCommand(DragEventArgs args)
        {
            DrawSeparator(args);
            args.Handled = true;
        }

        private void HandleDragEnterCommand(DragEventArgs args)
        {
            DrawSeparator(args);
            args.Handled = true;
        }

        private void HandleDragLeaveCommand(DragEventArgs args)
        {
            this.IsDragAbove.Value = false;
            this.IsDragBelow.Value = false;
            args.Handled = true;
        }

        private void HandleStartDrag(MouseEventArgs args)
        {
            this.isDragging = true;

            DataObject data = new DataObject(typeof(Map).ToString(), this.Project);
            DependencyObject dragSource = args.Source as DependencyObject;
            DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);

            this.isDragging = false;
        }

        private void DrawSeparator(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (data.GetDataPresent(typeof(ILayer).ToString()))
            {
                ILayer draggedLayer = data.GetData(typeof(ILayer).ToString()) as ILayer;
            }
            UIElement dragSource = args.Source as UIElement;
            double aboveHeight = 1 * dragSource.RenderSize.Height / 3;
            double belowHeight = 2 * dragSource.RenderSize.Height / 3;
            if (aboveHeight - args.GetPosition(dragSource).Y > 0)
            {
                this.IsDragAbove.Value = true;
                this.IsDragOnto.Value = false;
                this.IsDragBelow.Value = false;
            }
            else if (belowHeight - args.GetPosition(dragSource).Y > 0 || this.MapNodes.Count != 0)
            {
                this.IsDragAbove.Value = false;
                this.IsDragOnto.Value = true;
                this.IsDragBelow.Value = false;
            }
            else
            {
                this.IsDragAbove.Value = false;
                this.IsDragOnto.Value = false;
                this.IsDragBelow.Value = true;
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
