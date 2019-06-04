using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ame.App.Wpf.UI
{
    /// <summary>
    /// Interaction logic for WindowManager.xaml
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
