using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Xceed.Wpf.AvalonDock;

namespace AtomicMapEditor.Modules.Docks.Core
{
    /// <summary>
    /// Interaction logic for DockManager.xaml
    /// </summary>
    public partial class DockManager : UserControl
    {
        public DockManager()
        {
            InitializeComponent();

            DockManagerViewModel viewModel = this.DataContext as DockManagerViewModel;
            viewModel.DockManager = this.dockManager;
        }
    }
}
