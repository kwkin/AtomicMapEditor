using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtomicMapEditor.Infrastructure.BaseTypes;
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

            this.Documents.Add(new MainEditorViewModel());
        }

        #endregion constructor & destructer


        #region properties

        public ObservableCollection<EditorViewModelTemplate> Documents { get; private set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
