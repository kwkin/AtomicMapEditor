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
            this.LayerGroupName = layerGroupName;
        }

        public LayerGroup(string layerGroupName, IList<ILayer> layers)
        {
            this.LayerGroupName = layerGroupName;
            this.Layers = layers;
        }

        #endregion constructor & destructer


        #region properties

        public string LayerGroupName { get; set; }
        public bool IsImmutable { get; set; }
        public bool IsVisible { get; set; }
        public IList<ILayer> Layers { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
