using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Modules.Docks.Core;
using Ame.Modules.Windows.LayerEditorWindow;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Ame.Modules.Docks
{
    public class DockManagerViewModel : BindableBase, ILayoutViewModel
    {
        #region fields

        // TODO add this in a config file
        public static string applicationName = "AtomicMapEditor";

        private IEventAggregator eventAggregator;

        public event EventHandler ActiveDocumentChanged;

        #endregion fields


        #region constructor & destructer

        public DockManagerViewModel(DockingManager dockManager, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.DockManager = dockManager;
            this.DockLayout = new DockLayoutViewModel(this, eventAggregator);
            this.eventAggregator = eventAggregator;

            this.Documents = new ObservableCollection<DockViewModelTemplate>();
            this.Anchorables = new ObservableCollection<DockViewModelTemplate>();

            this._MapWindowInteraction = new InteractionRequest<INotification>();
            this._LayerWindowInteraction = new InteractionRequest<INotification>();

            // TODO add filter to dock and window open messages
            this.eventAggregator.GetEvent<OpenDockEvent>().Subscribe(
                OpenDock,
                ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Subscribe(
                OpenWindow,
                ThreadOption.PublisherThread);
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
        }

        #endregion constructor & destructer


        #region properties

        public DockingManager DockManager { get; set; }
        public DockLayoutViewModel DockLayout { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        public ContentControl MapWindowView { get; set; }
        public ContentControl LayerWindowView { get; set; }

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

        private DockViewModelTemplate _ActiveDocument = null;
        public DockViewModelTemplate ActiveDocument
        {
            get { return _ActiveDocument; }
            set
            {
                if (SetProperty(ref _ActiveDocument, value))
                {
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private InteractionRequest<INotification> _MapWindowInteraction;
        public IInteractionRequest MapWindowInteraction
        {
            get { return _MapWindowInteraction; }
        }

        private InteractionRequest<INotification> _LayerWindowInteraction;
        public IInteractionRequest LayerWindowInteraction
        {
            get { return _LayerWindowInteraction; }
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

        #endregion properties


        #region methods

        private void OpenDock(OpenDockMessage message)
        {
            IUnityContainer unityContainer = new UnityContainer();
            unityContainer.RegisterInstance<IEventAggregator>(this.eventAggregator);

            DockViewModelTemplate dockViewModel = DockViewModelSelector.GetViewModel(message.DockType, unityContainer);
            if (!string.IsNullOrEmpty(message.DockTitle))
            {
                dockViewModel.Title = message.DockTitle;
            }
            this.Anchorables.Add(dockViewModel);
            this.ActiveDocument = dockViewModel;
        }

        private void OpenWindow(OpenWindowMessage message)
        {
            INotification notification = null;
            string notificationTitle = string.Empty;
            if (!string.IsNullOrEmpty(message.WindowTitle))
            {
                notificationTitle = message.WindowTitle;
            }

            // TODO move this to another class
            switch (message.WindowType)
            {
                case WindowType.Map:
                    notification = NewMapWindow();
                    notification.Title = notificationTitle;
                    this._MapWindowInteraction.Raise(notification, OnMapWindowClosed);
                    break;

                case WindowType.Layer:
                    notification = NewLayerWindow();
                    notification.Title = notificationTitle;
                    this._LayerWindowInteraction.Raise(notification, OnLayerWindowClosed);
                    break;

                default:
                    break;
            }
        }

        // TODO, move this into another class, or the appropriate view model class (Static function)
        private INotification NewMapWindow()
        {
            this.MapWindowView = new Windows.MapEditorWindow.MapEditor();
            RaisePropertyChanged(nameof(this.MapWindowView));

            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Content = new Infrastructure.Models.MapModel("Map #1");
            return mapConfirmation;
        }

        private INotification NewLayerWindow()
        {
            this.LayerWindowView = new LayerEditor();
            RaisePropertyChanged(nameof(this.LayerWindowView));

            Confirmation layerWindowConfirmation = new Confirmation();
            return layerWindowConfirmation;
        }

        private void OnMapWindowClosed(INotification notification)
        {
            Confirmation confirmation = notification as Confirmation;
            if (confirmation.Confirmed)
            {
                MapModel mapModel = confirmation.Content as MapModel;
                Console.WriteLine("Map Updated: ");
                Console.WriteLine("Name: " + mapModel.Name);

            }
        }

        private void OnLayerWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Console.WriteLine("Layer Updated");
            }
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
            IUnityContainer unityContainer = new UnityContainer();
            unityContainer.RegisterInstance<IEventAggregator>(this.eventAggregator);

            DockType dockType = DockTypeUtils.GetById(args.Model.ContentId);
            DockViewModelTemplate contentViewModel = DockViewModelSelector.GetViewModel(dockType, unityContainer);
            if (contentViewModel == null)
            {
                args.Cancel = true;
            }
            args.Content = contentViewModel;
        }

        #endregion methods
    }
}
