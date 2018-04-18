using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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

        public static IList GetPropertyList(object item)
        {
            IList mapProperties = new List<MetadataProperty>();
            PropertyInfo[] propertyInfoList = item.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                MetadataPropertyAttribute attribute = propertyInfo.GetCustomAttribute<MetadataPropertyAttribute>();
                if (attribute != null)
                {
                    String metadataName = propertyInfo.Name;
                    if (attribute.Name != string.Empty)
                    {
                        metadataName = attribute.Name;
                    }
                    mapProperties.Add(new MetadataProperty(metadataName, propertyInfo.GetValue(item), attribute.Type));
                }
            }
            return mapProperties;
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
            this.Name = "";
        }

        internal MetadataPropertyAttribute(MetadataType type, String name)
        {
            this.Type = type;
            this.Name = name;
        }

        #endregion constructor


        #region properties

        public String Name { get; private set; }
        public MetadataType Type { get; private set; }

        #endregion properties
    }
}
