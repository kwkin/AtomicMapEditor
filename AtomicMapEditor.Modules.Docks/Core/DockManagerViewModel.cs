using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using AtomicMapEditor.Infrastructure.BaseTypes;
using AtomicMapEditor.Infrastructure.Core;
using AtomicMapEditor.Infrastructure.Events;
using AtomicMapEditor.Infrastructure.Requests;
using AtomicMapEditor.Modules.Docks.ClipboardDock;
using AtomicMapEditor.Modules.Docks.ItemEditorDock;
using AtomicMapEditor.Modules.Docks.ItemListDock;
using AtomicMapEditor.Modules.Docks.LayerListDock;
using AtomicMapEditor.Modules.Docks.MinimapDock;
using AtomicMapEditor.Modules.Docks.SelectedBrushDock;
using AtomicMapEditor.Modules.Docks.ToolboxDock;
using AtomicMapEditor.Modules.MapEditor.Editor;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace AtomicMapEditor.Modules.Docks.Core
{
    public class DockManagerViewModel : BindableBase, ILayoutViewModel
    {
        #region fields

        // TODO add this in a config file
        public static string applicationName = "AtomicMapEditor";

        private IEventAggregator ea;

        #endregion fields


        #region constructor & destructer

        public DockManagerViewModel(DockingManager dockManager, IEventAggregator eventAggregator)
        {
            this.DockManager = dockManager;
            this.DockLayout = new DockLayoutViewModel(this, eventAggregator);
            this.ea = eventAggregator;

            this.Documents = new ObservableCollection<EditorViewModelTemplate>();
            this.Anchorables = new ObservableCollection<DockViewModelTemplate>();

            this._MapWindowInteraction = new InteractionRequest<INotification>();

            // TODO add filter to dock and window open messages
            this.ea.GetEvent<OpenDockEvent>().Subscribe(
                OpenDock,
                ThreadOption.PublisherThread);
            this.ea.GetEvent<OpenWindowEvent>().Subscribe(
                OpenWindow,
                ThreadOption.PublisherThread);
            this.ea.GetEvent<NotificationActionEvent<string>>().Subscribe(
                SaveLayoutMessageReceived,
                ThreadOption.PublisherThread,
                false,
                (filter) => filter.Notification.Contains(MessageIds.SaveWorkspaceLayout));
            this.ea.GetEvent<NotificationEvent<string>>().Subscribe(
                LoadLayoutMessageReceived,
                ThreadOption.PublisherThread,
                false,
                (filter) => filter.Notification.Contains(MessageIds.LoadWorkspaceLayout));
        }

        #endregion constructor & destructer


        #region properties

        public DockingManager DockManager { get; set; }
        public DockLayoutViewModel DockLayout { get; private set; }
        public ObservableCollection<EditorViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return this._IsBusy; }
            set
            {
                if (SetProperty(ref _IsBusy, value))
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

        public string AppDataDirectory
        {
            get
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string directoryPath = Path.Combine(documentPath, applicationName);
                try
                {
                    if (Directory.Exists(directoryPath) == false)
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }
                catch
                {
                    directoryPath = string.Empty;
                }
                return directoryPath;
            }
        }

        private InteractionRequest<INotification> _MapWindowInteraction;
        public IInteractionRequest MapWindowInteraction
        {
            get { return _MapWindowInteraction; }
        }

        public ContentControl MapWindowView { get; set; }

        #endregion properties


        #region methods

        private void OpenDock(OpenDockMessage message)
        {
            // TODO: use prism to resolve view models
            DockViewModelTemplate dockViewModel = null;
            switch (message.DockType)
            {
                case DockType.Clipboard:
                    dockViewModel = new ClipboardViewModel();
                    break;

                case DockType.ItemEditor:
                    dockViewModel = new ItemEditorViewModel(this.ea);
                    break;

                case DockType.ItemList:
                    dockViewModel = new ItemListViewModel();
                    break;

                case DockType.LayerList:
                    dockViewModel = new LayerListViewModel();
                    break;

                case DockType.Minimap:
                    dockViewModel = new MinimapViewModel();
                    break;

                case DockType.SelectedBrush:
                    dockViewModel = new SelectedBrushViewModel(this.ea);
                    break;

                case DockType.Toolbox:
                    dockViewModel = new ToolboxViewModel();
                    break;

                default:
                    break;
            }

            if (dockViewModel == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(message.DockTitle))
            {
                dockViewModel.Title = message.DockTitle;
            }
            this.Anchorables.Add(dockViewModel);

            // TODO: add active document
        }
        
        private void OpenWindow(OpenWindowMessage message)
        {
            INotification notification = null;
            string notificationTitle = string.Empty;
            if (!string.IsNullOrEmpty(message.WindowTitle))
            {
                notificationTitle = message.WindowTitle;
            }
            
            switch (message.WindowType)
            {
                case WindowType.Map:
                    notification = NewMapWindow();
                    notification.Title = notificationTitle;
                    this._MapWindowInteraction.Raise(notification, OnMapWindowClosed);
                    break;

                default:
                    break;
            }
        }

        private INotification NewMapWindow()
        {
            this.MapWindowView = new Windows.MapEditorWindow.MapEditor();
            RaisePropertyChanged(nameof(this.MapWindowView));

            MapWindowConfirmation mapWindowConfirmation = new MapWindowConfirmation();
            mapWindowConfirmation.Map = new Infrastructure.Models.MapModel("Map #1");
            return mapWindowConfirmation;
        }

        private void OnMapWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Console.WriteLine("Map Updated");
            }
        }

        // TODO update content id parameter type
        private BindableBase ContentViewModelFromID(string contentId)
        {
            BindableBase viewModel = null;
            if (contentId == "Clipboard")
            {
                viewModel = new ClipboardViewModel();
            }
            else if (contentId == "Item")
            {
                viewModel = new ItemEditorViewModel(this.ea);
            }
            else if (contentId == "Item List")
            {
                viewModel = new ItemListViewModel();
            }
            else if (contentId == "Layer List")
            {
                viewModel = new LayerListViewModel();
            }
            else if (contentId == "Minimap")
            {
                viewModel = new MinimapViewModel();
            }
            else if (contentId == "Selected Brush")
            {
                viewModel = new SelectedBrushViewModel(this.ea);
            }
            else if (contentId == "Tools")
            {
                viewModel = new ToolboxViewModel();
            }
            else if (contentId == "Map Editor")
            {
                viewModel = new MainEditorViewModel();
            }
            return viewModel;
        }

        private void SaveLayoutMessageReceived(NotificationActionMessage<string> message)
        {
            string xmlLayoutString = string.Empty;
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(this.DockManager);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            message.Execute(xmlLayoutString);
        }

        private void LoadLayoutMessageReceived(NotificationMessage<string> message)
        {
            StringReader stringReader = new StringReader(message.Content);
            var layoutSerializer = new XmlLayoutSerializer(this.DockManager);
            layoutSerializer.LayoutSerializationCallback += UpdateLayout;
            layoutSerializer.Deserialize(stringReader);
        }

        private void UpdateLayout(object sender, LayoutSerializationCallbackEventArgs args)
        {
            BindableBase contentViewModel = ContentViewModelFromID(args.Model.ContentId);
            if (contentViewModel == null)
            {
                args.Cancel = true;
            }
            args.Content = contentViewModel;
        }

        #endregion methods
    }
}
