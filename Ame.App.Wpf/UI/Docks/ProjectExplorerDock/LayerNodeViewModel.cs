﻿using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ProjectExplorerDock
{
    // TODO add a layer group node
    public class LayerNodeViewModel : IProjectExplorerNodeViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;

        private bool isDragging;
        private Point startDragPoint;

        #endregion fields


        #region constructor

        public LayerNodeViewModel(IEventAggregator eventAggregator, IActionHandler actionHandler, Layer layer)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.Layer = layer ?? throw new ArgumentNullException("layer is null");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("actionHandler is null");

            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.EditLayerPropertiesCommand = new DelegateCommand(() => EditLayerProperties());
            this.EditTextboxCommand = new DelegateCommand(() => EditTextbox());
            this.StopEditingTextboxCommand = new DelegateCommand(() => StopEditingTextbox());
            this.MouseLeftButtonDownCommand = new DelegateCommand<object>((args) => HandleLeftClickDown((MouseEventArgs)args));
            this.MouseMoveCommand = new DelegateCommand<object>((args) => HandleMouseMove((MouseEventArgs)args));
            this.DropCommand = new DelegateCommand<object>((args) => HandleDropCommand((DragEventArgs)args));
            this.DragOverCommand = new DelegateCommand<object>((args) => HandleDragOverCommand((DragEventArgs)args));
            this.DragEnterCommand = new DelegateCommand<object>((args) => HandleDragEnterCommand((DragEventArgs)args));
            this.DragLeaveCommand = new DelegateCommand<object>((args) => HandleDragLeaveCommand((DragEventArgs)args));
        }

        #endregion constructor


        #region properties

        public ICommand NewLayerCommand { get; private set; }
        public ICommand EditLayerPropertiesCommand { get; private set; }
        public ICommand EditTextboxCommand { get; private set; }
        public ICommand StopEditingTextboxCommand { get; private set; }
        public ICommand MouseLeftButtonDownCommand { get; private set; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand DropCommand { get; private set; }
        public ICommand DragOverCommand { get; private set; }
        public ICommand DragEnterCommand { get; private set; }
        public ICommand DragLeaveCommand { get; private set; }

        public BindableProperty<bool> IsEditingName { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsSelected { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragAbove { get; set; } = BindableProperty<bool>.Prepare(false);
        public BindableProperty<bool> IsDragBelow { get; set; } = BindableProperty<bool>.Prepare(false);

        public ILayer Layer { get; set; }

        #endregion properties


        #region methods

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
                    StartDrag(args);
                }
            }
        }

        private void HandleDropCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.ExplorerLayerNode)))
            {
                ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.ExplorerLayerNode)) as ILayer;
                if (this.IsDragAbove.Value)
                {
                    this.Layer.AddLayerAbove(draggedLayer);
                }
                else if (this.IsDragBelow.Value)
                {
                    this.Layer.AddLayerBelow(draggedLayer);
                }
            }
            this.IsDragAbove.Value = false;
            this.IsDragBelow.Value = false;
            args.Handled = true;
        }

        private void HandleDragOverCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (!data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.LayerListNode)))
            {
                return;
            }
            ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.LayerListNode)) as ILayer;
            if (draggedLayer == this.Layer)
            {
                return;
            }
            DrawSeparator(args);
            args.Handled = true;
        }

        private void HandleDragEnterCommand(DragEventArgs args)
        {
            IDataObject data = args.Data;
            if (!data.GetDataPresent(SerializableNameUtils.GetName(DragDataType.LayerListNode)))
            {
                return;
            }
            ILayer draggedLayer = data.GetData(SerializableNameUtils.GetName(DragDataType.LayerListNode)) as ILayer;
            if (draggedLayer == this.Layer)
            {
                return;
            }
            DrawSeparator(args);
            args.Handled = true;
        }

        private void HandleDragLeaveCommand(DragEventArgs args)
        {
            this.IsDragAbove.Value = false;
            this.IsDragBelow.Value = false;
            args.Handled = true;
        }

        private void DrawSeparator(DragEventArgs args)
        {
            UIElement dragSource = args.Source as UIElement;
            double heightMiddle = dragSource.RenderSize.Height / 2;

            bool isBelow = args.GetPosition(dragSource).Y - heightMiddle > 0;
            this.IsDragAbove.Value = !isBelow;
            this.IsDragBelow.Value = isBelow;
        }

        private void StartDrag(MouseEventArgs args)
        {
            this.isDragging = true;

            DataObject data = new DataObject(SerializableNameUtils.GetName(DragDataType.LayerListNode), this.Layer);
            DependencyObject dragSource = args.Source as DependencyObject;
            DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);

            this.isDragging = false;
        }

        private void NewLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction(this.Layer.Map.Value);
            this.actionHandler.OpenWindow(interaction);
        }

        private void EditLayerProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction(this.Layer);
            this.actionHandler.OpenWindow(interaction);
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
