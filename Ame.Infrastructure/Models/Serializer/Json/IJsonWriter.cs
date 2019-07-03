using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Json.Data
{
    public interface IJsonWriter<T>
    {
        #region properties

        #endregion properties


        #region methods

        void Write(T item, string file);

        void Write(T item, Stream file);

        #endregion methods
    }
}
