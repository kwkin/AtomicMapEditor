using Ame.Infrastructure.BaseTypes;
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

        private bool isBindableProperty = false;

        #region constructor

        public MetadataProperty(string key, object value, MetadataType type)
        {
            this.Key = key;
            this.Value = value;
            if (typeof(PropertyValue).IsInstanceOfType(value))
            {
                this.isBindableProperty = true;
            }

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

        private object value;
        public object Value
        {
            get
            {
                object toReturn = this.value;
                if (this.isBindableProperty)
                {
                    toReturn = (this.value as PropertyValue).GetValue();
                }
                return toReturn;
            }
            set
            {
                this.value = value;
            }
        }
        public MetadataType Type { get; set; }
        public bool IsReadOnly { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
