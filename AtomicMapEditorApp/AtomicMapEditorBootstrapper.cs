using System;
using System.Windows;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows;
using Ame.Modules.MapEditor;
using Ame.Modules.MapEditor.Editor;
using Ame.Modules.Menu;
using Ame.Modules.Menu.Options;
using Ame.Modules.Windows.Interactions.TilesetEditorInteraction;
using Ame.Modules.Windows.Interactions.LayerPropertiesInteraction;
using Ame.Modules.Windows.Interactions.MapPropertiesInteraction;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using Ame.Modules.Windows.Interactions.PreferencesInteraction;
using Ame.Modules.Menu.Ribbon;

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
            
            this.Container.RegisterInstance<AmeSession>(new AmeSession());
            this.SessionManager = this.Container.Resolve<SessionManager>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            ViewModelLocationProvider.Register<MenuOptions, MenuOptionsViewModel>();
            ViewModelLocationProvider.Register<MapEditorDocument, MapEditorViewModel>();
            ViewModelLocationProvider.Register<MapEditorRibbon, MapEditorRibbonViewModel>();
            ViewModelLocationProvider.Register<WindowManager, WindowManagerViewModel>();
            
            ViewModelLocationProvider.Register<MapPropertiesWindow, MapPropertiesViewModel>();
            ViewModelLocationProvider.Register<LayerPropertiesWindow, LayerPropertiesViewModel>();
            ViewModelLocationProvider.Register<TilesetPropertiesWindow, TilesetPropertiesViewModel>();
            ViewModelLocationProvider.Register<PreferencesMenu, PreferencesViewModel>();

        }
    }
}
