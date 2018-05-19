using System.Windows.Controls;

namespace Ame.Modules.Windows
{
    /// <summary>
    /// Interaction logic for DockManager.xaml
    /// </summary>
    public partial class WindowManager : UserControl
    {
        public WindowManager()
        {
            InitializeComponent();

            WindowManagerViewModel viewModel = this.DataContext as WindowManagerViewModel;
            viewModel.WindowManager = this.windowManager;
        }
    }
}
