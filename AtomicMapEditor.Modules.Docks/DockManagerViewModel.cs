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
    // TODO Clean up doc creation
    // TODO Add icons to menu
    public class DockManagerViewModel : BindableBase, ILayoutViewModel
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
                MapEditorCreator mapEditorCreator = new MapEditorCreator(this.eventAggregator, new ScrollModel(), new Map("Map #1"));
                DockViewModelTemplate dockViewModel = mapEditorCreator.CreateDock();
                AddDockViewModel(dockViewModel);
            }
            if (this.Documents.Count > 0)
            {
                this.ActiveDocument = this.Documents[0];
            }

            IWindowInteractionCreator[] windowInteractionCreators = new IWindowInteractionCreator[]
            {
                new NewMapInteractionCreator(this.session, this.eventAggregator),
                new EditMapInteractionCreator(this.session, this.ActiveDocument),
                new NewLayerInteractionCreator(this.session, this.eventAggregator),
                new EditLayerInteractionCreator(this.session, this.session.CurrentMap.CurrentLayer),
                new TilesetEditorInteractionCreator(),
                new PreferenceOptionsInteractionCreator(this.eventAggregator)
            };
            this.windowInteractionCreator = new WindowInteractionCreator(windowInteractionCreators);

            IDockCreator[] dockCreators = new IDockCreator[]
            {
                new ClipboardCreator(this.eventAggregator),
                new ItemEditorCreator(this.eventAggregator, new ScrollModel()),
                new ItemListCreator(this.eventAggregator),
                new LayerListCreator(this.eventAggregator, this.session.CurrentMap.LayerList),
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
            this.ActiveDocument = dockViewModel;
        }

        private void OpenWindow(OpenWindowMessage message)
        {
            IWindowInteraction interaction = null;
            IUnityContainer container = message.Container;
            Action<INotification> callback = null;
            if (container != null)
            {
                this.windowInteractionCreator.UpdateContainer(message.Type, container);
            }
            interaction = this.windowInteractionCreator.CreateWindowInteraction(message.Type);
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
            Type registeredType = null;
            foreach (Type dockType in this.dockTemplateTypes)
            {
                DockContentIdAttribute contentId = Attribute.GetCustomAttribute(dockType, typeof(DockContentIdAttribute)) as DockContentIdAttribute;
                if (contentId.Id == args.Model.ContentId)
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

        #endregion methods
    }
}
