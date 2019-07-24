using Ame.App.Wpf.UI.Docks.ClipboardDock;
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
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using Ame.Infrastructure.Utils;
using Ame.App.Wpf.UI.Interactions.FileChooser;

namespace Ame.App.Wpf.UI.Menu
{
    public class MenuOptionsViewModel
    {
        #region fields

        private IActionHandler actionHandler;

        #endregion fields


        #region constructor

        public MenuOptionsViewModel(IAmeSession session, IActionHandler actionHandler)
        {
            this.Session = session ?? throw new ArgumentNullException("session");
            this.actionHandler = actionHandler ?? throw new ArgumentNullException("actionHandler");

            this.TestAddClosedDocksCommand = new DelegateCommand(() => TestAddClosedDocks());
            this.ConfirmationRequest = new InteractionRequest<IConfirmation>();

            this.RecentlyClosedDockItems = new ObservableCollection<MenuItem>();
            this.RecentFileItems = new ObservableCollection<MenuItem>();

            // File bindings
            this.NewProjectCommand = new DelegateCommand(() => NewProject());
            this.OpenProjectCommand = new DelegateCommand(() => this.actionHandler.OpenProject());
            this.OpenMapCommand = new DelegateCommand(() => OpenMap());
            this.SaveFileCommand = new DelegateCommand(() => this.actionHandler.SaveCurrentMap());
            this.SaveAsFileCommand = new DelegateCommand(() => OpenSaveMap());
            this.ExportFileCommand = new DelegateCommand(() => this.actionHandler.ExportFile());
            this.ExportAsFileCommand = new DelegateCommand(() => ExportAsFile());
            this.ImportFileCommand = new DelegateCommand(() => this.actionHandler.ImportFile());
            this.ViewFilePropertiesCommand = new DelegateCommand(() => this.actionHandler.ViewFileProperties());
            this.CloseFileCommand = new DelegateCommand(() => this.actionHandler.CloseFile());
            this.CloseAllFilesCommand = new DelegateCommand(() => this.actionHandler.CloseAllFiles());
            this.ExitProgramCommand = new DelegateCommand(() => this.actionHandler.ExitProgram());

            // Edit bindings
            this.UndoCommand = new DelegateCommand(() => this.actionHandler.Undo());
            this.RedoCommand = new DelegateCommand(() => this.actionHandler.Redo());
            this.CutCommand = new DelegateCommand(() => this.actionHandler.CutSelection());
            this.CopyCommand = new DelegateCommand(() => this.actionHandler.CopySelection());
            this.PasteCommand = new DelegateCommand(() => this.actionHandler.PasteClipboard());
            this.OpenClipboardCommand = new DelegateCommand(() => OpenClipboardDock());
            this.OpenPreferencesCommand = new DelegateCommand(() => OpenPreferenencesWindow());

            // Map bindings
            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.DuplicateMapCommand = new DelegateCommand(() => this.actionHandler.DuplicateMap());
            this.FlipMapHorizontallyCommand = new DelegateCommand(() => this.actionHandler.FlipMapHorizontally());
            this.FlipMapVerticallyCommand = new DelegateCommand(() => this.actionHandler.FlipMapVertically());
            this.GuillotineMapCommand = new DelegateCommand(() => this.actionHandler.GuillotineMap());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());

            // Layer bindings
            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.NewGroupCommand = new DelegateCommand(() => this.actionHandler.NewGroup());
            this.DuplicateLayerCommand = new DelegateCommand(() => this.actionHandler.DuplicateLayer());
            this.MergeLayerDownCommand = new DelegateCommand(() => this.actionHandler.MergeLayerDown());
            this.MergeLayerUpCommand = new DelegateCommand(() => this.actionHandler.MergeLayerUp());
            this.MergeVisibleCommand = new DelegateCommand(() => this.actionHandler.MergeVisible());
            this.DeleteLayerCommand = new DelegateCommand(() => this.actionHandler.DeleteLayer());
            this.EditLayerPropertiesCommand = new DelegateCommand(() => EditLayerProperties());
            this.LayerToMapCommand = new DelegateCommand(() => this.actionHandler.LayerToMap());

