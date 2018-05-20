using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Attributes
{
    public class MetadataPropertyAttribute : Attribute
    {
        #region constructor

        internal MetadataPropertyAttribute(MetadataType type)
        {
            this.Type = type;
            this.Name = "";
        }

        internal MetadataPropertyAttribute(MetadataType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        #endregion constructor


        #region properties

        public string Name { get; private set; }
        public MetadataType Type { get; private set; }

        #endregion properties
    }
}
