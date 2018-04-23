using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Models;
using Ame.Modules.Docks.Core;
using Ame.Modules.MapEditor.Editor;
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

        private IEventAggregator eventAggregator;

        private event EventHandler ActiveDocumentChanged;

        #endregion fields


        #region constructor

        public DockManagerViewModel(AmeSession session, DockingManager dockManager, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.Session = session;
            this.DockManager = dockManager;
            this.DockLayout = new DockLayoutViewModel(this, eventAggregator);
            this.eventAggregator = eventAggregator;

            this.Documents = new ObservableCollection<DockViewModelTemplate>();
            this.Anchorables = new ObservableCollection<DockViewModelTemplate>();
            foreach (Map map in session.MapList)
            {
                IUnityContainer container = new UnityContainer();
                container.RegisterInstance<IEventAggregator>(this.eventAggregator);
                container.RegisterInstance<IScrollModel>(new ScrollModel());
                container.RegisterInstance<Map>(map);
                DockViewModelTemplate dockViewModel = DockViewModelSelector.GetViewModel(DockType.MapEditor, container);
                AddDockViewModel(dockViewModel);
            }

            this.mapWindowInteraction = new InteractionRequest<INotification>();
            this.layerWindowInteraction = new InteractionRequest<INotification>();

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

        #endregion constructor


        #region properties

        public AmeSession Session { get; set; }
        public DockingManager DockManager { get; set; }
        public DockLayoutViewModel DockLayout { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        public ContentControl MapWindowView { get; set; }
        public ContentControl LayerWindowView { get; set; }

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

        private DockViewModelTemplate activeDocument = null;
        public DockViewModelTemplate ActiveDocument
        {
            get { return activeDocument; }
            set
            {
                if (SetProperty(ref activeDocument, value))
                {
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private InteractionRequest<INotification> mapWindowInteraction;
        public IInteractionRequest MapWindowInteraction
        {
            get { return mapWindowInteraction; }
        }

        private InteractionRequest<INotification> layerWindowInteraction;
        public IInteractionRequest LayerWindowInteraction
        {
            get { return layerWindowInteraction; }
        }

        public string AppDataDirectory
        {
            get
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string directoryPath = Path.Combine(documentPath, Global.applicationName);
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

        // TODO add style and template selecter, similar to docks
        private void OpenDock(OpenDockMessage message)
        {
            IUnityContainer container;
            if (message.Container == null)
            {
                container = new UnityContainer();
                container.RegisterInstance<IEventAggregator>(this.eventAggregator);
                container.RegisterInstance<IScrollModel>(new ScrollModel());

                // TODO fix this when DockViewModelSelector is removed
                IList<ILayer> layerList = this.Session.CurrentMap().LayerList;
                ObservableCollection<ILayer> layerObservableList = new ObservableCollection<ILayer>(layerList);
                container.RegisterInstance<ObservableCollection<ILayer>>(layerObservableList);
            }
            else
            {
                container = message.Container;
            }

            // TODO remove this static class, replace with interface for each
            DockViewModelTemplate dockViewModel = DockViewModelSelector.GetViewModel(message.DockType, container);
            if (!string.IsNullOrEmpty(message.DockTitle))
            {
                dockViewModel.Title = message.DockTitle;
            }
            AddDockViewModel(dockViewModel);
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

            // TODO switch window type to an interface
            switch (message.WindowType)
            {
                case WindowType.NewMap:
                    notification = NewMapWindow();
                    notification.Title = notificationTitle;
                    this.mapWindowInteraction.Raise(notification, OnNewMapWindowClosed);
                    break;

                case WindowType.EditMap:
                    notification = EditMapWindow();
                    notificationTitle = string.Format("Edit Map - {0}", this.Session.CurrentMap().Name);
                    notification.Title = notificationTitle;
                    this.mapWindowInteraction.Raise(notification, OnEditMapWindowClosed);
                    break;

                case WindowType.NewLayer:
                    Map currentMap = this.Session.CurrentMap();
                    int layerCount = currentMap.LayerList.Count;
                    String newLayerName = String.Format("Layer #{0}", layerCount);
                    notification = NewLayerWindow(new Layer(newLayerName, 32, 32, 32, 32));
                    notification.Title = notificationTitle;

                    // TODO do not rely on the title name 
                    // TODO establish a better messaging system
                    this.layerWindowInteraction.Raise(notification, OnLayerWindowClosed);
                    break;

                case WindowType.EditLayer:
                    notification = NewLayerWindow(message.Content as Layer);
                    notification.Title = notificationTitle;
                    this.layerWindowInteraction.Raise(notification);
                    break;

                default:
                    break;
            }
        }

        private INotification NewMapWindow()
        {
            this.MapWindowView = new Windows.MapEditorWindow.MapEditor();
            RaisePropertyChanged(nameof(this.MapWindowView));

            Confirmation mapConfirmation = new Confirmation();
            int mapCount = this.Session.MapList.Count + 1;
            string newMapName = String.Format("Map #{0}", mapCount);
            mapConfirmation.Content = new Map(newMapName);
            return mapConfirmation;
        }

        private INotification EditMapWindow()
        {
            this.MapWindowView = new Windows.MapEditorWindow.MapEditor();
            RaisePropertyChanged(nameof(this.MapWindowView));

            Confirmation mapConfirmation = new Confirmation();
            Map currentMap = this.Session.CurrentMap();
            mapConfirmation.Content = currentMap;
            string newMapName = currentMap.Name;
            return mapConfirmation;
        }

        private INotification NewLayerWindow(ILayer layer)
        {
            this.LayerWindowView = new LayerEditor();
            RaisePropertyChanged(nameof(this.LayerWindowView));

            Confirmation layerWindowConfirmation = new Confirmation();
            layerWindowConfirmation.Content = layer;
            return layerWindowConfirmation;
        }

        private void OnNewMapWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Map mapModel = confirmation.Content as Map;

                IUnityContainer container = new UnityContainer();
                container.RegisterInstance<IEventAggregator>(this.eventAggregator);
                container.RegisterInstance<IScrollModel>(new ScrollModel());
                container.RegisterInstance<Map>(mapModel);

                // TODO srsly, remove this
                IList<ILayer> layerList = this.Session.CurrentMap().LayerList;
                ObservableCollection<ILayer> layerObservableList = new ObservableCollection<ILayer>(layerList);
                container.RegisterInstance<ObservableCollection<ILayer>>(layerObservableList);

                this.Session.MapList.Add(mapModel);

                OpenDockMessage openEditorMessage = new OpenDockMessage(DockType.MapEditor, container);
                OpenDock(openEditorMessage);
            }
        }

        private void OnEditMapWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Map mapModel = confirmation.Content as Map;
                this.ActiveDocument.Title = mapModel.Name;
            }
        }

        private void OnLayerWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Layer layerModel = confirmation.Content as Layer;

                NewLayerMessage newLayerMessage = new NewLayerMessage(layerModel);
                this.eventAggregator.GetEvent<NewLayerEvent>().Publish(newLayerMessage);
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
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IEventAggregator>(this.eventAggregator);
            container.RegisterInstance<IScrollModel>(new ScrollModel());

            // TODO srsly, remove this
            IList<ILayer> layerList = this.Session.CurrentMap().LayerList;
            ObservableCollection<ILayer> layerObservableList = new ObservableCollection<ILayer>(layerList);
            container.RegisterInstance<ObservableCollection<ILayer>>(layerObservableList);

            DockType dockType = DockTypeUtils.GetById(args.Model.ContentId);
            DockViewModelTemplate contentViewModel = DockViewModelSelector.GetViewModel(dockType, container);
            if (contentViewModel == null)
            {
                args.Cancel = true;
            }
            else
            {
                AddDockViewModel(contentViewModel);
            }
            args.Content = contentViewModel;
        }

        private void AddDockViewModel(DockViewModelTemplate dockViewModel)
        {
            dockViewModel.IsVisible = true;
            if (dockViewModel is DockToolViewModelTemplate)
            {
                this.Anchorables.Add(dockViewModel);
            }
            else if (dockViewModel is EditorViewModelTemplate)
            {
                this.Documents.Add(dockViewModel);
            }
        }

        #endregion methods
    }
}
