using System;
using System.Reflection;

namespace Ame.Infrastructure.BaseTypes
{
    public class DockContentIdAttribute : Attribute
    {
        #region constructor

        public DockContentIdAttribute(string id)
        {
            this.Id = id;
        }

        #endregion constructor


        #region properties

        public string Id { get; private set; }

        #endregion properties
    }
}
