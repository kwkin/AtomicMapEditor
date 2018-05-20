using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Attributes;

namespace Ame.Infrastructure.Attributes
{
    public static class MetadataPropertyUtils
    {
        #region methods

        public static ObservableCollection<MetadataProperty> GetPropertyList(object item)
        {
            ObservableCollection<MetadataProperty> mapProperties = new ObservableCollection<MetadataProperty>();
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

        #endregion methods
    }
}
