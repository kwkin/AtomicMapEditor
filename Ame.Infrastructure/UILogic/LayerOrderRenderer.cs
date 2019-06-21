using Ame.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.UILogic
{

    public class LayerOrderRenderer
    {
        #region fields

        private AmeSession session;

        #endregion fields


        #region constructor

        public LayerOrderRenderer(AmeSession session)
        {
            this.session = session ?? throw new ArgumentNullException("session");
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public int getAffectedIndex(ILayer layer)
        {
            int listIndex = this.session.CurrentLayerList.IndexOf(layer);
            int visibleCount = 0;
            for(int index = 0; index < listIndex; ++index)
            {
                ILayer currentLayer = this.session.CurrentLayerList[index];
                if (currentLayer.IsVisible.Value)
                {
                    visibleCount++;
                }
            }
            return visibleCount;
        }

        #endregion methods
    }
}
