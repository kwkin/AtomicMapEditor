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
using Ame.Infrastructure.Serialization;

namespace Ame.Infrastructure.Models
{
    public class AmeSession : INotifyPropertyChanged
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class AmeSessionJson : JsonAdapter<AmeSession>
        {
            public AmeSessionJson()
            {
            }
                
            public AmeSessionJson(AmeSession session)
            {
                this.Version = Global.Version;
                this.CurrentMap = session.CurrentMapIndex;
                this.OpenedTilesetFiles = new List<string>();
                foreach (Map map in session.MapList)
                {
                    this.OpenedTilesetFiles.Add(map.SourcePath);
                }
                this.OpenedTilesetFiles = new List<string>();
                foreach (TilesetModel tileset in session.CurrentTilesetList)
                {
                    this.OpenedTilesetFiles.Add(tileset.SourcePath);
                }
                this.LastMapDirectory = session.LastMapDirectory;
                this.LastTilesetDirectory = session.LastTilesetDirectory;

            }

            [JsonProperty(PropertyName = "Version")]
            public string Version { get; set; }

            [JsonProperty(PropertyName = "CurrentMap")]
            public int CurrentMap { get; set; }

            [JsonProperty(PropertyName = "OpenedMaps")]
            public IList<string> OpenedMapFiles { get; set; }

            [JsonProperty(PropertyName = "OpenedTilesets")]
            public IList<string> OpenedTilesetFiles { get; set; }

            [JsonProperty(PropertyName = "MapDirectory")]
            public string LastMapDirectory { get; set; }

            [JsonProperty(PropertyName = "TilesetDirectory")]
            public string LastTilesetDirectory { get; set; }

            public AmeSession Generate()
            {
                // TODO load maps, tilesets, and other properties
                AmeSession session = new AmeSession();
                session.LastMapDirectory = this.LastMapDirectory;
                session.lastTilesetDirectory = this.LastTilesetDirectory;
                return session;
            }
        }

        #region fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

        public AmeSession()
        {
            this.MapList = new ObservableCollection<Map>();
            this.CurrentLayerList = new ObservableCollection<ILayer>();
            this.CurrentTilesetList = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();
        }

        public AmeSession(ObservableCollection<Map> MapList)
        {
            this.MapList = MapList;
            this.CurrentLayerList = new ObservableCollection<ILayer>();
            this.CurrentTilesetList = new ObservableCollection<TilesetModel>();
            this.DrawingTool = new StampTool();
        }

        public AmeSession(Map Map)
        {
            this.MapList = new ObservableCollection<Map>();
            this.MapList.Add(Map);
            this.CurrentLayerList = new ObservableCollection<ILayer>();
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
                this.mapList = value;
                NotifyPropertyChanged();
            }
        }

        // TODO reference the current map
        private ObservableCollection<ILayer> currentLayerList;
        public ObservableCollection<ILayer> CurrentLayerList
        {
            get
            {
                return this.currentLayerList;
            }
            set
            {
                this.currentLayerList = value;
                NotifyPropertyChanged();
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
                this.currentTilesetList = value;
                NotifyPropertyChanged();
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
                this.currentMap = value;
                this.CurrentLayerList = this.currentMap.LayerList;
                this.CurrentLayer = this.currentMap.CurrentLayer;
                this.CurrentTilesetList = this.currentMap.TilesetList;
                NotifyPropertyChanged();
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
                this.currentLayer = value;
                int currentLayerIndex = this.CurrentMap.LayerList.IndexOf(this.CurrentLayer);
                if (currentLayerIndex != -1)
                {
                    this.CurrentMap.SelectedLayerIndex = currentLayerIndex;
                }
                NotifyPropertyChanged();
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
                this.currentTileset = value;
                NotifyPropertyChanged();
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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion methods
    }
}
