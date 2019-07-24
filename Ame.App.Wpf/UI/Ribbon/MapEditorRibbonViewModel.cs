using Ame.App.Wpf.UI.Docks.ItemEditorDock;
using Ame.App.Wpf.UI.Interactions.LayerProperties;
using Ame.App.Wpf.UI.Interactions.MapProperties;
using Ame.App.Wpf.UI.Interactions.TilesetProperties;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Ribbon
{
    public class MapEditorRibbonViewModel
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public MapEditorRibbonViewModel(IEventAggregator eventAggregator, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.Session = session ?? throw new ArgumentNullException("session");

            this.ZoomLevels = ZoomLevel.CreateZoomList(0.125, 0.25, 0.5, 1, 2, 4, 8, 16, 32);
            this.ZoomLevels.OrderBy(f => f.zoom);

            // Map bindings
            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.EditMapPropertiesCommand = new DelegateCommand(() => EditMapProperties());

            // Layer bindings
            this.NewLayerCommand = new DelegateCommand(() => NewLayer());
            this.EditLayerPropertiesCommand = new DelegateCommand(() => EditLayerProperties());

            // Item bindings
            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.OpenItemListCommand = new DelegateCommand(() => OpenItemList());

            // Zoom bindings
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetZoomCommand = new DelegateCommand<ZoomLevel>((zoomLevel) => SetZoom(zoomLevel));

            // View bindings
            this.SampleViewCommand = new DelegateCommand(() => SampleView());
            this.CollisionsViewCommand = new DelegateCommand(() => CollisionsView());

            // File bindings
            this.SaveFileCommand = new DelegateCommand(() => SaveFile());
            this.ExportFileCommand = new DelegateCommand(() => ExportFile());
        }

        #endregion constructor


        #region properties

        // Map Menu
        public ICommand NewMapCommand { get; set; }

        public ICommand EditMapPropertiesCommand { get; set; }

        // Layer Menu
        public ICommand NewLayerCommand { get; set; }

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

        // File Menu
        public ICommand SaveFileCommand { get; set; }

        public ICommand ExportFileCommand { get; set; }

        public ObservableCollection<ZoomLevel> ZoomLevels { get; set; }

        public IAmeSession Session { get; set; }
        public bool ISTHISFUCKINGBROKEN { get; set; }

        #endregion properties


        #region methods

        #region map methods

        public void NewMap()
        {
            NewMapInteraction interaction = new NewMapInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void EditMapProperties()
        {
            EditMapInteraction interaction = new EditMapInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        #endregion map methods

        #region layer methods

        public void NewLayer()
        {
            NewLayerInteraction interaction = new NewLayerInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void EditLayerProperties()
        {
            EditLayerInteraction interaction = new EditLayerInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        #endregion layer methods

        #region item methods

        public void AddTileset()
        {
            NewTilesetInteraction interaction = new NewTilesetInteraction(OnNewTilesetWindowClosed);
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
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
            NotificationMessage<ViewNotification> message = new NotificationMessage<ViewNotification>(ViewNotification.ZoomInDocument);
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Publish(message);
        }

        public void ZoomOut()
        {
            NotificationMessage<ViewNotification> message = new NotificationMessage<ViewNotification>(ViewNotification.ZoomOutDocument);
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Publish(message);
        }

        public void SetZoom(ZoomLevel zoomLevel)
        {
            NotificationMessage<ZoomLevel> message = new NotificationMessage<ZoomLevel>(zoomLevel);
            this.eventAggregator.GetEvent<NotificationEvent<ZoomLevel>>().Publish(message);
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


        private void OnNewTilesetWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (Mouse.OverrideCursor == Cursors.Pen)
            {
                Mouse.OverrideCursor = null;
            }
            if (confirmation.Confirmed)
            {
                TilesetModel tilesetModel = confirmation.Content as TilesetModel;
                this.Session.CurrentTilesets.Add(tilesetModel);

                OpenDockMessage message = new OpenDockMessage(typeof(ItemEditorViewModel));
                message.IgnoreIfExists = true;
                message.Content = tilesetModel;
                this.eventAggregator.GetEvent<OpenDockEvent>().Publish(message);
            }
        }

        #endregion methods
    }
}
