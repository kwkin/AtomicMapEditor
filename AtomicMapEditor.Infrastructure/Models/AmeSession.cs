using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models
{
    public class AmeSession
    {
        #region fields

        #endregion fields


        #region constructor

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
        public int MapListIndex { get; set; }
        public int MapCount { get { return MapList.Count; } }

        public Map CurrentMap
        {
            get
            {
                return this.MapList[this.MapListIndex];
            }
            private set
            {
                int selectedMapIndex = this.MapList.IndexOf(value);
                if (selectedMapIndex == -1)
                {
                    this.MapList.Add(value);
                    this.MapListIndex = this.MapList.Count - 1;
                }
                else
                {
                    this.MapListIndex = selectedMapIndex;
                }
            }
        }

        #endregion properties


        #region methods

        public void ChangeCurrentMap(Map currentMap)
        {
            this.CurrentMap = currentMap;
        }

        #endregion methods
    }
}
