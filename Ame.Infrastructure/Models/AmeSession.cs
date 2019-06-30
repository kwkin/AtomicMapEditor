using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Core;
using System.IO;
using Newtonsoft.Json;
using Prism.Mvvm;
using Ame.Infrastructure.Models.Serializer.Json;

namespace Ame.Infrastructure.Models
{
    // TODO change to a bindable property solution
    public class AmeSession : BindableBase
    {
        #region fields
        
        #endregion fields


        #region constructor

        public AmeSession()
        {
            this.MapList = new ObservableCollection<Map>();
            this.CurrentTilesetList = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();
        }

        public AmeSession(ObservableCollection<Map> MapList)
        {
            this.MapList = MapList;
            this.CurrentTilesetList = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();
        }

        public AmeSession(Map Map)
        {
            this.MapList = new ObservableCollection<Map>();
            this.MapList.Add(Map);
            this.CurrentTilesetList = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();
        }

        #endregion constructor


        #region properties

        private ObservableCollection<Map> mapList;
        public ObservableCollection<Map> MapList
        {
            get
            {
                return this.mapList;
            }
            set
            {
                this.SetProperty(ref this.mapList, value);
            }
        }

        public ObservableCollection<ILayer> CurrentLayerList
        {
            get
            {
                // TODO this nulls all the time. Fix it.
                return this.CurrentMap.Layers;
            }
        }

        private ObservableCollection<TilesetModel> currentTilesetList;
        public ObservableCollection<TilesetModel> CurrentTilesetList
        {
            get
            {
                return this.currentTilesetList;
            }
            set
            {
                this.SetProperty(ref this.currentTilesetList, value);
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
                this.currentTilesetList = value.TilesetList;
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

        public int MapCount
        {
            get
            {
                return MapList.Count;
            }
        }

        public int CurrentLayerCount
        {
            get
            {
                return this.CurrentLayerList.Count;
            }
        }

        public int CurrentTilesetCount
        {
            get
            {
                return this.CurrentTilesetList.Count;
            }
        }

        public int CurrentMapIndex
        {
            get
            {
                return this.MapList.IndexOf(this.CurrentMap);
            }
        }

        public int CurrentLayerIndex
        {
            get
            {
                return this.CurrentLayerList.IndexOf(this.CurrentLayer);
            }
        }

        public int CurrentTilesetIndex
        {
            get
            {
                return this.CurrentTilesetList.IndexOf(this.CurrentTileset);
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
            if (!this.MapList.Contains(currentMap))
            {
                throw new ArgumentOutOfRangeException(currentMap.Name + " not found in current map list.");
            }
            this.CurrentMap = currentMap;
        }

        public void SetCurrentMapAtIndex(int currentIndex)
        {
            this.CurrentMap = this.MapList[currentIndex];
        }

        // TODO add an interface for this
        public void SerializeFile(string file)
        {
            AmeSessionJson json = new AmeSessionJson(this);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter stream = new StreamWriter(file))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                serializer.Serialize(writer, json);
            }
        }
        
        #endregion methods
    }
}
