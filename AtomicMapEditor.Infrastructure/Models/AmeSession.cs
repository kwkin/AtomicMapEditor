using System;
using System.Collections.Generic;
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
                NotifyPropertyChanged();
            }
        }
        
        public ILayer CurrentLayer
        {
            get
            {
                if (this.currentMap == null)
                {
                    return null;
                }
                return this.currentMap.CurrentLayer;
            }
        }

        #endregion properties


        #region methods
        
        public void ChangeMap(Map currentMap)
        {
            if (!this.MapList.Contains(currentMap))
            {
                this.MapList.Add(currentMap);
                this.CurrentMap = currentMap;
            }
            else
            {
                this.CurrentMap = currentMap;
            }
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
