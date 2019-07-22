using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
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
    public class OpenMapInteraction : IWindowInteraction
    {
        #region fields

        private AmeSession session;

        #endregion fields


        #region constructor

        public OpenMapInteraction()
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

        public void UpdateMissingContent(AmeSession session)
        {
            this.session = session;
        }

        public void RaiseNotification(DependencyObject parent)
        {
            OpenFileDialog openMapDialog = new OpenFileDialog();
            openMapDialog.Title = "Open Map";
            openMapDialog.Filter = SaveMapExtension.GetOpenMapSaveExtensions();
            openMapDialog.InitialDirectory = this.session.LastMapDirectory.Value;
            if (openMapDialog.ShowDialog().Value)
            {
                string file = openMapDialog.FileName;
                if (File.Exists(file))
                {
                    MapJsonReader reader = new MapJsonReader();
                    ResourceLoader loader = ResourceLoader.Instance;
                    Map importedMap = loader.Load<Map>(file, reader);

                    this.session.LastMapDirectory.Value = Directory.GetParent(file).FullName;

                    OpenMapMessage message = new OpenMapMessage(importedMap);
                    NotificationMessage<OpenMapMessage> notification = new NotificationMessage<OpenMapMessage>(message);
                    this.EventAggregator.GetEvent<NotificationEvent<OpenMapMessage>>().Publish(notification);
                }
            }
        }

        #endregion methods
    }
}
