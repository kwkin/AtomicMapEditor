﻿using Ame.App.Wpf.UI.Docks.ClipboardDock;
using Ame.App.Wpf.UI.Docks.ItemEditorDock;
using Ame.App.Wpf.UI.Docks.ItemListDock;
using Ame.App.Wpf.UI.Docks.LayerListDock;
using Ame.App.Wpf.UI.Docks.MinimapDock;
using Ame.App.Wpf.UI.Docks.ProjectExplorerDock;
using Ame.App.Wpf.UI.Docks.SelectedBrushDock;
using Ame.App.Wpf.UI.Docks.SessionViewerDock;
using Ame.App.Wpf.UI.Docks.ToolboxDock;
using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.App.Wpf.UI.Interactions.Preferences;
using Ame.App.Wpf.UI.Interactions.ProjectProperties;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Ame.App.Wpf.UI.Menu
{
    public class MenuOptionsViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;

        private string filePath;
        private string fileType;

        #endregion fields


        #region constructor

        public MenuOptionsViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.Session = session ?? throw new ArgumentNullException("session");

            this.TestAddClosedDocksCommand = new DelegateCommand(() => TestAddClosedDocks());
            this.ConfirmationRequest = new InteractionRequest<IConfirmation>();

            this.recentlyClosedDockItems = new ObservableCollection<MenuItem>();
            this.recentFileItems = new ObservableCollection<MenuItem>();

            // File bindings
            this.NewProjectCommand = new DelegateCommand(() => NewProject());
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
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => SetZoom(zoomLevel));
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
            this.OpenProjectExplorerDockCommand = new DelegateCommand(() => OpenProjectExplorerDock());
            this.OpenSessionViewDockCommand = new DelegateCommand(() => OpenSessionViewerDock());
            this.HideDocksCommand = new DelegateCommand(() => HideDocks());
            this.SingleWindowCommand = new DelegateCommand(() => SingleWindow());

            // Help bindings
            this.HelpCommand = new DelegateCommand(() => Help());
            this.AboutCommand = new DelegateCommand(() => About());
        }

        #endregion constructor


        #region properties

        public ICommand NewProjectCommand { get; private set; }
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
        public ICommand OpenProjectExplorerDockCommand { get; private set; }
        public ICommand OpenSessionViewDockCommand { get; private set; }
        public ICommand HideDocksCommand { get; private set; }
        public ICommand SingleWindowCommand { get; private set; }

        public ICommand HelpCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }

        public ICommand TestAddClosedDocksCommand { get; private set; }

        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }


        private ObservableCollection<MenuItem> recentlyClosedDockItems;
        public ObservableCollection<MenuItem> RecentlyClosedDockItems
        {
            get
            {
                return recentlyClosedDockItems;
            }
            set
            {
                recentlyClosedDockItems = value;
            }
        }

        private ObservableCollection<MenuItem> recentFileItems;
        public ObservableCollection<MenuItem> RecentFileItems
        {
            get
            {
                return recentFileItems;
            }
            set
            {
                recentFileItems = value;
            }
        }

        public AmeSession Session { get; set; }

        public bool IsShowGrid { get; set; }
        public bool IsShowRuler { get; set; }
        public bool IsShowScrollBar { get; set; }

        #endregion properties


        #region methods

        #region file methods

        public void NewProject()
        {
            NewProjectInteraction interaction = new NewProjectInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void OpenFile()
        {
            OpenFileDialog openMapDialog = new OpenFileDialog();
            openMapDialog.Title = "Open Map";
            openMapDialog.Filter = SaveMapExtension.GetOpenMapSaveExtensions();
            openMapDialog.InitialDirectory = this.Session.LastMapDirectory;
            if (openMapDialog.ShowDialog() == true)
            {
                string dialogFilePath = openMapDialog.FileName;
                if (File.Exists(dialogFilePath))
                {
                    this.filePath = dialogFilePath;
                    this.fileType = openMapDialog.Filter;
                    this.Session.LastMapDirectory = Directory.GetParent(openMapDialog.FileName).FullName;

                    MapJsonReader reader = new MapJsonReader();
                    ResourceLoader loader = ResourceLoader.Instance;
                    Map importedMap = loader.Load<Map>(this.filePath, reader);

                    OpenMapMessage message = new OpenMapMessage(importedMap);
                    NotificationMessage<OpenMapMessage> notification = new NotificationMessage<OpenMapMessage>(message);
                    this.eventAggregator.GetEvent<NotificationEvent<OpenMapMessage>>().Publish(notification);
                }
            }
        }

        public void SaveFile()
        {
            Console.WriteLine("Save File");
        }

        public void SaveAsFile()
        {
            SaveFileDialog saveMapDialog = new SaveFileDialog();
            saveMapDialog.Title = "Save Map";
            saveMapDialog.Filter = SaveMapExtension.GetOpenMapSaveExtensions();
            saveMapDialog.InitialDirectory = this.Session.LastMapDirectory;
            if (saveMapDialog.ShowDialog() == true)
            {
                this.filePath = saveMapDialog.FileName;
                this.fileType = saveMapDialog.Filter;
                this.Session.LastMapDirectory = Directory.GetParent(saveMapDialog.FileName).FullName;

                SaveMessage message = new SaveMessage(this.filePath, this.Session.CurrentMap.Value);
                NotificationMessage<SaveMessage> notification = new NotificationMessage<SaveMessage>(message);
                this.eventAggregator.GetEvent<NotificationEvent<SaveMessage>>().Publish(notification);
            }
        }

        public void ExportFile()
        {
            Console.WriteLine("Export File");
        }

        public void ExportAsFile()
        {
            SaveFileDialog exportMapDialog = new SaveFileDialog();
            exportMapDialog.Title = "Export Map";
            exportMapDialog.Filter = ExportMapExtension.GetOpenFileExportMapExtensions();
            if (exportMapDialog.ShowDialog() == true)
            {
                this.filePath = exportMapDialog.FileName;
                this.fileType = exportMapDialog.Filter;

                StateMessage message = new StateMessage(this.filePath);
                BitmapEncoder encoder = ExportMapExtension.getEncoder(this.fileType);
                message.Encoder = encoder;
                NotificationMessage<StateMessage> notification = new NotificationMessage<StateMessage>(message);
                this.eventAggregator.GetEvent<NotificationEvent<StateMessage>>().Publish(notification);
            }
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
            PreferenceOptionsInteraction interaction = new PreferenceOptionsInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        #endregion edit methods

        #region map methods

        public void NewMap()
        {
            NewMapInteraction interaction = new NewMapInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
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
            EditMapInteraction interaction = new EditMapInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        #endregion map methods

        #region layer methods

        public void NewLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void NewGroup()
        {
            NotificationMessage<LayerNotification> newLayerGroupMessage = new NotificationMessage<LayerNotification>(LayerNotification.NewLayerGroup, "LayerGroup");
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(newLayerGroupMessage);
        }

        public void DuplicateLayer()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.DuplicateCurrentLayer);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void MergeLayerDown()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.MergeCurrentLayerDown);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void MergeLayerUp()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.MergeCurrentLayerDown);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void MergeVisible()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.MergeVisibleLayers);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void DeleteLayer()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.DeleteCurrentLayer);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void EditLayerProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
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
            NotificationMessage<ViewNotification> message = new NotificationMessage<ViewNotification>(ViewNotification.ZoomInDocument);
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Publish(message);
        }

        public void ZoomOut()
        {
            NotificationMessage<ViewNotification> message = new NotificationMessage<ViewNotification>(ViewNotification.ZoomOutDocument);
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Publish(message);
        }

        public void ZoomTool()
        {
            Console.WriteLine("Zoom Tool");
        }

        public void FitMapToWindow()
        {
            Console.WriteLine("Fit Map To Window");
        }

        public void SetZoom(ZoomLevel zoomLevel)
        {
            NotificationMessage<ZoomLevel> message = new NotificationMessage<ZoomLevel>(zoomLevel);
            this.eventAggregator.GetEvent<NotificationEvent<ZoomLevel>>().Publish(message);
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

        public void OpenProjectExplorerDock()
        {
            OpenDockMessage dock = new OpenDockMessage(typeof(ProjectExplorerViewModel));
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

            this.RecentlyClosedDockItems.Add(closedDock1);
            this.RecentlyClosedDockItems.Add(closedDock2);
        }

        #endregion methods
    }
}
