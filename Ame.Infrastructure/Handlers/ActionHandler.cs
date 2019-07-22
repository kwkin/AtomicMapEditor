using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
using Ame.Infrastructure.UILogic;
using Ame.Infrastructure.Utils;
using Microsoft.Win32;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ame.Infrastructure.Handlers
{
    // TODO check how this can be moved to the infrastructure package
    public class ActionHandler : IActionHandler
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructor

        public ActionHandler(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session");
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public void OpenWindow(IWindowInteraction interaction)
        {
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public void OpenDock(OpenDockMessage dockMessage)
        {
            this.eventAggregator.GetEvent<OpenDockEvent>().Publish(dockMessage);
        }

        public void OpenProject()
        {
            Console.WriteLine("Open project");
        }

        public void OpenMap()
        {
            OpenFileDialog openMapDialog = new OpenFileDialog();
            openMapDialog.Title = "Open Map";
            openMapDialog.Filter = SaveMapExtension.GetOpenMapSaveExtensions();
            openMapDialog.InitialDirectory = this.session.LastMapDirectory.Value;
            if (openMapDialog.ShowDialog() == true)
            {
                string dialogFilePath = openMapDialog.FileName;
                if (File.Exists(dialogFilePath))
                {
                    this.session.LastMapDirectory.Value = Directory.GetParent(openMapDialog.FileName).FullName;

                    MapJsonReader reader = new MapJsonReader();
                    ResourceLoader loader = ResourceLoader.Instance;
                    Map importedMap = loader.Load<Map>(dialogFilePath, reader);

                    OpenMapMessage message = new OpenMapMessage(importedMap);
                    NotificationMessage<OpenMapMessage> notification = new NotificationMessage<OpenMapMessage>(message);
                    this.eventAggregator.GetEvent<NotificationEvent<OpenMapMessage>>().Publish(notification);
                }
            }
        }

        public void SaveCurrentMap()
        {
            Map currentMap = this.session.CurrentMap.Value;
            SaveMap(currentMap);
        }

        public void SaveMap(Map map)
        {
            if (map.SourcePath.Value == null)
            {
                SaveAsMap();
            }
            else
            {
                map.UpdateFile();
            }
        }

        // TODO move this to view model class. Instead maybe pass the file to save and the file location
        public void SaveAsMap()
        {
            SaveFileDialog saveMapDialog = new SaveFileDialog();
            saveMapDialog.Title = "Save Map";
            saveMapDialog.Filter = SaveMapExtension.GetOpenMapSaveExtensions();
            saveMapDialog.InitialDirectory = this.session.LastMapDirectory.Value;
            if (saveMapDialog.ShowDialog().Value)
            {
                Map currentMap = this.session.CurrentMap.Value;
                string file = saveMapDialog.FileName;
                currentMap.WriteFile(file);

                this.session.LastMapDirectory.Value = Directory.GetParent(saveMapDialog.FileName).FullName;
            }
        }
        public void ExportFile()
        {
            Console.WriteLine("Export file");
        }

        public void ExportAsFile()
        {
            SaveFileDialog exportMapDialog = new SaveFileDialog();
            exportMapDialog.Title = "Export Map";
            exportMapDialog.Filter = ExportMapExtension.GetOpenFileExportMapExtensions();
            if (exportMapDialog.ShowDialog() == true)
            {
                string file = exportMapDialog.FileName;
                string fileType = exportMapDialog.Filter;

                StateMessage message = new StateMessage(file);
                BitmapEncoder encoder = ExportMapExtension.getEncoder(fileType);
                message.Encoder = encoder;
                NotificationMessage<StateMessage> notification = new NotificationMessage<StateMessage>(message);
                this.eventAggregator.GetEvent<NotificationEvent<StateMessage>>().Publish(notification);
            }
        }

        public void EditLayerProperties()
        {
            throw new NotImplementedException();
        }

        public void SetZoom(ZoomLevel zoomLevel)
        {
            NotificationMessage<ZoomLevel> message = new NotificationMessage<ZoomLevel>(zoomLevel);
            this.eventAggregator.GetEvent<NotificationEvent<ZoomLevel>>().Publish(message);
        }

        public void DuplicateMap()
        {
            Console.WriteLine("Duplicate Map");
        }

        public void FlipMapHorizontally()
        {
            Console.WriteLine("Flip Map Horizontally");
        }

        public void FlipMapVertically()
        {
            Console.WriteLine("Flip Map Vertically");
        }

        public void GuillotineMap()
        {
            Console.WriteLine("Guillotine");
        }

        public void ImportFile()
        {
            Console.WriteLine("Import");
        }

        public void ViewFileProperties()
        {
            Console.WriteLine("Properties");
        }

        public void CloseFile()
        {
            Console.WriteLine("Close");
        }

        public void CloseAllFiles()
        {
            Console.WriteLine("Close All Files");
        }

        public void ExitProgram()
        {
            Console.WriteLine("Exiting Program");
        }

        public void Undo()
        {
            Console.WriteLine("Undo");
        }

        public void Redo()
        {
            Console.WriteLine("Redo");
        }

        public void CutSelection()
        {
            Console.WriteLine("Cut Selection");
        }

        public void CopySelection()
        {
            Console.WriteLine("Copy Selection");
        }

        public void PasteClipboard()
        {
            Console.WriteLine("Paste Clipboard");
        }

        public void SampleView()
        {
            Console.WriteLine("Sample View");
        }

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

        public void FitMapToWindow()
        {
            Console.WriteLine("Fit Map To Window");
        }

        public void NewGroup()
        {
            NotificationMessage<LayerNotification> newLayerGroupMessage = new NotificationMessage<LayerNotification>(LayerNotification.NewLayerGroup, "LayerGroup");
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(newLayerGroupMessage);
        }

        public void DuplicateLayer()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.DuplicateCurrentLayer);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void MergeLayerDown()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.MergeCurrentLayerDown);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void MergeLayerUp()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.MergeCurrentLayerDown);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void MergeVisible()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.MergeVisibleLayers);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void DeleteLayer()
        {
            NotificationMessage<LayerNotification> message = new NotificationMessage<LayerNotification>(LayerNotification.DeleteCurrentLayer);
            this.eventAggregator.GetEvent<NotificationEvent<LayerNotification>>().Publish(message);
        }

        public void LayerToMap()
        {
            Console.WriteLine("Layer To Map ");
        }

        public void AddTileset()
        {
            Console.WriteLine("Add Tileset");
        }

        public void AddImage()
        {
            Console.WriteLine("Add Image");
        }

        public void AddGroup()
        {
            Console.WriteLine("Add Group");
        }

        public void EditItemProperties()
        {
            Console.WriteLine("Edit Item Properties...");
        }

        public void EditItemCollisions()
        {
            Console.WriteLine("Edit Item Collisions");
        }

        public void CollisionsView()
        {
            Console.WriteLine("Collisions View");
        }

        public void ZoomTool()
        {
            Console.WriteLine("Zoom Tool");
        }

        public void DockPresetView()
        {
            Console.WriteLine("Dock Preset");
        }

        public void ShowGrid(bool isShowGrid)
        {
            Console.WriteLine("Show Grid: " + isShowGrid);
        }

        public void ShowRuler(bool isShowRuler)
        {
            Console.WriteLine("Show Ruler: " + isShowRuler);
        }

        public void ShowScrollBar(bool isShowScrollbar)
        {
            Console.WriteLine("Show Scroll: " + isShowScrollbar);
        }

        public void OpenUndoHistoryDock()
        {
            Console.WriteLine("Open Undo History Dock");
        }

        public void HideDocks()
        {
            Console.WriteLine("Hide Dock");
        }

        public void SingleWindow()
        {
            Console.WriteLine("Single Window ");
        }

        public void Help()
        {
            Console.WriteLine("Open Help Window");
        }

        public void About()
        {
            Console.WriteLine("Open About Window");
        }

        #endregion methods
    }
}
