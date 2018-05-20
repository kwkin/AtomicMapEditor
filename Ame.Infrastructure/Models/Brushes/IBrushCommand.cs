using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Brushes
{
    public interface IBrushCommand
    {
        #region properties

        #endregion


        #region methods

        void Execute();
        void UnExecute();

        #endregion
    }
}
