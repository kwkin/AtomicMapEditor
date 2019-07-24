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
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
using DryIoc;
using Prism.DryIoc;
using Prism.Events;
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
            IEventAggregator eventAggregator = containerRegistry.GetContainer().Resolve<IEventAggregator>();
            IConstants constants = new Constants();
            IAmeSession session = new AmeSession(constants);
            try
            {
                if (File.Exists(constants.SessionFileName))
                {
                    IAmeSessionJsonReader reader = new IAmeSessionJsonReader();
                    session = reader.Read(constants.SessionFileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            IActionHandler actionHandler = new ActionHandler(eventAggregator, session);

            containerRegistry.RegisterInstance(typeof(IConstants), constants);
            containerRegistry.RegisterInstance(typeof(IAmeSession), session);
            containerRegistry.RegisterInstance(typeof(IActionHandler), actionHandler);

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
