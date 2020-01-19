using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public interface ILayerParent
    {
        #region fields

        #endregion fields


        #region properties

        ObservableCollection<ILayer> Layers { get; set; }

        #endregion properties


        #region methods

        void AddLayer(ILayer layer);

        #endregion methods
    }
}
