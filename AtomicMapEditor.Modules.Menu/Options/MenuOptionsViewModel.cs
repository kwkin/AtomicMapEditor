using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Modules.Docks.ClipboardDock;
using Ame.Modules.Docks.ItemEditorDock;
using Ame.Modules.Docks.ItemListDock;
using Ame.Modules.Docks.LayerListDock;
using Ame.Modules.Docks.MinimapDock;
using Ame.Modules.Docks.SelectedBrushDock;
using Ame.Modules.Docks.SessionViewerDock;
using Ame.Modules.Docks.ToolboxDock;
using Ame.Modules.Windows.LayerEditorWindow;
using Ame.Modules.Windows.MapEditorWindow;
using Ame.Modules.Windows.PreferencesWindow;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Ame.Modules.Menu.Options
{
    public class MenuOptionsViewModel : BindableBase
    {
        #region fields

        private IEventAggregator eventAggregator;
        private ObservableCollection<MenuItem> recentlyClosedDockItems = new ObservableCollection<MenuItem>();
        private ObservableCollection<MenuItem> recentFileItems = new ObservableCollection<MenuItem>();

        #endregion fields


        #region constructor

        public MenuOptionsViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.eventAggregator = eventAggregator;

            // File bindings
            this.NewFileCommand = new DelegateCommand(() => NewFile());
            this.OpenFileCommand = new DelegateCommand(() => OpenFile());
            this.SaveFileCommand = new DelegateCommand(() => SaveFile());
            this.SaveAsFileCommand = new DelegateCommand(() => SaveAsFile());
            this.ExportFileCommand = new DelegateCommand(() => ExportFile());
            this.ExportAsFileCommand = new DelegateCommand(() => ExportAsFile());
            this.ImportFileCommand = new DelegateCommand(() => ImportFile());
            this.ViewFilePropertiesCommand = new DelegateCommand(() => ViewFileProperties());
            this.CloseFileCommand = new DelegateCommand(() => CloseFile());
            this.CloseAllFilesCommand = new DelegateCommand(() => CloseAllFiles());
            this.ExitProgramCommand = new DelegateCommand(() => ExitProgram());

            // Edit bindings
            this.UndoCommand = new DelegateCommand(() => Undo());
            this.RedoCommand = new DelegateCommand(() => Redo());
            this.CutCommand = new DelegateCommand(() => CutSelection());
            this.CopyCommand = new DelegateCommand(() => CopySelection());
            this.PasteCommand = new DelegateCommand(() => PasteClipboard());
            this.OpenClipboardCommand = new DelegateCommand(() => OpenClipboard());
            this.OpenPreferencesCommand = new DelegateCommand(() => OpenPreferenences());

            // Map bindings
            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.DuplicateMapCommand = new DelegateCommand(() => DuplicateMap());
            this.FlipMapHorizontallyCommand = new DelegateCommand(() => FlipMapHorizontally());
            this.FlipMapVerticallyCommand = new DelegateCommand(() => FlipMapVertically());
            this.GuillotineMapCommand = new DelegateCommand(() => GuillotineMap());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());

            // Layer bindings
            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.NewGroupCommand = new DelegateCommand(() => NewGroup());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.MergeLayerDownCommand = new DelegateCommand(() => MergeLayerDown());
            this.MergeLayerUpCommand = new DelegateCommand(() => MergeLayerUp());
            this.MergeVisibleCommand = new DelegateCommand(() => MergeVisible());
            this.DeleteLayerCommand = new DelegateCommand(() => DeleteLayer());
            this.EditLayerPropertiesCommand = new DelegateCommand(() => EditLayerProperties());
            this.LayerToMapCommand = new DelegateCommand(() => LayerToMap());

            // Item bindings
            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.AddGroupCommand = new DelegateCommand(() => AddGroup());
            this.EditItemPropertiesCommand = new DelegateCommand(() => EditItemProperties());
            this.EditItemCollisionsCommand = new DelegateCommand(() => EditItemCollisions());

            // View bindings
            this.SampleViewCommand = new DelegateCommand(() => SampleView());
            this.CollisionsViewCommand = new DelegateCommand(() => CollisionsView());
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.ZoomToolCommand = new DelegateCommand(() => ZoomTool());
            this.FitMapToWindowCommand = new DelegateCommand(() => FitMapToWindow());
            this.SetZoomCommand = new DelegateCommand(() => SetZoom());
            this.DockPresetViewCommand = new DelegateCommand(() => DockPresetView());
            this.ShowGridCommand = new DelegateCommand(() => ShowGrid());
            this.ShowRulerCommand = new DelegateCommand(() => ShowRuler());
            this.ShowScrollBarCommand = new DelegateCommand(() => ShowScrollBar());

            // Window bindings
            this.OpenItemEditorDockCommand = new DelegateCommand(() => OpenItemEditorDock());
            this.OpenItemListDockCommand = new DelegateCommand(() => OpenItemListDock());
            this.OpenLayerListDockCommand = new DelegateCommand(() => OpenLayerListDock());
            this.OpenToolboxDockCommand = new DelegateCommand(() => OpenToolboxDock());
            this.OpenMinimapDockCommand = new DelegateCommand(() => OpenMinimapDock());
            this.OpenClipboardDockCommand = new DelegateCommand(() => OpenClipboardDock());
            this.OpenUndoHistoryDockCommand = new DelegateCommand(() => OpenUndoHistoryDock());
            this.OpenSelectedBrushDockCommand = new DelegateCommand(() => OpenSelectedBrushDock());
            this.OpenSessionViewDockCommand = new DelegateCommand(() => OpenSessionViewerDock());
            this.HideDocksCommand = new DelegateCommand(() => HideDocks());
            this.SingleWindowCommand = new DelegateCommand(() => SingleWindow());

            // Help bindings
            this.HelpCommand = new DelegateCommand(() => Help());
            this.AboutCommand = new DelegateCommand(() => About());

            this.TestAddClosedDocksCommand = new DelegateCommand(() => TestAddClosedDocks());
        }

        #endregion constructor


        #region properties

        public ObservableCollection<MenuItem> RecentlyClosedDockItems
        {
            get { return recentlyClosedDockItems; }
            set { recentlyClosedDockItems = value; }
        }

        public ObservableCollection<MenuItem> RecentlFileItems
        {
            get { return recentFileItems; }
            set { recentFileItems = value; }
        }

        public bool IsShowGrid { get; set; }
        public bool IsShowRuler { get; set; }
        public bool IsShowScrollBar { get; set; }

        public ICommand NewFileCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand SaveAsFileCommand { get; private set; }
        public ICommand ExportFileCommand { get; private set; }
        public ICommand ExportAsFileCommand { get; private set; }
        public ICommand ImportFileCommand { get; private set; }
        public ICommand ViewFilePropertiesCommand { get; private set; }
        public ICommand CloseFileCommand { get; private set; }
        public ICommand CloseAllFilesCommand { get; private set; }
        public ICommand ExitProgramCommand { get; private set; }

        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand OpenClipboardCommand { get; private set; }
        public ICommand OpenPreferencesCommand { get; private set; }

        public ICommand NewMapCommand { get; private set; }
        public ICommand DuplicateMapCommand { get; private set; }
        public ICommand FlipMapHorizontallyCommand { get; private set; }
        public ICommand FlipMapVerticallyCommand { get; private set; }
        public ICommand GuillotineMapCommand { get; private set; }
        public ICommand EditMapPropertiesCommand { get; private set; }

        public ICommand NewLayerCommand { get; private set; }
        public ICommand NewGroupCommand { get; private set; }
        public ICommand DuplicateLayerCommand { get; private set; }
        public ICommand MergeLayerDownCommand { get; private set; }
        public ICommand MergeLayerUpCommand { get; private set; }
        public ICommand MergeVisibleCommand { get; private set; }
        public ICommand DeleteLayerCommand { get; private set; }
        public ICommand EditLayerPropertiesCommand { get; private set; }
        public ICommand LayerToMapCommand { get; private set; }

        public ICommand AddTilesetCommand { get; private set; }
        public ICommand AddImageCommand { get; private set; }
        public ICommand AddGroupCommand { get; private set; }
        public ICommand EditItemPropertiesCommand { get; private set; }
        public ICommand EditItemCollisionsCommand { get; private set; }

        public ICommand SampleViewCommand { get; private set; }
        public ICommand CollisionsViewCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand ZoomToolCommand { get; private set; }
        public ICommand FitMapToWindowCommand { get; private set; }
        public ICommand SetZoomCommand { get; private set; }
        public ICommand DockPresetViewCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand ShowRulerCommand { get; private set; }
        public ICommand ShowScrollBarCommand { get; private set; }

        public ICommand OpenItemEditorDockCommand { get; private set; }
        public ICommand OpenItemListDockCommand { get; private set; }
        public ICommand OpenLayerListDockCommand { get; private set; }
        public ICommand OpenToolboxDockCommand { get; private set; }
        public ICommand OpenMinimapDockCommand { get; private set; }
        public ICommand OpenClipboardDockCommand { get; private set; }
        public ICommand OpenUndoHistoryDockCommand { get; private set; }
        public ICommand OpenSelectedBrushDockCommand { get; private set; }
        public ICommand OpenSessionViewDockCommand { get; private set; }
        public ICommand HideDocksCommand { get; private set; }
        public ICommand SingleWindowCommand { get; private set; }

        public ICommand HelpCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }

        public ICommand TestAddClosedDocksCommand { get; private set; }

        #endregion properties


        #region methods

        #region file methods

        public void NewFile()
        {
            Console.WriteLine("New File");
        }

        public void OpenFile()
        {
            Console.WriteLine("Open File");
        }

        public void SaveFile()
        {
            Console.WriteLine("Save File");
        }

        public void SaveAsFile()
        {
            Console.WriteLine("Save As");
        }

        public void ExportFile()
        {
            Console.WriteLine("Export File");
        }

        public void ExportAsFile()
        {
            Console.WriteLine("Export As");
        }

        public void ImportFile()
        {
            Console.WriteLine("Import");
        }

        public void ViewFileProperties()
        {
            Console.WriteLine("Properties");
        }

        public void CloseFile()
        {
            Console.WriteLine("Close");
        }

        public void CloseAllFiles()
        {
            Console.WriteLine("Close All Files");
        }

        public void ExitProgram()
        {
            Console.WriteLine("Exiting Program");
        }

        #endregion file methods

        #region edit methods

        public void Undo()
        {
            Console.WriteLine("Undo ");
        }

        public void Redo()
        {
            Console.WriteLine("Redo ");
        }

        public void CutSelection()
        {
            Console.WriteLine("Cut Selection");
        }

        public void CopySelection()
        {
            Console.WriteLine("Copy Selection");
        }

        public void PasteClipboard()
        {
            Console.WriteLine("Paste Clipboard");
        }

        public void OpenClipboard()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(ClipboardViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenPreferenences()
        {
            OpenWindowMessage window = new OpenWindowMessage(typeof(PreferenceOptionsInteraction));
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(window);
        }

        #endregion edit methods

        #region map methods

        public void NewMap()
        {
            OpenWindowMessage window = new OpenWindowMessage(typeof(NewMapInteraction));
            window.Title = "New Map";
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(window);
        }

        public void DuplicateMap()
        {
            Console.WriteLine("Duplicate Map");
        }

        public void FlipMapHorizontally()
        {
            Console.WriteLine("Flip Map Horizontally");
        }

        public void FlipMapVertically()
        {
            Console.WriteLine("Flip Map Vertically");
        }

        public void GuillotineMap()
        {
            Console.WriteLine("Guillotine");
        }

        public void EditMapProperties()
        {
            OpenWindowMessage window = new OpenWindowMessage(typeof(EditMapInteraction));
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(window);
        }

        #endregion map methods

        #region layer methods

        public void NewLayer()
        {
            OpenWindowMessage openWindowMessage = new OpenWindowMessage(typeof(NewLayerInteraction));
            openWindowMessage.Title = "New Layer";
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(openWindowMessage);
        }

        public void NewGroup()
        {
            NotificationMessage<WindowType> newLayerGroupMessage = new NotificationMessage<WindowType>(WindowType.NewLayerGroup, "LayerGroup");
            this.eventAggregator.GetEvent<NotificationEvent<WindowType>>().Publish(newLayerGroupMessage);
        }

        public void DuplicateLayer()
        {
            NotificationMessage<Notification> message = new NotificationMessage<Notification>(Notification.DeleteCurrentLayer);
            this.eventAggregator.GetEvent<NotificationEvent<Notification>>().Publish(message);
        }

        public void MergeLayerDown()
        {
            NotificationMessage<Notification> message = new NotificationMessage<Notification>(Notification.MergeCurrentLayerDown);
            this.eventAggregator.GetEvent<NotificationEvent<Notification>>().Publish(message);
        }

        public void MergeLayerUp()
        {
            NotificationMessage<Notification> message = new NotificationMessage<Notification>(Notification.MergeCurrentLayerDown);
            this.eventAggregator.GetEvent<NotificationEvent<Notification>>().Publish(message);
        }

        public void MergeVisible()
        {
            NotificationMessage<Notification> message = new NotificationMessage<Notification>(Notification.MergeVisibleLayers);
            this.eventAggregator.GetEvent<NotificationEvent<Notification>>().Publish(message);
        }

        public void DeleteLayer()
        {
            NotificationMessage<Notification> message = new NotificationMessage<Notification>(Notification.DeleteCurrentLayer);
            this.eventAggregator.GetEvent<NotificationEvent<Notification>>().Publish(message);
        }

        public void EditLayerProperties()
        {
            OpenWindowMessage openWindowMessage = new OpenWindowMessage(typeof(EditLayerInteraction));
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(openWindowMessage);
        }

        public void LayerToMap()
        {
            Console.WriteLine("Layer To Map ");
        }

        #endregion layer methods

        #region item methods

        public void AddTileset()
        {
            Console.WriteLine("Add Tileset");
        }

        public void AddImage()
        {
            Console.WriteLine("Add Image");
        }

        public void AddGroup()
        {
            Console.WriteLine("Add Group");
        }

        public void EditItemProperties()
        {
            Console.WriteLine("Edit Item Properties...");
        }

        public void EditItemCollisions()
        {
            Console.WriteLine("Edit Item Collisions");
        }

        #endregion item methods

        #region view methods

        public void SampleView()
        {
            Console.WriteLine("Sample View");
        }

        public void CollisionsView()
        {
            Console.WriteLine("Collisions View");
        }

        public void ZoomIn()
        {
            Console.WriteLine("Zoom in");
        }

        public void ZoomOut()
        {
            Console.WriteLine("Zoom out");
        }

        public void ZoomTool()
        {
            Console.WriteLine("Zoom Tool");
        }

        public void FitMapToWindow()
        {
            Console.WriteLine("Fit Map To Window");
        }

        public void SetZoom()
        {
            Console.WriteLine("Set Zoom");
        }

        public void DockPresetView()
        {
            Console.WriteLine("Dock Preset");
        }

        public void ShowGrid()
        {
            Console.WriteLine("Show Grid: " + IsShowGrid);
        }

        public void ShowRuler()
        {
            Console.WriteLine("Show Ruler: " + IsShowRuler);
        }

        public void ShowScrollBar()
        {
            Console.WriteLine("Show Scroll: " + IsShowScrollBar);
        }

        #endregion view methods

        #region window methods

        public void OpenClipboardDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(ClipboardViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenItemEditorDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(ItemEditorViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenItemListDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(ItemListViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenLayerListDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(LayerListViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenToolboxDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(ToolboxViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenMinimapDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(MinimapViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenSelectedBrushDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(SelectedBrushViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenSessionViewerDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(SessionViewerViewModel));
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dock);
        }

        public void OpenUndoHistoryDock()
        {
            Console.WriteLine("Open Undo History Dock");
        }

        public void HideDocks()
        {
            Console.WriteLine("Hide Dock ");
        }

        public void SingleWindow()
        {
            Console.WriteLine("Single Window ");
        }

        #endregion window methods

        #region help window

        public void Help()
        {
            Console.WriteLine("Open Help Window");
        }

        public void About()
        {
            Console.WriteLine("Open  Window");
        }

        #endregion help window

        public void TestAddClosedDocks()
        {
            var closedDock1 = new MenuItem() { Header = "Item Editor", InputGestureText = "Ctrl+1" };
            var closedDock2 = new MenuItem() { Header = "Minimap", InputGestureText = "Ctrl+2" };

            RecentlyClosedDockItems.Add(closedDock1);
            RecentlyClosedDockItems.Add(closedDock2);
        }

        #endregion methods
    }
}
