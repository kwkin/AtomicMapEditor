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
    public class AmeSession
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

            this.CurrentMap.PropertyChanged += CurrentMapChanged;
            this.CurrentLayer.PropertyChanged += CurrentLayerChanged;
        }

        public AmeSession(Map Map)
        {
            this.Projects = new ObservableCollection<Project>();
            this.Maps = new ObservableCollection<Map>();
            this.Maps.Add(Map);
            this.CurrentTilesets = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();

            this.CurrentMap.PropertyChanged += CurrentMapChanged;
            this.CurrentLayer.PropertyChanged += CurrentLayerChanged;
        }

        #endregion constructor


        #region properties

        public ObservableCollection<Project> Projects { get; set; }

        public ObservableCollection<Map> Maps { get; set; }

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

        public ObservableCollection<TilesetModel> CurrentTilesets { get; set; }

        public BindableProperty<Project> CurrentProject { get; set; } = BindableProperty.Prepare<Project>();

        public BindableProperty<Map> CurrentMap { get; set; } = BindableProperty.Prepare<Map>();

        public BindableProperty<ILayer> CurrentLayer { get; set; } = BindableProperty.Prepare<ILayer>();

        public BindableProperty<TilesetModel> CurrentTileset { get; set; } = BindableProperty.Prepare<TilesetModel>();

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
