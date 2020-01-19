using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    // TODO remove reference to current layer
    public class AmeSession : IAmeSession
    {
        #region fields

        #endregion fields


        #region constructor

        public AmeSession(IConstants constants)
            : this(new List<Map>(), new List<Project>(), constants.DefaultWorkspaceDirectory, constants.DefaultWorkspaceDirectory, constants.DefaultWorkspaceDirectory, constants.Version)
        {
        }

        public AmeSession(string workspaceDirectory, string tilesetDirectory, string mapDirectory, string version)
            : this(new List<Map>(), new List<Project>(), workspaceDirectory, tilesetDirectory, mapDirectory, version)
        {
        }

        public AmeSession(IList<Map> openedMaps, IList<Project> openedProjects, IConstants constants)
            : this(openedMaps, openedProjects, constants.DefaultWorkspaceDirectory, constants.DefaultWorkspaceDirectory, constants.DefaultWorkspaceDirectory, constants.Version)
        {
        }

        public AmeSession(IList<Map> openedMaps, IList<Project> openedProjects, string workspaceDirectory, string tilesetDirectory, string mapDirectory, string version)
        {
            this.Projects = new ObservableCollection<Project>(openedProjects);
            this.Maps = new ObservableCollection<Map>(openedMaps);
            this.CurrentTilesets.Value = new ObservableCollection<TilesetModel>();
            this.DrawingTool.Value = new StampTool();

            this.DefaultWorkspaceDirectory.Value = workspaceDirectory;
            this.LastTilesetDirectory.Value = tilesetDirectory;
            this.LastMapDirectory.Value = mapDirectory;
            this.Version.Value = version;

            this.CurrentMap.PropertyChanged += CurrentMapChanged;
            this.CurrentLayer.PropertyChanged += CurrentLayerChanged;
        }

        #endregion constructor


        #region properties

        public ObservableCollection<Project> Projects { get; set; }

        public ObservableCollection<Map> Maps { get; set; }

        private BindableProperty<ObservableCollection<ILayer>> currentLayers = BindableProperty.Prepare<ObservableCollection<ILayer>>();
        private ReadOnlyBindableProperty<ObservableCollection<ILayer>> currentLayersReadOnly;
        public ReadOnlyBindableProperty<ObservableCollection<ILayer>> CurrentLayers
        {
            get
            {
                this.currentLayersReadOnly = this.currentLayersReadOnly ?? this.currentLayers.ReadOnlyProperty();
                return this.currentLayersReadOnly;
            }
        }

        public BindableProperty<ObservableCollection<TilesetModel>> CurrentTilesets { get; set; } = BindableProperty.Prepare<ObservableCollection<TilesetModel>>();

        public BindableProperty<Project> CurrentProject { get; set; } = BindableProperty.Prepare<Project>();

        public BindableProperty<Map> CurrentMap { get; set; } = BindableProperty.Prepare<Map>();

        public BindableProperty<ILayer> CurrentLayer { get; set; } = BindableProperty.Prepare<ILayer>();

        public BindableProperty<TilesetModel> CurrentTileset { get; set; } = BindableProperty.Prepare<TilesetModel>();

        public BindableProperty<string> Version { get; set; } = BindableProperty.Prepare<string>("");

        public int ProjectCount
        {
            get
            {
                return this.Projects.Count;
            }
        }

        public int MapCount
        {
            get
            {
                return this.Maps.Count;
            }
        }

        public int CurrentLayerCount
        {
            get
            {
                return this.CurrentLayers.Value.Count;
            }
        }

        public int CurrentTilesetCount
        {
            get
            {
                return this.CurrentTilesets.Value.Count;
            }
        }

        public int CurrentMapIndex
        {
            get
            {
                return this.Maps.IndexOf(this.CurrentMap.Value);
            }
        }

        public int CurrentTilesetIndex
        {
            get
            {
                return this.CurrentTilesets.Value.IndexOf(this.CurrentTileset.Value);
            }
        }

        public BindableProperty<IDrawingTool> DrawingTool { get; set; } = BindableProperty.Prepare<IDrawingTool>();

        public BindableProperty<string> DefaultWorkspaceDirectory { get; set; } = BindableProperty.Prepare<string>();

        public BindableProperty<string> LastTilesetDirectory { get; set; } = BindableProperty.Prepare<string>();

        public BindableProperty<string> LastMapDirectory { get; set; } = BindableProperty.Prepare<string>();

        #endregion properties


        #region methods

        public void SetCurrentMap(Map currentMap)
        {
            if (!this.Maps.Contains(currentMap))
            {
                throw new ArgumentOutOfRangeException(currentMap.Name + " not found in current map list.");
            }
            this.CurrentMap.Value = currentMap;
        }

        public void SetCurrentMapAtIndex(int currentIndex)
        {
            this.CurrentMap.Value = this.Maps[currentIndex];
        }

        private void CurrentMapChanged(object sender, PropertyChangedEventArgs e)
        {
            this.currentLayers.Value = this.CurrentMap.Value.Layers;
            this.CurrentTilesets.Value = this.CurrentMap.Value.Tilesets;
            this.CurrentLayer.Value = this.CurrentMap.Value.CurrentLayer.Value;
        }

        private void CurrentLayerChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CurrentMap.Value.CurrentLayer.Value = this.CurrentLayer.Value;
        }

        #endregion methods
    }
}
