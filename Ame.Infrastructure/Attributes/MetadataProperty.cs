using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Attributes
{
    public enum MetadataType
    {
        Property, Statistic, Custom
    }

    public class MetadataProperty
    {
        #region fields

        #endregion fields


        #region constructor

        public MetadataProperty(string key, object value, MetadataType type)
        {
            this.Key = key;
            this.Value = value;
            this.Type = type;
            if (type == MetadataType.Custom)
            {
                this.IsReadOnly = false;
            }
            else
            {
                this.IsReadOnly = true;
            }
        }

        #endregion constructor


        #region properties

        public string Key { get; set; }
        public object Value { get; set; }
        public MetadataType Type { get; set; }
        public bool IsReadOnly { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
