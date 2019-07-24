using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Microsoft.Win32;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ame.App.Wpf.UI.Interactions.FileChooser
{
    class SaveMapInteraction : IWindowInteraction
    {
        #region fields

        private IAmeSession session;
        private Map map;

        #endregion fields


        #region constructor

        public SaveMapInteraction()
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
            SaveFileDialog saveMapDialog = new SaveFileDialog();
            saveMapDialog.Title = "Save Map";
            saveMapDialog.Filter = SaveMapExtension.GetOpenMapSaveExtensions();
            saveMapDialog.InitialDirectory = this.session.LastMapDirectory.Value;
            if (saveMapDialog.ShowDialog().Value)
            {
                string file = saveMapDialog.FileName;
                map.WriteFile(file);
                this.session.LastMapDirectory.Value = Directory.GetParent(file).FullName;
            }
        }

        #endregion methods
    }
}
