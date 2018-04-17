using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;

namespace Ame.Infrastructure.BaseTypes
{
    public enum PropertyType
    {
        Property, Statistic, Custom
    }

    public class PropertyStat
    {
        #region fields

        #endregion fields


        #region constructor

        public PropertyStat(object key, object value, PropertyType type)
        {
            this.Key = key;
            this.Value = value;
            this.Type = type;
            if (type == PropertyType.Custom)
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

        public object Key { get; set; }
        public object Value { get; set; }
        public PropertyType Type { get; set; }
        public bool IsReadOnly { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }

    public static class PropertyStatUtils
    {
        #region fields

        #endregion fields


        #region constructor

        public static ListCollectionView GetPropertyList(object item)
        {
            IList mapProperties = new List<PropertyStat>();
            PropertyInfo[] propertyInfoList = item.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                PropertyStatAttribute attribute = propertyInfo.GetCustomAttribute<PropertyStatAttribute>();
                if (attribute != null)
                {
                    mapProperties.Add(new PropertyStat(propertyInfo.Name, propertyInfo.GetValue(item), attribute.Type));
                }
            }
            return new ListCollectionView(mapProperties);
        }

        #endregion constructor


        #region properties
        
        #endregion properties


        #region methods

        #endregion methods
    }

    internal class PropertyStatAttribute : Attribute
    {
        #region constructor

        internal PropertyStatAttribute(PropertyType type)
        {
            this.Type = type;
        }

        #endregion constructor


        #region properties

        public PropertyType Type { get; private set; }

        #endregion properties
    }
}
