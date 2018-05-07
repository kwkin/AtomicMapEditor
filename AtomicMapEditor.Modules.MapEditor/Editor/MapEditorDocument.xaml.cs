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

namespace Ame.Modules.MapEditor.Editor
{
    /// <summary>
    /// Interaction logic for MapEditorDocument.xaml
    /// </summary>
    public partial class MapEditorDocument : UserControl
    {
        public MapEditorDocument()
        {
            InitializeComponent();
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
        }
    }
}
