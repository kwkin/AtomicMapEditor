using System.Collections.Generic;

namespace Ame.Infrastructure.Models
{
    public class LayerGroup : ILayer
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public LayerGroup(string layerGroupName)
        {
            this.LayerName = layerGroupName;
        }

        public LayerGroup(string layerGroupName, IList<ILayer> layers)
        {
            this.LayerName = layerGroupName;
            this.Layers = layers;
        }

        #endregion constructor & destructer


        #region properties

        public string LayerName { get; set; }
        public bool IsImmutable { get; set; }
        public bool IsVisible { get; set; }
        public IList<ILayer> Layers { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
