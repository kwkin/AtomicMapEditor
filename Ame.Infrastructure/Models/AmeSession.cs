using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.DrawingTools;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using Ame.Infrastructure.Files;
using Ame.Infrastructure.Core;
using System.IO;

namespace Ame.Infrastructure.Models
{
    // TODO change xml to a more automated solution
    [XmlRoot("Session")]
    public class AmeSession : INotifyPropertyChanged, IXmlSerializable
    {
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
                this.CurrentLayerList = this.currentMap.LayerList.Layers;
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
                if (this.lastTilesetDirectory == null)
                {
                    string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    this.lastTilesetDirectory = documentPath;
                }
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
                if (this.lastMapDirectory == null)
                {
                    string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    this.lastMapDirectory = documentPath;
                }
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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            while(reader.Read())
            {
                AmeXMLTags tag = XMLTagMethods.GetTag(reader.Name);
                if (reader.IsStartElement() && !reader.IsEmptyElement)
                {
                    if (reader.Read())
                    {
                        string value = reader.Value;
                        switch (tag)
                        {
                            case AmeXMLTags.Version:
                                break;
                            case AmeXMLTags.Maps:
                                break;
                            case AmeXMLTags.Tilesets:
                                break;
                            case AmeXMLTags.LastMapDirectory:
                                this.LastMapDirectory = value;
                                break;
                            case AmeXMLTags.LastTilesetDirectory:
                                this.LastTilesetDirectory = value;
                                break;
                        }
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XMLTagMethods.WriteElement(writer, AmeXMLTags.Version, Global.version);
            IList<string> mapNames = new List<string>();
            foreach (Map map in this.mapList)
            {
                mapNames.Add(map.File);
            }
            XMLTagMethods.WriteElements<Map>(writer, AmeXMLTags.Maps, AmeXMLTags.Source, this.MapList);
            XMLTagMethods.WriteElements<TilesetModel>(writer, AmeXMLTags.Tilesets, AmeXMLTags.Source, this.CurrentTilesetList);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.LastMapDirectory, this.LastMapDirectory);
            XMLTagMethods.WriteElement(writer, AmeXMLTags.LastTilesetDirectory, this.LastTilesetDirectory);
        }

        #endregion methods
    }
}
