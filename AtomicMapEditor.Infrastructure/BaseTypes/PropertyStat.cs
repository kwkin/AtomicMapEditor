using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;

namespace Ame.Infrastructure.BaseTypes
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

        public MetadataProperty(object key, object value, MetadataType type)
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

        public object Key { get; set; }
        public object Value { get; set; }
        public MetadataType Type { get; set; }
        public bool IsReadOnly { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }

    public static class MetadataPropertyUtils
    {
        #region fields

        #endregion fields


        #region constructor

        public static ListCollectionView GetPropertyList(object item)
        {
            IList mapProperties = new List<MetadataProperty>();
            PropertyInfo[] propertyInfoList = item.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                MetadataPropertyAttribute attribute = propertyInfo.GetCustomAttribute<MetadataPropertyAttribute>();
                if (attribute != null)
                {
                    mapProperties.Add(new MetadataProperty(propertyInfo.Name, propertyInfo.GetValue(item), attribute.Type));
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

    internal class MetadataPropertyAttribute : Attribute
    {
        #region constructor

        internal MetadataPropertyAttribute(MetadataType type)
        {
            this.Type = type;
        }

        #endregion constructor


        #region properties

        public MetadataType Type { get; private set; }

        #endregion properties
    }
}
