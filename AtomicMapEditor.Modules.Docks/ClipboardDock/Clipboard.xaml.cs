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

namespace Ame.Modules.Docks.ClipboardDock
{
    /// <summary>
    /// Interaction logic for Clipboard.xaml
    /// </summary>
    public partial class Clipboard : UserControl
    {
        public Clipboard()
        {
            InitializeComponent();
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

            if (this.DataContext == null)
            {
                Console.WriteLine("Null bois");
            }
            else
            {
                Console.WriteLine("Not null");
            }
        }
    }
}
