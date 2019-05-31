using System;
using System.Windows;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows;
using Ame.Modules.MapEditor;
using Ame.Modules.MapEditor.Editor;
using Ame.Modules.Menu;
using Ame.Modules.Menu.Options;
using Ame.Modules.Windows.Interactions.TilesetProperties;
using Ame.Modules.Windows.Interactions.LayerProperties;
using Ame.Modules.Windows.Interactions.MapProperties;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using Ame.Modules.Windows.Interactions.Preferences;
using Ame.Modules.Menu.Ribbon;
using System.IO;
using Ame.Infrastructure.Core;
using Newtonsoft.Json;

namespace Ame
{
    internal class AtomicMapEditorBootstrapper : UnityBootstrapper
    {
        public SessionManager SessionManager { get; private set; }

        protected override void ConfigureModuleCatalog()
        {
            Type canvasEditorModule = typeof(MapEditorModules);
            this.ModuleCatalog.AddModule(new ModuleInfo(canvasEditorModule.Name, canvasEditorModule.AssemblyQualifiedName));

            Type menuModule = typeof(MenuModule);
            this.ModuleCatalog.AddModule(new ModuleInfo(menuModule.Name, menuModule.AssemblyQualifiedName));

            Type dockModule = typeof(WindowModule);
            this.ModuleCatalog.AddModule(new ModuleInfo(dockModule.Name, dockModule.AssemblyQualifiedName));
        }

        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            
            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();

            // TODO standardize paths
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string directoryPath = Path.Combine(documentPath, Global.applicationName);
            string sessionFile = Path.Combine(directoryPath, Global.sessionFileName);
            AmeSession.AmeSessionJson sessionJson = JsonConvert.DeserializeObject<AmeSession.AmeSessionJson>(File.ReadAllText(sessionFile));
            AmeSession session = sessionJson.Generate();

            this.Container.RegisterInstance<AmeSession>(session);
            this.SessionManager = this.Container.Resolve<SessionManager>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            ViewModelLocationProvider.Register<MenuOptions, MenuOptionsViewModel>();
            ViewModelLocationProvider.Register<MapEditorDocument, MapEditorViewModel>();
            ViewModelLocationProvider.Register<MapEditorRibbon, MapEditorRibbonViewModel>();
            ViewModelLocationProvider.Register<WindowManager, WindowManagerViewModel>();
            
            ViewModelLocationProvider.Register<EditMapWindow, MapPropertiesViewModel>();
            ViewModelLocationProvider.Register<NewMapWindow, MapPropertiesViewModel>();
            ViewModelLocationProvider.Register<EditLayerWindow, LayerPropertiesViewModel>();
            ViewModelLocationProvider.Register<NewLayerWindow, LayerPropertiesViewModel>();
            ViewModelLocationProvider.Register<EditTilesetWindow, TilesetPropertiesViewModel>();
            ViewModelLocationProvider.Register<NewTilesetWindow, TilesetPropertiesViewModel>();
            ViewModelLocationProvider.Register<PreferencesMenu, PreferencesViewModel>();

        }
    }
}
