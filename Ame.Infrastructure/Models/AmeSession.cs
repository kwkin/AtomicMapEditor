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
    // TODO implement ISession interface
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
            this.CurrentTilesets = new ObservableCollection<TilesetModel>();
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

        // TODO change to a bindable property
        public ObservableCollection<ILayer> CurrentLayers
        {
            get
            {
                ObservableCollection<ILayer> layers = null;
                if (this.CurrentMap != null)
                {
                    layers = this.CurrentMap.Value.Layers;
                }
                return layers;
            }
        }

        // TODO change to a bindable property
        public ObservableCollection<TilesetModel> CurrentTilesets { get; set; }

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
                return this.CurrentLayers.Count;
            }
        }

        public int CurrentTilesetCount
        {
            get
            {
                return this.CurrentTilesets.Count;
            }
        }

        public int CurrentMapIndex
        {
            get
            {
                return this.Maps.IndexOf(this.CurrentMap.Value);
            }
        }

        public int CurrentLayerIndex
        {
            get
            {
                return this.CurrentLayers.IndexOf(this.CurrentLayer.Value);
            }
        }

        public int CurrentTilesetIndex
        {
            get
            {
                return this.CurrentTilesets.IndexOf(this.CurrentTileset.Value);
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
            this.CurrentLayer.Value = this.CurrentMap.Value.CurrentLayer;
            this.CurrentTilesets = this.CurrentMap.Value.Tilesets;
        }

        private void CurrentLayerChanged(object sender, PropertyChangedEventArgs e)
        {
            int currentLayerIndex = this.CurrentMap.Value.Layers.IndexOf(this.CurrentLayer.Value);
            if (currentLayerIndex != -1)
            {
                this.CurrentMap.Value.SelectedLayerIndex.Value = currentLayerIndex;
            }
        }

        #endregion methods
    }
}
