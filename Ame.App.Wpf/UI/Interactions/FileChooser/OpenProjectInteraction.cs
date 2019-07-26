using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
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
    public class OpenProjectInteraction : IWindowInteraction
    {
        #region fields

        private IAmeSession session;

        #endregion fields


        #region constructor

        public OpenProjectInteraction()
        {
        }

        #endregion constructor


        #region properties

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
        }

        public void RaiseNotification(DependencyObject parent)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Open a Project";
            openDialog.InitialDirectory = this.session.LastTilesetDirectory.Value;
            if (openDialog.ShowDialog().Value)
            {
                string dialogPath = openDialog.FileName;
                if (File.Exists(dialogPath))
                {
                    ProjectJsonReader reader = new ProjectJsonReader();
                    ResourceLoader loader = ResourceLoader.Instance;
                    Project project = loader.Load<Project>(dialogPath, reader);

                    this.session.Projects.Add(project);
                }
            }
        }

        #endregion methods
    }
}
