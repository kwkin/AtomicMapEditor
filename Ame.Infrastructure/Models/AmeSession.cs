﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models
{
    public class AmeSession : INotifyPropertyChanged
    {
        #region fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

        public AmeSession()
        {
            this.MapList = new List<Map>();
        }

        public AmeSession(IList<Map> MapList)
        {
            this.MapList = MapList;
        }

        public AmeSession(Map Map)
        {
            this.MapList = new List<Map>();
            this.MapList.Add(Map);
        }

        #endregion constructor


        #region properties
        
        public IList<Map> MapList { get; set; }
        public int MapListIndex
        {
            get
            {
                return this.MapList.IndexOf(this.CurrentMap);
            }
        }
        public int MapCount { get { return MapList.Count; } }

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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion methods
    }
}