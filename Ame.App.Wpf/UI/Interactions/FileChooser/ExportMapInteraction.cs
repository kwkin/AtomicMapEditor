using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Microsoft.Win32;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Ame.App.Wpf.UI.Interactions.FileChooser
{
    public class ExportMapInteraction : IWindowInteraction
    {
        #region fields

        private IAmeSession session;
        private Map map;

        #endregion fields


        #region constructor

        public ExportMapInteraction()
        {
        }

        #endregion constructor


        #region properties

        public ILayer Layer { get; set; }
        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        #endregion properties


        #region methods

        public void UpdateMissingContent(IAmeSession session)
        {
            this.session = session;
            this.map = session.CurrentMap.Value;
        }

        public void RaiseNotification(DependencyObject parent)
        {
            SaveFileDialog exportMapDialog = new SaveFileDialog();
            exportMapDialog.Title = "Export Map";
            exportMapDialog.Filter = ExportMapExtension.GetOpenFileExportMapExtensions();
            exportMapDialog.InitialDirectory = this.session.LastMapDirectory.Value;
            exportMapDialog.FileName = this.map.Name.Value;
            if (exportMapDialog.ShowDialog().Value)
            {
                BitmapEncoder encoder = ExportMapExtension.getEncoder(exportMapDialog.Filter);
                this.map.ExportAs(exportMapDialog.FileName, encoder);
            }
        }

        #endregion methods
    }
}
