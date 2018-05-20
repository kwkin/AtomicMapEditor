using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Attributes
{
    public class FileExtensionAttribute : Attribute
    {
        #region constructor

        internal FileExtensionAttribute(string name, params string[] extensions)
        {
            this.Name = name;
            this.Extensions = extensions;
        }

        #endregion constructor


        #region properties

        public string Name { get; private set; }
        public string[] Extensions { get; private set; }

        #endregion properties
    }
}
