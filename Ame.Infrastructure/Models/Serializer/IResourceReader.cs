using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer
{
    public interface IResourceReader<T>
    {
        #region properties

        #endregion properties


        #region methods

        T Read(string file);

        #endregion methods
    }
}
