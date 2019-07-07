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
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
using Ame.Infrastructure.Models.Serializer.Json.Data;
using Ame.Infrastructure.UILogic;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class WindowManagerViewModel : ILayoutViewModel
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

            foreach (Map map in session.Maps)
            {
                MapEditorCreator mapEditorCreator = new MapEditorCreator(this.eventAggregator, this.session);
                DockViewModelTemplate dockViewModel = mapEditorCreator.CreateDock();
                AddDockViewModel(dockViewModel);
            }
            if (this.Documents.Count > 0)
            {
                this.ActiveDocument.Value = this.Documents[0];
            }

            ObservableCollection<ILayer> layers = null;
            if (this.session.CurrentMap != null)
            {
                layers = this.session.CurrentMap.Layers;
            }
            DockCreatorTemplate[] dockCreators = new DockCreatorTemplate[]
            {
                new ClipboardCreator(this.eventAggregator),
                new ItemEditorCreator(this.eventAggregator, this.session),
                new ItemListCreator(this.eventAggregator, this.session),
                new LayerListCreator(this.eventAggregator, this.session),
                new MinimapCreator(this.eventAggregator, this.session),
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

            this.IsBusy.PropertyChanged += IsBusyChanged;
            this.ActiveDock.PropertyChanged += ActiveDockPropertyChanged;
            this.ActiveDocument.PropertyChanged += ActiveDocumentPropertyChanged;


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
            this.eventAggregator.GetEvent<NotificationEvent<OpenMapMessage>>().Subscribe((message) =>
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

        public BindableProperty<bool> IsBusy { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<DockViewModelTemplate> ActiveDock { get; set; } = BindableProperty<DockViewModelTemplate>.Prepare();

        public BindableProperty<EditorViewModelTemplate> ActiveDocument { get; set; } = BindableProperty<EditorViewModelTemplate>.Prepare();

        #endregion properties


        #region methods

        public void CloseApplication(object sender, CancelEventArgs e)
        {
            AmeSessionWriter writer = new AmeSessionWriter();
            writer.Write(this.session, Global.SessionFileName);
        }

        private void IsBusyChanged(object sender, PropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.IsBusy.Value == false)
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

        private void ActiveDockPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ActiveDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ActiveDocument.Value is MapEditorViewModel)
            {
                Map selectedMapContent = this.ActiveDocument.Value.GetContent() as Map;
                if (!this.session.Maps.Contains(selectedMapContent))
                {
                    this.session.Maps.Add(selectedMapContent);
                }
                this.session.SetCurrentMap(selectedMapContent);
            }
            ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
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
                dockViewModel.Title.Value = message.Title;
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
            dockViewModel.IsVisible.Value = true;
            if (dockViewModel is DockToolViewModelTemplate)
            {
                this.Anchorables.Add(dockViewModel);
                this.ActiveDock.Value = dockViewModel;
            }
            else if (dockViewModel is EditorViewModelTemplate)
            {
                this.Documents.Add(dockViewModel as EditorViewModelTemplate);
                this.ActiveDocument.Value = dockViewModel as EditorViewModelTemplate;
            }
        }

        private void ViewNotificationReceived(NotificationMessage<ViewNotification> notification)
        {
            switch (notification.Content)
            {
                case ViewNotification.ZoomInDocument:
                    this.ActiveDocument.Value.ZoomIn();
                    break;

                case ViewNotification.ZoomOutDocument:
                    this.ActiveDocument.Value.ZoomOut();
                    break;

                default:
                    break;
            }
        }

        private void SetZoomLevel(NotificationMessage<ZoomLevel> notification)
        {
            this.ActiveDocument.Value.SetZoom(notification.Content);
        }

        private void SaveAs(NotificationMessage<SaveMessage> message)
        {
            SaveMessage content = message.Content;
            content.Map.SerializeFile(content.Path);
        }

        private void OpenMap(NotificationMessage<OpenMapMessage> message)
        {
            OpenMapMessage content = message.Content;
            Map importedMap = content.Map;

            OpenDockMessage openEditorMessage = new OpenDockMessage(typeof(MapEditorViewModel), importedMap);
            foreach (TilesetModel tileset in importedMap.Tilesets)
            {
                this.session.CurrentTilesets.Add(tileset);
            }
            this.session.CurrentMap = importedMap;
            CollectionViewSource.GetDefaultView(this.session.CurrentLayers).Refresh();
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(openEditorMessage);
        }

        private void ExportAs(NotificationMessage<StateMessage> message)
        {
            StateMessage content = message.Content;
            this.ActiveDocument.Value.ExportAs(content.Path, content.Encoder);
        }

        #endregion methods
    }
}
