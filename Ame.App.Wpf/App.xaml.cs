using Ame.App.Wpf.UI;
using Ame.App.Wpf.UI.Editor.MapEditor;
using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.App.Wpf.UI.Interactions.Preferences;
using Ame.App.Wpf.UI.Interactions.ProjectProperties;
using Ame.App.Wpf.UI.Interactions.TilesetProperties;
using Ame.App.Wpf.UI.Menu;
using Ame.App.Wpf.UI.Ribbon;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.IO;
using System.Windows;

namespace Ame.App.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public SessionManager SessionManager { get; private set; }

        protected override Window CreateShell()
        {
            this.SessionManager = this.Container.Resolve<SessionManager>();

            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            AmeSession session = new AmeSession();
            try
            {
                if (File.Exists(Global.SessionFileName))
                {
                    AmeSessionJsonReader reader = new AmeSessionJsonReader();
                    session = reader.Read(Global.SessionFileName);
                }
            }
            catch (Exception e)
            {
            }
            containerRegistry.RegisterInstance(typeof(AmeSession), session);

            ViewModelLocationProvider.Register<MenuOptions, MenuOptionsViewModel>();
            ViewModelLocationProvider.Register<MapEditorDocument, MapEditorViewModel>();
            ViewModelLocationProvider.Register<MapEditorRibbon, MapEditorRibbonViewModel>();
            ViewModelLocationProvider.Register<WindowManager, WindowManagerViewModel>();

            ViewModelLocationProvider.Register<EditProjectWindow, ProjectPropertiesViewModel>();
            ViewModelLocationProvider.Register<NewProjectWindow, ProjectPropertiesViewModel>();
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
