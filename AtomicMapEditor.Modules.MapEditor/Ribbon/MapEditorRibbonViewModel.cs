using System;
using System.Windows.Input;
using Ame.Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Ame.Modules.MapEditor.Ribbon
{
    public class MapEditorRibbonViewModel : BindableBase
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields

        #region constructor

        public MapEditorRibbonViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.eventAggregator = eventAggregator;

            // Map bindings
            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.DuplicateMapCommand = new DelegateCommand(() => DuplicateMap());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());

            // Layer bindings
            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.DuplicateLayerCommand = new DelegateCommand(() => DuplicateLayer());
            this.EditLayerPropertiesCommand = new DelegateCommand(() => EditLayerProperties());

            // Item bindings
            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.OpenItemListCommand = new DelegateCommand(() => OpenItemList());

            // Zoom bindings
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand(() => SetZoom());

            // View bindings
            this.SampleViewCommand = new DelegateCommand(() => SampleView());
            this.CollisionsViewCommand = new DelegateCommand(() => CollisionsView());

            // Window bindings
            this.OpenDockCommand = new DelegateCommand(() => OpenDock());
            this.RecentlyClosedDockCommand = new DelegateCommand(() => RecentlyClosedDock());
            this.DockPresetCommand = new DelegateCommand(() => DockPreset());
            this.SnapDockCommand = new DelegateCommand(() => SnapDock());
            this.HideDocksCommand = new DelegateCommand(() => HideDocks());

            // File bindings
            this.SaveFileCommand = new DelegateCommand(() => SaveFile());
            this.ExportFileCommand = new DelegateCommand(() => ExportFile());
        }

        #endregion constructor


        #region properties

        // Map Menu
        public ICommand NewMapCommand { get; set; }

        public ICommand DuplicateMapCommand { get; set; }
        public ICommand EditMapPropertiesCommand { get; set; }

        // Layer Menu
        public ICommand NewLayerCommand { get; set; }

        public ICommand DuplicateLayerCommand { get; set; }
        public ICommand EditLayerPropertiesCommand { get; set; }

        // Item Menu
        public ICommand AddTilesetCommand { get; set; }

        public ICommand AddImageCommand { get; set; }
        public ICommand OpenItemListCommand { get; set; }

        // Zoom Menu
        public ICommand ZoomInCommand { get; set; }

        public ICommand ZoomOutCommand { get; set; }
        public ICommand SetZoomCommand { get; set; }

        // View Menu
        public ICommand SampleViewCommand { get; set; }

        public ICommand CollisionsViewCommand { get; set; }

        // Window Menu
        public ICommand OpenDockCommand { get; set; }

        public ICommand RecentlyClosedDockCommand { get; set; }
        public ICommand DockPresetCommand { get; set; }
        public ICommand SnapDockCommand { get; set; }
        public ICommand HideDocksCommand { get; set; }

        // File Menu
        public ICommand SaveFileCommand { get; set; }

        public ICommand ExportFileCommand { get; set; }


        #endregion properties


        #region methods

        #region map methods

        public void NewMap()
        {
            OpenWindowMessage window = new OpenWindowMessage(WindowType.Map);
            window.WindowTitle = "New Map";
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(window);
        }

        public void DuplicateMap()
        {
            Console.WriteLine("Duplicate Map");
        }

        public void EditMapProperties()
        {
            Console.WriteLine("Edit Map Properties");
        }

        #endregion map methods

        #region layer methods

        public void NewLayer()
        {
            Console.WriteLine("Open Layer Editor");
        }

        public void DuplicateLayer()
        {
            Console.WriteLine("Duplicate Layer");
        }

        public void EditLayerProperties()
        {
            Console.WriteLine("Edit Layer Properties");
        }

        #endregion layer methods

        #region item methods

        public void AddTileset()
        {
            Console.WriteLine("Adding Tileset");
        }

        public void AddImage()
        {
            Console.WriteLine("Adding Image");
        }

        public void OpenItemList()
        {
            Console.WriteLine("Open Item List");
        }

        #endregion item methods

        #region zoom methods

        public void ZoomIn()
        {
            Console.WriteLine("Zoom in");
        }

        public void ZoomOut()
        {
            Console.WriteLine("Zoom out");
        }

        public void SetZoom()
        {
            Console.WriteLine("Set Zoom");
        }

        #endregion zoom methods

        #region view methods

        public void SampleView()
        {
            Console.WriteLine("Sample View");
        }

        public void CollisionsView()
        {
            Console.WriteLine("Collisions View");
        }

        #endregion view methods

        #region dock methods

        public void OpenDock()
        {
            Console.WriteLine("Sample View");
        }

        public void RecentlyClosedDock()
        {
            Console.WriteLine("Recently Closed Docks");
        }

        public void DockPreset()
        {
            Console.WriteLine("Dock Preset");
        }

        public void SnapDock()
        {
            Console.WriteLine("Snap Dock");
        }

        public void HideDocks()
        {
            Console.WriteLine("Hide Dock");
        }

        #endregion dock methods

        #region file methods

        public void SaveFile()
        {
            Console.WriteLine("Save File");
        }

        public void ExportFile()
        {
            Console.WriteLine("Export File");
        }

        #endregion file methods

        #endregion methods
    }
}
