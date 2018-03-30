﻿using System.Collections.ObjectModel;
using AtomicMapEditor.Infrastructure.BaseTypes;
using AtomicMapEditor.Infrastructure.Events;
using AtomicMapEditor.Modules.Docks.ClipboardDock;
using AtomicMapEditor.Modules.Docks.ItemEditorDock;
using AtomicMapEditor.Modules.Docks.ItemListDock;
using AtomicMapEditor.Modules.Docks.LayerListDock;
using AtomicMapEditor.Modules.Docks.ToolboxDock;
using AtomicMapEditor.Modules.MapEditor.Editor;
using Prism.Events;
using Prism.Mvvm;
using Xceed.Wpf.AvalonDock;

namespace AtomicMapEditor.Modules.Docks.Core
{
    public class DockManagerViewModel : BindableBase
    {
        #region fields

        private DockingManager dockManager;

        private IEventAggregator ea;

        #endregion fields


        #region constructor & destructer

        public DockManagerViewModel(DockingManager dockManager, IEventAggregator eventAggregator)
        {
            this.dockManager = dockManager;
            this.ea = eventAggregator;

            this.Documents = new ObservableCollection<EditorViewModelTemplate>();
            this.Anchorables = new ObservableCollection<DockViewModelTemplate>();

            this.Documents.Add(new MainEditorViewModel());

            this.ea.GetEvent<OpenDockEvent>().Subscribe(OpenDock, ThreadOption.UIThread);
        }

        #endregion constructor & destructer


        #region properties

        public ObservableCollection<EditorViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        #endregion properties


        #region methods

        private void OpenDock(OpenDockMessage message)
        {
            DockViewModelTemplate dockViewModel = null;
            switch (message.DockType)
            {
                case DockType.Clipboard:
                    dockViewModel = new ClipboardViewModel();
                    break;

                case DockType.ItemEditor:
                    dockViewModel = new ItemEditorViewModel();
                    break;

                case DockType.ItemList:
                    dockViewModel = new ItemListViewModel();
                    break;

                case DockType.LayerList:
                    dockViewModel = new LayerListViewModel();
                    break;

                case DockType.Toolbox:
                    dockViewModel = new ToolboxViewModel();
                    break;

                default:
                    break;
            }

            if (dockViewModel == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(message.DockTitle))
            {
                dockViewModel.Title = message.DockTitle;
            }
            this.Anchorables.Add(dockViewModel);

            // TODO: add active document
        }

        #endregion methods
    }
}
