using Ame.App.Wpf.UI.Docks;
using Ame.App.Wpf.UI.Docks.ClipboardDock;
using Ame.App.Wpf.UI.Docks.ItemEditorDock;
using Ame.App.Wpf.UI.Docks.ItemListDock;
using Ame.App.Wpf.UI.Docks.LayerListDock;
using Ame.App.Wpf.UI.Docks.MinimapDock;
using Ame.App.Wpf.UI.Docks.ProjectExplorerDock;
using Ame.App.Wpf.UI.Docks.SelectedBrushDock;
using Ame.App.Wpf.UI.Docks.SessionViewerDock;
using Ame.App.Wpf.UI.Docks.ToolboxDock;
using Ame.App.Wpf.UI.Editor.MapEditor;
using Ame.App.Wpf.UI.Serializer;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Ame.Modules.Windows;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ame.App.Wpf.UI
{
    public class WindowManagerViewModel : BindableBase, ILayoutViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private Type[] dockTemplateTypes;

        private event EventHandler ActiveDocumentChanged;

        private AmeSession session;
        private DockCreator dockCreator;

        #endregion fields


        #region constructor

        public WindowManagerViewModel(AmeSession session, DockingManager dockManager, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.session = session;
            this.WindowManager = dockManager;
            this.DockLayout = new DockLayoutViewModel(this, eventAggregator);
            this.eventAggregator = eventAggregator;

            this.Documents = new ObservableCollection<EditorViewModelTemplate>();
            this.Anchorables = new ObservableCollection<DockViewModelTemplate>();

            foreach (Map map in session.MapList)
            {
                MapEditorCreator mapEditorCreator = new MapEditorCreator(this.eventAggregator, this.session);
                DockViewModelTemplate dockViewModel = mapEditorCreator.CreateDock();
                AddDockViewModel(dockViewModel);
            }
            if (this.Documents.Count > 0)
            {
                this.ActiveDocument = this.Documents[0];
            }

            ObservableCollection<ILayer> layerList = null;
            if (this.session.CurrentMap != null)
            {
                layerList = this.session.CurrentMap.LayerList;
            }
            DockCreatorTemplate[] dockCreators = new DockCreatorTemplate[]
            {
                new ClipboardCreator(this.eventAggregator),
                new ItemEditorCreator(this.eventAggregator, this.session),
                new ItemListCreator(this.eventAggregator, this.session),
                new LayerListCreator(this.eventAggregator, this.session),
                new MinimapCreator(this.eventAggregator),
                new SelectedBrushCreator(this.eventAggregator),
                new ProjectExplorerCreator(this.eventAggregator, this.session),
                new SessionViewerCreator(this.eventAggregator, this.session),
                new ToolboxCreator(this.eventAggregator, this.session),
                new MapEditorCreator(this.eventAggregator, this.session)
            };
            this.dockCreator = new DockCreator(dockCreators);

            Type dockTemplateType = typeof(DockViewModelTemplate);
            this.dockTemplateTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                      from assemblyType in domainAssembly.GetTypes()
                                      where typeof(DockViewModelTemplate).IsAssignableFrom(assemblyType)
                                      select assemblyType).ToArray();

            this.eventAggregator.GetEvent<OpenDockEvent>().Subscribe((messge) =>
            {
                OpenDock(messge);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<CloseDockEvent>().Subscribe((messge) =>
            {
                CloseDock(messge);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Subscribe((messge) =>
            {
                OpenWindow(messge);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationActionEvent<string>>().Subscribe(
                SaveLayoutMessageReceived,
                ThreadOption.PublisherThread,
                false,
                (filter) => filter.Notification.Contains(MessageIds.SaveWorkspaceLayout));
            this.eventAggregator.GetEvent<NotificationEvent<string>>().Subscribe(
                LoadLayoutMessageReceived,
                ThreadOption.PublisherThread,
                false,
                (filter) => filter.Notification.Contains(MessageIds.LoadWorkspaceLayout));
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Subscribe((messge) =>
            {
                ViewNotificationReceived(messge);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<ZoomLevel>>().Subscribe((message) =>
            {
                SetZoomLevel(message);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<SaveMessage>>().Subscribe((message) =>
            {
                SaveAs(message);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<OpenMessage>>().Subscribe((message) =>
            {
                OpenMap(message);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<StateMessage>>().Subscribe((message) =>
            {
                ExportAs(message);
            }, ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        public DockingManager WindowManager { get; set; }
        public DockLayoutViewModel DockLayout { get; private set; }

        public ObservableCollection<EditorViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        private bool isBusy;
        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                if (SetProperty(ref isBusy, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (value == false)
                        {
                            Mouse.OverrideCursor = null;
                        }
                        else
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                        }
                    }),
                    DispatcherPriority.Background);
                }
            }
        }

        private DockViewModelTemplate activeDock = null;
        public DockViewModelTemplate ActiveDock
        {
            get { return activeDock; }
            set
            {
                if (SetProperty(ref activeDock, value))
                {
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private EditorViewModelTemplate activeDocument = null;
        public EditorViewModelTemplate ActiveDocument
        {
            get { return activeDocument; }
            set
            {
                if (SetProperty(ref activeDocument, value))
                {
                    if (this.ActiveDocument is MapEditorViewModel)
                    {
                        Map selectedMapContent = this.ActiveDocument.GetContent() as Map;
                        if (!this.session.MapList.Contains(selectedMapContent))
                        {
                            this.session.MapList.Add(selectedMapContent);
                        }
                        this.session.SetCurrentMap(selectedMapContent);
                        Console.WriteLine(this.session.CurrentMap.Name);
                    }
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion properties


        #region methods

        public void CloseApplication(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.session.SerializeFile(Global.SessionFileName);
        }

        private void OpenDock(OpenDockMessage message)
        {
            if (message.IgnoreIfExists)
            {
                if (typeof(DockToolViewModelTemplate).IsAssignableFrom(message.Type))
                {
                    if (this.Anchorables.ToList().Exists((item) => item.GetType() == message.Type))
                    {
                        return;
                    }
                }
                else if (typeof(EditorViewModelTemplate).IsAssignableFrom(message.Type))
                {
                    if (this.Documents.ToList().Exists((item) => item.GetType() == message.Type))
                    {
                        return;
                    }
                }
            }
            DockViewModelTemplate dockViewModel;
            object content = message.Content;
            if (content != null)
            {
                this.dockCreator.UpdateContainer(message.Type, content.GetType(), content);
            }
            dockViewModel = this.dockCreator.CreateDock(message.Type);
            if (!string.IsNullOrEmpty(message.Title))
            {
                dockViewModel.Title = message.Title;
            }
            AddDockViewModel(dockViewModel);
        }

        private void CloseDock(CloseDockMessage message)
        {
            Type dockType = message.Dock.GetType();
            if (typeof(DockToolViewModelTemplate).IsAssignableFrom(dockType))
            {
                this.Anchorables.Remove(message.Dock);
            }
            else if (typeof(EditorViewModelTemplate).IsAssignableFrom(dockType))
            {
                this.Documents.Remove(message.Dock as EditorViewModelTemplate);
            }
        }

        private void OpenWindow(IWindowInteraction interaction)
        {
            string title = interaction.Title;
            interaction.EventAggregator = this.eventAggregator;
            interaction.UpdateMissingContent(this.session);
            interaction.RaiseNotification(this.WindowManager);
        }

        private void SaveLayoutMessageReceived(NotificationActionMessage<string> message)
        {
            string xmlLayoutString = string.Empty;
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(this.WindowManager);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            message.Execute(xmlLayoutString);
        }

        private void LoadLayoutMessageReceived(NotificationMessage<string> message)
        {
            StringReader stringReader = new StringReader(message.Content);
            var layoutSerializer = new XmlLayoutSerializer(this.WindowManager);
            layoutSerializer.LayoutSerializationCallback += UpdateLayout;
            layoutSerializer.Deserialize(stringReader);
        }

        private void UpdateLayout(object sender, LayoutSerializationCallbackEventArgs args)
        {
            Type registeredType = null;
            foreach (Type dockType in this.dockTemplateTypes)
            {
                if (dockType.Name == args.Model.ContentId)
                {
                    registeredType = dockType;
                    break;
                }
            }
            if (registeredType != null)
            {
                DockViewModelTemplate dockViewModel = this.dockCreator.CreateDock(registeredType);
                AddDockViewModel(dockViewModel);
                args.Content = dockViewModel;
            }
        }

        private void AddDockViewModel(DockViewModelTemplate dockViewModel)
        {
            dockViewModel.IsVisible = true;
            if (dockViewModel is DockToolViewModelTemplate)
            {
                this.Anchorables.Add(dockViewModel);
                this.ActiveDock = dockViewModel;
            }
            else if (dockViewModel is EditorViewModelTemplate)
            {
                this.Documents.Add(dockViewModel as EditorViewModelTemplate);
                this.ActiveDocument = dockViewModel as EditorViewModelTemplate;
            }
        }

        private void ViewNotificationReceived(NotificationMessage<ViewNotification> notification)
        {
            switch (notification.Content)
            {
                case ViewNotification.ZoomInDocument:
                    this.ActiveDocument.ZoomIn();
                    break;

                case ViewNotification.ZoomOutDocument:
                    this.ActiveDocument.ZoomOut();
                    break;

                default:
                    break;
            }
        }

        private void SetZoomLevel(NotificationMessage<ZoomLevel> notification)
        {
            this.ActiveDocument.SetZoom(notification.Content);
        }

        private void SaveAs(NotificationMessage<SaveMessage> message)
        {
            SaveMessage content = message.Content;
            content.Map.SerializeFile(content.Path);
        }

        private void OpenMap(NotificationMessage<OpenMessage> message)
        {
            OpenMessage content = message.Content;
            Map.MapJson mapJson = JsonConvert.DeserializeObject<Map.MapJson>(File.ReadAllText(content.Path));
            Map importedMap = mapJson.Generate();

            OpenDockMessage openEditorMessage = new OpenDockMessage(typeof(MapEditorViewModel), importedMap);
            foreach (TilesetModel tileset in importedMap.TilesetList)
            {
                this.session.CurrentTilesetList.Add(tileset);
            }
            this.session.CurrentMap = importedMap;
            CollectionViewSource.GetDefaultView(this.session.CurrentLayerList).Refresh();
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(openEditorMessage);
        }

        private void ExportAs(NotificationMessage<StateMessage> message)
        {
            StateMessage content = message.Content;
            this.ActiveDocument.ExportAs(content.Path, content.Encoder);
        }

        #endregion methods
    }
}
