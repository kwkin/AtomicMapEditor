using Ame.Infrastructure.Core;
using Ame.Infrastructure.DrawingTools;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    // TODO change to a bindable property solution
    public class AmeSession : BindableBase
    {
        #region fields

        #endregion fields


        #region constructor

        public AmeSession()
            : this(new ObservableCollection<Map>())
        {
        }

        public AmeSession(ObservableCollection<Map> maps)
        {
            this.Projects = new ObservableCollection<Project>();
            this.Maps = maps;
            this.CurrentTilesets = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();
        }

        public AmeSession(Map Map)
        {
            this.Projects = new ObservableCollection<Project>();
            this.Maps = new ObservableCollection<Map>();
            this.Maps.Add(Map);
            this.CurrentTilesets = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();
        }

        #endregion constructor


        #region properties

        private ObservableCollection<Project> projects;
        public ObservableCollection<Project> Projects
        {
            get
            {
                return this.projects;
            }
            set
            {
                this.SetProperty(ref this.projects, value);
            }
        }

        private ObservableCollection<Map> maps;
        public ObservableCollection<Map> Maps
        {
            get
            {
                return this.maps;
            }
            set
            {
                this.SetProperty(ref this.maps, value);
            }
        }

        public ObservableCollection<ILayer> CurrentLayers
        {
            get
            {
                return this.CurrentMap.Layers ?? null;
            }
        }

        private ObservableCollection<TilesetModel> currentTilesets;
        public ObservableCollection<TilesetModel> CurrentTilesets
        {
            get
            {
                return this.currentTilesets;
            }
            set
            {
                this.SetProperty(ref this.currentTilesets, value);
            }
        }

        private Project currentProject;
        public Project CurrentProject
        {
            get
            {
                return this.currentProject;
            }
            set
            {
                this.SetProperty(ref this.currentProject, value);
            }
        }

        private Map currentMap;
        public Map CurrentMap
        {
            get
            {
                return this.currentMap;
            }
            set
            {
                this.currentLayer = value.CurrentLayer;
                this.currentTilesets = value.Tilesets;
                this.SetProperty(ref this.currentMap, value);
            }
        }

        private ILayer currentLayer;
        public ILayer CurrentLayer
        {
            get
            {
                return this.currentLayer;
            }
            set
            {
                int currentLayerIndex = this.CurrentMap.Layers.IndexOf(value);
                if (currentLayerIndex != -1)
                {
                    this.CurrentMap.SelectedLayerIndex.Value = currentLayerIndex;
                }
                this.SetProperty(ref this.currentLayer, value);
            }
        }

        private TilesetModel currentTileset;
        public TilesetModel CurrentTileset
        {
            get
            {
                return this.currentTileset;
            }
            set
            {
                this.SetProperty(ref this.currentTileset, value);
            }
        }

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
                return this.Maps.IndexOf(this.CurrentMap);
            }
        }

        public int CurrentLayerIndex
        {
            get
            {
                return this.CurrentLayers.IndexOf(this.CurrentLayer);
            }
        }

        public int CurrentTilesetIndex
        {
            get
            {
                return this.CurrentTilesets.IndexOf(this.CurrentTileset);
            }
        }

        private IDrawingTool drawingTool;
        public IDrawingTool DrawingTool
        {
            get
            {
                return this.drawingTool;
            }
            set
            {
                this.drawingTool = value;
            }
        }

        private string lastTilesetDirectory;
        public string LastTilesetDirectory
        {
            get
            {
                this.lastTilesetDirectory = this.lastTilesetDirectory ?? Global.DefaultFileDirectory;
                return this.lastTilesetDirectory;
            }
            set
            {
                this.lastTilesetDirectory = value;
            }
        }

        private string lastMapDirectory;
        public string LastMapDirectory
        {
            get
            {
                this.lastMapDirectory = this.lastMapDirectory ?? Global.DefaultFileDirectory;
                return this.lastMapDirectory;
            }
            set
            {
                this.lastMapDirectory = value;
            }
        }
        #endregion properties


        #region methods

        public void SetCurrentMap(Map currentMap)
        {
            if (!this.Maps.Contains(currentMap))
            {
                throw new ArgumentOutOfRangeException(currentMap.Name + " not found in current map list.");
            }
            this.CurrentMap = currentMap;
        }

        public void SetCurrentMapAtIndex(int currentIndex)
        {
            this.CurrentMap = this.Maps[currentIndex];
        }

        #endregion methods
    }
}
