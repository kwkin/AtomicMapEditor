using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Attributes
{
    public class SerializableNameAttribute : Attribute
    {
        #region constructor

        internal SerializableNameAttribute(string name)
        {
            this.Name = name;
        }

        #endregion constructor


        #region properties
        public string Name { get; private set; }

        #endregion properties
    }
}
