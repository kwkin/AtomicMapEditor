using System;
using System.Windows;
using Ame.Infrastructure.Models;
using Ame.Modules.Docks;
using Ame.Modules.MapEditor;
using Ame.Modules.MapEditor.Editor;
using Ame.Modules.Menu;
using Ame.Modules.Menu.Options;
using Ame.Modules.Windows;
using Ame.Modules.Windows.LayerEditorWindow;
using Ame.Modules.Windows.MapEditorWindow;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;

namespace Ame
{
    internal class AtomicMapEditorBootstrapper : UnityBootstrapper
    {
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
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            Container.RegisterInstance<AmeSession>(new AmeSession(new Map("Map #1")));

            ViewModelLocationProvider.Register<MenuOptions, MenuOptionsViewModel>();
            ViewModelLocationProvider.Register<MainEditor, MainEditorViewModel>();
            ViewModelLocationProvider.Register<DockManager, DockManagerViewModel>();

            ViewModelLocationProvider.Register<MapEditor, MapEditorViewModel>();
            ViewModelLocationProvider.Register<LayerEditor, LayerEditorViewModel>();
            
        }
    }
}
