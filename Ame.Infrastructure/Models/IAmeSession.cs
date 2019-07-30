using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.DrawingTools;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public interface IAmeSession
    {
        #region fields

        #endregion fields


        #region properties

        ObservableCollection<Project> Projects { get; set; }

        ObservableCollection<Map> Maps { get; set; }

        ReadOnlyBindableProperty<ObservableCollection<ILayer>> CurrentLayers { get; }

        BindableProperty<ObservableCollection<TilesetModel>> CurrentTilesets { get; set; }

        BindableProperty<Project> CurrentProject { get; set; }

        BindableProperty<Map> CurrentMap { get; set; }

        BindableProperty<TilesetModel> CurrentTileset { get; set; }

        BindableProperty<string> Version { get; set; }

        int ProjectCount { get; }

        int MapCount { get; }

        int CurrentLayerCount { get; }

        int CurrentTilesetCount { get; }

        int CurrentMapIndex { get; }

        int CurrentTilesetIndex { get; }

        BindableProperty<IDrawingTool> DrawingTool { get; set; }

        BindableProperty<string> DefaultWorkspaceDirectory { get; set; }

        BindableProperty<string> LastTilesetDirectory { get; set; }

        BindableProperty<string> LastMapDirectory { get; set; }

        #endregion properties


        #region methods

        void SetCurrentMap(Map currentMap);

        void SetCurrentMapAtIndex(int currentIndex);

        #endregion methods
    }
}
