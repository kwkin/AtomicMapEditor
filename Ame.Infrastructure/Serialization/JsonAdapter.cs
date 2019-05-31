using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Serialization
{
    public interface JsonAdapter<T>
    {
        T Generate();
    }
}
