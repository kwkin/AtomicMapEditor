using System;
using System.Windows;
using AtomicMapEditor.Modules.MapEditor;
using AtomicMapEditor.Modules.MapEditor.Editor;
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

            ViewModelLocationProvider.Register<MainEditor, MainEditorViewModel>();
        }
    }
}