            // Item bindings
            this.AddTilesetCommand = new DelegateCommand(() => this.actionHandler.AddTileset());
            this.AddImageCommand = new DelegateCommand(() => this.actionHandler.AddImage());
            this.AddGroupCommand = new DelegateCommand(() => this.actionHandler.AddGroup());
            this.EditItemPropertiesCommand = new DelegateCommand(() => this.actionHandler.EditItemProperties());
            this.EditItemCollisionsCommand = new DelegateCommand(() => this.actionHandler.EditItemCollisions());

            // View bindings
            this.SampleViewCommand = new DelegateCommand(() => this.actionHandler.SampleView());
            this.CollisionsViewCommand = new DelegateCommand(() => this.actionHandler.CollisionsView());
            this.ZoomInCommand = new DelegateCommand(() => this.actionHandler.ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => this.actionHandler.ZoomOut());
            this.ZoomToolCommand = new DelegateCommand(() => this.actionHandler.ZoomTool());
            this.FitMapToWindowCommand = new DelegateCommand(() => this.actionHandler.FitMapToWindow());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => this.actionHandler.SetZoom(zoomLevel));
            this.DockPresetViewCommand = new DelegateCommand(() => this.actionHandler.DockPresetView());
            this.ShowGridCommand = new DelegateCommand(() => this.actionHandler.ShowGrid(this.IsShowGrid));
            this.ShowRulerCommand = new DelegateCommand(() => this.actionHandler.ShowRuler(this.IsShowRuler));
            this.ShowScrollBarCommand = new DelegateCommand(() => this.actionHandler.ShowScrollBar(this.IsShowScrollBar));

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
            this.HideDocksCommand = new DelegateCommand(() => this.actionHandler.HideDocks());
            this.SingleWindowCommand = new DelegateCommand(() => this.actionHandler.SingleWindow());

            // Help bindings
            this.HelpCommand = new DelegateCommand(() => this.actionHandler.Help());
            this.AboutCommand = new DelegateCommand(() => this.actionHandler.About());
        }

        #endregion constructor


        #region properties

        public ICommand NewProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand OpenMapCommand { get; private set; }
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
        public ObservableCollection<MenuItem> RecentlyClosedDockItems { get; set; }
        public ObservableCollection<MenuItem> RecentFileItems { get; set; }

        public IAmeSession Session { get; set; }

        public bool IsShowGrid { get; set; }
        public bool IsShowRuler { get; set; }
        public bool IsShowScrollBar { get; set; }

        #endregion properties


        #region methods

        public void NewProject()
        {
            NewProjectInteraction interaction = new NewProjectInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void NewMap()
        {
            NewMapInteraction interaction = new NewMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void NewLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void EditMapProperties()
        {
            EditMapInteraction interaction = new EditMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void EditLayerProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void OpenMap()
        {
            OpenMapInteraction interaction = new OpenMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void OpenSaveMap()
        {
            SaveMapInteraction interaction = new SaveMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void ExportAsFile()
        {
            ExportMapInteraction interaction = new ExportMapInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void OpenPreferenencesWindow()
        {
            PreferenceOptionsInteraction interaction = new PreferenceOptionsInteraction();
            this.actionHandler.OpenWindow(interaction);
        }

        public void OpenClipboardDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(ClipboardViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        public void OpenItemEditorDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(ItemEditorViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        public void OpenItemListDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(ItemListViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        public void OpenLayerListDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(LayerListViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        public void OpenToolboxDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(ToolboxViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        public void OpenMinimapDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(MinimapViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        public void OpenSelectedBrushDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(SelectedBrushViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        // TODO implement undo history dock
        public void OpenUndoHistoryDock()
        {
            Console.WriteLine("Opening undo history");
        }

        public void OpenProjectExplorerDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(ProjectExplorerViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

        public void OpenSessionViewerDock()
        {
            OpenDockMessage openDockMessage = new OpenDockMessage(typeof(SessionViewerViewModel));
            this.actionHandler.OpenDock(openDockMessage);
        }

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
