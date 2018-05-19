using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.Docks.ClipboardDock;
using Ame.Modules.Windows.Docks.ItemEditorDock;
using Ame.Modules.Windows.Docks.ItemListDock;
using Ame.Modules.Windows.Docks.LayerListDock;
using Ame.Modules.Windows.Docks.MinimapDock;
using Ame.Modules.Windows.Docks.SelectedBrushDock;
using Ame.Modules.Windows.Serializer;
using Ame.Modules.Windows.Docks.SessionViewerDock;
using Ame.Modules.Windows.Docks.ToolboxDock;
using Ame.Modules.MapEditor.Editor;
using Ame.Modules.Windows.Interactions.LayerPropertiesInteraction;
using Ame.Modules.Windows.Interactions.MapPropertiesInteraction;
using Ame.Modules.Windows.Interactions.PreferencesInteraction;
using Ame.Modules.Windows.Interactions.TilesetEditorInteraction;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Ame.Modules.Windows.Interactions;
using Ame.Modules.Windows.Docks;

namespace Ame.Modules.Windows
{
    public class WindowManagerViewModel : BindableBase, ILayoutViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;
        private Type[] dockTemplateTypes;

        private event EventHandler ActiveDocumentChanged;
        
        private AmeSession session;
        private WindowInteractionCreator windowInteractionCreator;
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
                MapEditorCreator mapEditorCreator = new MapEditorCreator(this.eventAggregator, new ScrollModel(), new Map("Map #1"));
                DockViewModelTemplate dockViewModel = mapEditorCreator.CreateDock();
                AddDockViewModel(dockViewModel);
            }
            if (this.Documents.Count > 0)
            {
                this.ActiveDocument = this.Documents[0];
            }
            // TODO use ? operator
            ILayer currentLayer = null;
            if (this.session.CurrentMap != null)
            {
                if (this.session.CurrentMap.CurrentLayer != null)
                {
                    currentLayer = this.session.CurrentMap.CurrentLayer;
                }
            }
            IWindowInteractionCreator[] windowInteractionCreators = new IWindowInteractionCreator[]
            {
                new NewMapInteractionCreator(this.session, this.eventAggregator, OnNewMapWindowClosed),
                new EditMapInteractionCreator(this.session, OnEditMapWindowClosed),
                new NewLayerInteractionCreator(this.session, this.eventAggregator, OnNewLayerWindowClosed),
                new EditLayerInteractionCreator(currentLayer),
                new EditTilesetInteractionCreator(this.eventAggregator),
                new PreferenceOptionsInteractionCreator(this.eventAggregator)
            };
            this.windowInteractionCreator = new WindowInteractionCreator(windowInteractionCreators);


            ObservableCollection<ILayer> layerList = null;
            if (this.session.CurrentMap != null)
            {
                layerList = this.session.CurrentMap.LayerList;
            }
            IDockCreator[] dockCreators = new IDockCreator[]
            {
                new ClipboardCreator(this.eventAggregator),
                new ItemEditorCreator(this.eventAggregator, new ScrollModel()),
                new ItemListCreator(this.eventAggregator),
                new LayerListCreator(this.eventAggregator, layerList),
                new MinimapCreator(this.eventAggregator),
                new SelectedBrushCreator(this.eventAggregator, new ScrollModel()),
                new SessionViewerCreator(this.eventAggregator, this.session),
                new ToolboxCreator(this.eventAggregator),
                new MapEditorCreator(this.eventAggregator, new ScrollModel(), new Map("Map #1"))
            };
            this.dockCreator = new DockCreator(dockCreators);

            Type dockTemplateType = typeof(DockViewModelTemplate);
            this.dockTemplateTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                      from assemblyType in domainAssembly.GetTypes()
                                      where typeof(DockViewModelTemplate).IsAssignableFrom(assemblyType)
                                      select assemblyType).ToArray();

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
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Subscribe(
                ViewNotificationReceived,
                ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<ZoomLevel>>().Subscribe(
                SetZoomLevel,
                ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        public DockingManager WindowManager { get; set; }
        public DockLayoutViewModel DockLayout { get; private set; }
        public ObservableCollection<EditorViewModelTemplate> Documents { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Anchorables { get; private set; }

        public ContentControl MapWindowView { get; set; }
        public ContentControl LayerWindowView { get; set; }
        public ContentControl TilesetWindowView { get; set; }

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
                        this.session.ChangeMap(selectedMapContent);
                        Console.WriteLine(this.session.CurrentMap.Name);
                    }
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
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
        
        private void OpenDock(OpenDockMessage message)
        {
            DockViewModelTemplate dockViewModel;
            IUnityContainer container = message.Container;
            if (container != null)
            {
                this.dockCreator.UpdateContainer(message.Type, container);
            }
            dockViewModel = this.dockCreator.CreateDock(message.Type);
            if (!string.IsNullOrEmpty(message.Title))
            {
                dockViewModel.Title = message.Title;
            }
            AddDockViewModel(dockViewModel);
        }

        private void OpenWindow(OpenWindowMessage message)
        {
            IWindowInteraction interaction = null;
            object content = message.content;
            if (content != null)
            {
                this.windowInteractionCreator.UpdateContainer(message.Type, content.GetType(), content);
            }
            interaction = this.windowInteractionCreator.CreateWindowInteraction(message.Type);
            if (interaction != null)
            {
                interaction.RaiseNotification(this.WindowManager);
            }
        }

        private void OnNewLayerWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Layer layerModel = confirmation.Content as Layer;

                NewLayerMessage newLayerMessage = new NewLayerMessage(layerModel);
                this.eventAggregator.GetEvent<NewLayerEvent>().Publish(newLayerMessage);
            }
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
                container.RegisterInstance(this.session);
                
                ObservableCollection<ILayer> layerObservableList = new ObservableCollection<ILayer>();
                container.RegisterInstance<ObservableCollection<ILayer>>(layerObservableList);

                this.session.MapList.Add(mapModel);

                OpenDockMessage openEditorMessage = new OpenDockMessage(typeof(MapEditor.Editor.MapEditorViewModel), container);
                this.eventAggregator.GetEvent<OpenDockEvent>().Publish(openEditorMessage);
            }
        }

        private void OnEditMapWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Map mapModel = confirmation.Content as Map;
                this.activeDocument.Title = mapModel.Name;
            }
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

        #endregion methods
    }
}
