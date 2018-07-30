using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.DrawingTools
{
    public interface ISelectionTool
    {
        #region properties

        #endregion properties


        #region methods

        void DeleteSelected();
        void CopySelected();

        #endregion methods
    }
}
