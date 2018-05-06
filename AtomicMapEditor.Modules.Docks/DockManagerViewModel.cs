using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using Ame.Modules.Docks.ClipboardDock;
using Ame.Modules.Docks.ItemEditorDock;
using Ame.Modules.Docks.ItemListDock;
using Ame.Modules.Docks.LayerListDock;
using Ame.Modules.Docks.MinimapDock;
using Ame.Modules.Docks.SelectedBrushDock;
using Ame.Modules.Docks.Serializer;
using Ame.Modules.Docks.SessionViewerDock;
using Ame.Modules.Docks.ToolboxDock;
using Ame.Modules.MapEditor.Editor;
using Ame.Modules.Windows;
using Ame.Modules.Windows.LayerEditorWindow;
using Ame.Modules.Windows.MapEditorWindow;
using Ame.Modules.Windows.PreferencesWindow;
using Ame.Modules.Windows.TilesetEditorWindow;
using Ame.Modules.Windows.WindowInteractionFactories;
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

        private AmeSession session;
        private WindowInteractionCreator windowBuilder;

        #endregion fields


        #region constructor

        public DockManagerViewModel(AmeSession session, DockingManager dockManager, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.session = session;
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
                container.RegisterInstance(this.session);
                DockViewModelTemplate dockViewModel = CreateDockViewModel(DockType.MapEditor, container);
                AddDockViewModel(dockViewModel);
            }
            if (this.Documents.Count > 0)
            {
                this.ActiveDocument = this.Documents[0];
            }

            IWindowInteractionCreator[] factories = new IWindowInteractionCreator[]
            {
                new NewMapInteractionCreator(this.session, this.eventAggregator),
                new EditMapInteractionCreator(this.session, this.ActiveDocument),
                new NewLayerInteractionCreator(this.session, this.eventAggregator),
                new EditLayerInteractionCreator(this.session, this.session.CurrentMap.CurrentLayer),
                new TilesetEditorInteractionCreator(),
                new PreferenceOptionsInteractionCreator(this.eventAggregator)
            };
            this.windowBuilder = new WindowInteractionCreator(factories);

            this.eventAggregator.GetEvent<OpenDockEvent>().Subscribe(
                OpenDock,
                ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Subscribe(
                OpenWindow,
                ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<WindowType>>().Subscribe(
                CreateNewLayerGroup,
                ThreadOption.PublisherThread);
            this.eventAggregator.GetEvent<NotificationEvent<Infrastructure.Messages.Notification>>().Subscribe(
                NotificationReceived,
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

        public DockingManager DockManager { get; set; }
        public DockLayoutViewModel DockLayout { get; private set; }
        public ObservableCollection<DockViewModelTemplate> Documents { get; private set; }
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
            IUnityContainer container = message.Container;
            if (message.Container == null)
            {
                container = new UnityContainer();
                container.RegisterInstance<IEventAggregator>(this.eventAggregator);
                container.RegisterInstance<IScrollModel>(new ScrollModel());
                container.RegisterInstance(this.session);

                IList<ILayer> layerList = this.session.CurrentMap.LayerList;
                ObservableCollection<ILayer> layerObservableList = new ObservableCollection<ILayer>(layerList);
                container.RegisterInstance<ObservableCollection<ILayer>>(layerObservableList);
            }
            else
            {
                container = message.Container;
            }

            DockViewModelTemplate dockViewModel = CreateDockViewModel(message.Type, container);
            if (!string.IsNullOrEmpty(message.Title))
            {
                dockViewModel.Title = message.Title;
            }
            AddDockViewModel(dockViewModel);
            this.ActiveDocument = dockViewModel;
        }

        private void OpenWindow(OpenWindowMessage message)
        {
            IWindowInteraction interaction = null;
            IUnityContainer container = message.Container;
            Action<INotification> callback = null;
            if (message.Container != null)
            {
                this.windowBuilder.UpdateContainer(message.Type, container);
                interaction = this.windowBuilder.CreateWindowInteraction(message.Type);
            }
            else
            {
                interaction = this.windowBuilder.CreateWindowInteraction(message.Type);
            }
            if (interaction != null)
            {
                callback = interaction.OnWindowClosed;
                interaction.RaiseNotification(this.DockManager, callback);
            }
        }

        private void CreateNewLayerGroup(NotificationMessage<WindowType> layerGroup)
        {
            if (layerGroup.Content == WindowType.NewLayerGroup)
            {
                int layerGroupCount = GetLayerGroupCount();
                String newLayerGroupName = String.Format("Layer Group #{0}", layerGroupCount);
                ILayer newLayerGroup = new LayerGroup(newLayerGroupName);
                this.session.CurrentMap.LayerList.Add(newLayerGroup);

                NewLayerMessage newLayerMessage = new NewLayerMessage(newLayerGroup);
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
            container.RegisterInstance(this.session);

            IList<ILayer> layerList = this.session.CurrentMap.LayerList;
            ObservableCollection<ILayer> layerObservableList = new ObservableCollection<ILayer>(layerList);
            container.RegisterInstance<ObservableCollection<ILayer>>(layerObservableList);

            DockType dockType = DockTypeUtils.GetById(args.Model.ContentId);
            DockViewModelTemplate contentViewModel = CreateDockViewModel(dockType, container);
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

        private void NotificationReceived(NotificationMessage<Infrastructure.Messages.Notification> notification)
        {
            Map currentMap = this.session.CurrentMap;
            switch (notification.Content)
            {
                case Infrastructure.Messages.Notification.MergeCurrentLayerDown:
                    currentMap.MergeCurrentLayerDown();
                    break;

                case Infrastructure.Messages.Notification.MergeCurrentLayerUp:
                    currentMap.MergeCurrentLayerUp();
                    break;

                case Infrastructure.Messages.Notification.MergeVisibleLayers:
                    currentMap.MergeVisibleLayers();
                    break;

                case Infrastructure.Messages.Notification.DeleteCurrentLayer:
                    currentMap.DeleteCurrentLayer();
                    break;

                case Infrastructure.Messages.Notification.DuplicateCurrentLayer:
                    currentMap.DuplicateCurrentLayer();
                    break;

                default:
                    break;
            }
        }

        private int GetLayerGroupCount()
        {
            int layerGroupCount = 0;
            foreach (ILayer layer in this.session.CurrentMap.LayerList)
            {
                if (layer is LayerGroup)
                {
                    layerGroupCount++;
                }
            }
            return layerGroupCount;
        }

        private int GetLayerCount()
        {
            int layerCount = 0;
            foreach (ILayer layer in this.session.CurrentMap.LayerList)
            {
                if (layer is Layer)
                {
                    layerCount++;
                }
            }
            return layerCount;
        }

        private DockViewModelTemplate CreateDockViewModel(DockType dockType, IUnityContainer container)
        {
            Type viewModelType = null;
            switch (dockType)
            {
                case DockType.Clipboard:
                    viewModelType = typeof(ClipboardViewModel);
                    break;

                case DockType.ItemEditor:
                    viewModelType = typeof(ItemEditorViewModel);
                    break;

                case DockType.ItemList:
                    viewModelType = typeof(ItemListViewModel);
                    break;

                case DockType.LayerList:
                    viewModelType = typeof(LayerListViewModel);
                    break;

                case DockType.Minimap:
                    viewModelType = typeof(MinimapViewModel);
                    break;

                case DockType.SelectedBrush:
                    viewModelType = typeof(SelectedBrushViewModel);
                    break;

                case DockType.SessionView:
                    viewModelType = typeof(SessionViewerViewModel);
                    break;

                case DockType.Toolbox:
                    viewModelType = typeof(ToolboxViewModel);
                    break;

                case DockType.MapEditor:
                    if (!container.IsRegistered(typeof(Map)))
                    {
                        container.RegisterInstance<Map>(new Map("Map #1"));
                    }
                    viewModelType = typeof(MainEditorViewModel);
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unknown DockType");
            }
            DockViewModelTemplate dockViewModel = container.Resolve(viewModelType) as DockViewModelTemplate;
            return dockViewModel;
        }

        #endregion methods
    }
}
