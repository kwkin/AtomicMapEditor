using System;
using System.Windows;
using Ame.Infrastructure.Models;
using Ame.Modules.Docks;
using Ame.Modules.MapEditor;
using Ame.Modules.MapEditor.Editor;
using Ame.Modules.Menu;
using Ame.Modules.Menu.Options;
using Ame.Modules.Windows;
using Ame.Modules.Windows.TilesetEditorWindow;
using Ame.Modules.Windows.LayerEditorWindow;
using Ame.Modules.Windows.MapEditorWindow;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using Ame.Modules.Windows.PreferencesWindow;

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

            Type dockModule = typeof(DocksModule);
            this.ModuleCatalog.AddModule(new ModuleInfo(dockModule.Name, dockModule.AssemblyQualifiedName));

            Type windowModule = typeof(WindowsModule);
            this.ModuleCatalog.AddModule(new ModuleInfo(windowModule.Name, windowModule.AssemblyQualifiedName));
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

            Map map = new Map("Map #1");
            map.LayerList.Add(new Layer("Layer #1", 32, 32, 32, 32));
            this.Container.RegisterInstance<AmeSession>(new AmeSession(map));
            this.SessionManager = this.Container.Resolve<SessionManager>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            ViewModelLocationProvider.Register<MenuOptions, MenuOptionsViewModel>();
            ViewModelLocationProvider.Register<MapEditorDocument, Modules.MapEditor.Editor.MapEditorViewModel>();
            ViewModelLocationProvider.Register<DockManager, DockManagerViewModel>();

            ViewModelLocationProvider.Register<MapEditorWindow, Modules.Windows.MapEditorWindow.MapEditorViewModel>();
            ViewModelLocationProvider.Register<LayerEditor, LayerEditorViewModel>();
            ViewModelLocationProvider.Register<TilesetEditor, TilesetEditorViewModel>();
            ViewModelLocationProvider.Register<PreferencesMenu, PreferencesViewModel>();

        }
    }
}
