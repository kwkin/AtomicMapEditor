using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.UILogic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Handlers
{
    public interface IActionHandler
    {
        #region fields

        #endregion fields


        #region properties

        #endregion properties


        #region methods


        void OpenWindow(IWindowInteraction interaction);

        void OpenDock(OpenDockMessage dockMessage);

        void OpenProject();

        void SaveMap(Map map);

        void SaveCurrentMap();

        void ExportFile();

        void ImportFile();

        void ViewFileProperties();

        void CloseFile();

        void CloseAllFiles();

        void ExitProgram();

        void Undo();

        void Redo();

        void CutSelection();

        void CopySelection();

        void PasteClipboard();

        void SampleView();

        void ZoomIn();

        void ZoomOut();

        void FitMapToWindow();

        void DuplicateMap();

        void FlipMapHorizontally();

        void FlipMapVertically();

        void GuillotineMap();

        void NewLayerGroup();

        void DuplicateLayer();

        void MergeLayerDown();

        void MoveLayerDown();

        void MoveLayerUp();

        void MergeLayerUp();

        void MergeVisible();

        void DeleteLayer();

        void LayerToMap();

        void AddTileset();

        void AddImage();

        void AddGroup();

        void LayerToMapSize();

        void EditItemProperties();

        void EditCollisions();

        void CollisionsView();

        void ZoomTool();

        void SetZoom(ZoomLevel zoomLevel);

        void DockPresetView();

        void ShowGrid(bool isShowGrid);

        void ShowRuler(bool isShowRuler);

        void ShowScrollBar(bool isShowScrollbar);

        void OpenUndoHistoryDock();

        void HideDocks();

        void SingleWindow();

        void Help();

        void About();

        #endregion methods
    }
}
