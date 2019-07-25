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
using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.App.Wpf.UI.Interactions.ProjectProperties;
using Ame.App.Wpf.UI.Serializer;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
using Ame.Infrastructure.UILogic;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Ame.App.Wpf.UI.Interactions.FileChooser;

namespace Ame.App.Wpf.UI
{
    public class WindowManagerViewModel : ILayoutViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IActionHandler actionHandler;
        private IAmeSession session;

        private IConstants constants;
        private Type[] dockTemplateTypes;

        private event EventHandler ActiveDocumentChanged;

        private DockCreator dockCreator;

        #endregion fields


        #region constructor

        public WindowManagerViewModel(IEventAggregator eventAggregator, IConstants constants, IAmeSession session, IActionHandler actionHandler, DockingManager dockManager)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.constants = constants ?? throw new ArgumentNullException("constants");
            this.session = session ?? throw new ArgumentNullException("constants");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("actionHandler");

            this.WindowManager = dockManager;
            this.DockLayout = new DockLayoutViewModel(this, constants, eventAggregator);

            this.Documents = new ObservableCollection<EditorViewModelTemplate>();
            this.Anchorables = new ObservableCollection<DockViewModelTemplate>();

            foreach (Map map in session.Maps)
            {
                MapEditorCreator mapEditorCreator = new MapEditorCreator(this.eventAggregator, constants, this.session);
                DockViewModelTemplate dockViewModel = mapEditorCreator.CreateDock();
                AddDockViewModel(dockViewModel);
            }
            if (this.Documents.Count > 0)
            {
                this.ActiveDocument.Value = this.Documents[0];
            }

            DockCreatorTemplate[] dockCreators = new DockCreatorTemplate[]
            {
                new ClipboardCreator(this.eventAggregator),
                new ItemEditorCreator(this.eventAggregator, constants, this.session),
                new ItemListCreator(this.eventAggregator, this.session),
                new LayerListCreator(this.eventAggregator, this.session),
                new MinimapCreator(this.eventAggregator, this.session),
                new SelectedBrushCreator(this.eventAggregator, constants),
                new ProjectExplorerCreator(this.eventAggregator, this.session),
                new SessionViewerCreator(this.eventAggregator, this.session),
                new ToolboxCreator(this.eventAggregator, this.session),
                new MapEditorCreator(this.eventAggregator, constants, this.session)
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

            Application.Current.MainWindow.Closing += CloseApplication;

            this.NewProjectCommand = new DelegateCommand(() => NewProject());
            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.OpenProjectCommand = new DelegateCommand(() => this.actionHandler.OpenProject());
            this.OpenMapCommand = new DelegateCommand(() => OpenMap());
            this.SaveFileCommand = new DelegateCommand(() => this.actionHandler.SaveCurrentMap());
            this.SaveAsFileCommand = new DelegateCommand(() => SaveAsMap());
            this.ExportFileCommand = new DelegateCommand(() => this.actionHandler.ExportFile());
            this.ExportAsFileCommand = new DelegateCommand(() => ExportAsMap());
            this.CloseFileCommand = new DelegateCommand(() => this.actionHandler.CloseFile());
            this.UndoCommand = new DelegateCommand(() => this.actionHandler.Undo());
            this.RedoCommand = new DelegateCommand(() => this.actionHandler.Redo());
            this.CutCommand = new DelegateCommand(() => this.actionHandler.CutSelection());
            this.CopyCommand = new DelegateCommand(() => this.actionHandler.CopySelection());
            this.PasteCommand = new DelegateCommand(() => this.actionHandler.PasteClipboard());
            this.SampleViewCommand = new DelegateCommand(() => this.actionHandler.SampleView());
            this.ZoomInCommand = new DelegateCommand(() => this.actionHandler.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => this.actionHandler.ZoomOut());
            this.FitMapToWindowCommand = new DelegateCommand(() => this.actionHandler.FitMapToWindow());

            this.eventAggregator.GetEvent<OpenDockEvent>().Subscribe((message) =>
            {
                OpenDock(message);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<CloseDockEvent>().Subscribe((message) =>
            {
                CloseDock(message);
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<CloseApplicationEvent>().Subscribe((message) =>
            {
                CloseApplication();
            }, ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Subscribe((message) =>
            {
                OpenWindow(message);
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
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Subscribe((message) =>
            {
                ViewNotificationReceived(message);
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

        public ICommand WindowClosingCommand { get; private set; }
        public ICommand NewProjectCommand { get; private set; }
        public ICommand NewMapCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand OpenMapCommand { get; private set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand SaveAsFileCommand { get; private set; }
        public ICommand ExportFileCommand { get; private set; }
        public ICommand ExportAsFileCommand { get; private set; }
        public ICommand CloseFileCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand SampleViewCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand FitMapToWindowCommand { get; private set; }

        public IActionHandler ActionHandler { get; set; }

        public DockingManager WindowManager { get; set; }
        public DockLayoutViewModel DockLayout { get; private set; }

        public ObservableCollection<EditorViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        public BindableProperty<bool> IsBusy { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<DockViewModelTemplate> ActiveDock { get; set; } = BindableProperty<DockViewModelTemplate>.Prepare();

        public BindableProperty<EditorViewModelTemplate> ActiveDocument { get; set; } = BindableProperty<EditorViewModelTemplate>.Prepare();

        #endregion properties


        #region methods

        private void CloseApplication(object sender, CancelEventArgs e)
        {
            IAmeSessionJsonWriter writer = new IAmeSessionJsonWriter();
            writer.Write(this.session, this.constants.SessionFileName);
        }

        private void CloseApplication()
        {
            Application.Current.MainWindow.Close();
        }

        private void NewProject()
        {
            NewProjectInteraction interaction = new NewProjectInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        private void NewMap()
        {
            NewMapInteraction interaction = new NewMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }
        public void OpenMap()
        {
            OpenMapInteraction interaction = new OpenMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void SaveAsMap()
        {
            SaveMapInteraction interaction = new SaveMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void ExportAsMap()
        {
            ExportMapInteraction interaction = new ExportMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        private void IsBusyChanged(object sender, PropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Mouse.OverrideCursor = this.IsBusy.Value ? Cursors.Wait : null;
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
            content.Map.WriteFile(content.Path);
            content.Map.Project.Value.UpdateFile();
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
            this.session.CurrentMap.Value = importedMap;
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
