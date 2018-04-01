using System;
using System.Windows;
using AtomicMapEditor.Modules.Docks;
using AtomicMapEditor.Modules.Docks.Core;
using AtomicMapEditor.Modules.MapEditor;
using AtomicMapEditor.Modules.MapEditor.Editor;
using AtomicMapEditor.Modules.Menu;
using AtomicMapEditor.Modules.Menu.Options;
using AtomicMapEditor.Modules.Windows;
using AtomicMapEditor.Modules.Windows.MapEditorWindow;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;

namespace AtomicMapEditor
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

            ViewModelLocationProvider.Register<MenuOptions, MenuOptionsViewModel>();
            ViewModelLocationProvider.Register<MainEditor, MainEditorViewModel>();
            ViewModelLocationProvider.Register<DockManager, DockManagerViewModel>();

            ViewModelLocationProvider.Register<MapEditor, MapEditorViewModel>();
        }
    }
}
