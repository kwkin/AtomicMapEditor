using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Handlers
{
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

        public void SaveCurrentMap()
        {
            Map currentMap = this.session.CurrentMap.Value;
            SaveMap(currentMap);
        }

        public void SaveMap(Map map)
        {
            if (map.SourcePath.Value == null)
            {
                map.WriteFile(map.SourcePath.Value);
                this.session.LastMapDirectory.Value = Directory.GetParent(map.SourcePath.Value).FullName;
            }
            else
            {
                map.UpdateFile();
            }
        }

        public void ExportFile()
        {
            Console.WriteLine("Export file");
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
