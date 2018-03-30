using System.Collections.ObjectModel;
using AtomicMapEditor.Infrastructure.BaseTypes;
using AtomicMapEditor.Modules.Docks.ItemEditorDock;
using AtomicMapEditor.Modules.MapEditor.Editor;
using Prism.Mvvm;
using Xceed.Wpf.AvalonDock;

namespace AtomicMapEditor.Modules.Docks.Core
{
    public class DockManagerViewModel : BindableBase
    {
        #region fields

        private DockingManager dockManager;


        #endregion fields


        #region constructor & destructer

        public DockManagerViewModel(DockingManager dockManager)
        {
            this.dockManager = dockManager;

            this.Documents = new ObservableCollection<EditorViewModelTemplate>();
            this.Anchorables = new ObservableCollection<DockViewModelTemplate>();

            this.Documents.Add(new MainEditorViewModel());
            this.Anchorables.Add(new ItemEditorViewModel());
        }

        #endregion constructor & destructer


        #region properties

        public ObservableCollection<EditorViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
