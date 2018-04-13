using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public interface ILayer
    {
        #region fields

        #endregion fields
                
        #region properties

        string LayerName { get; set; }
        bool IsImmutable { get; set; }
        bool IsVisible { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
