using Ame.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Attributes
{
    public class SerializableNameUtils
    {
        #region methods

        public static string GetName(DragDataType dataType)
        {
            PropertyInfo[] propertyInfoList = dataType.GetType().GetProperties();
            string metadataName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                SerializableNameAttribute attribute = propertyInfo.GetCustomAttribute<SerializableNameAttribute>();
                if (attribute != null)
                {
                    metadataName = propertyInfo.Name;
                }
            }
            return metadataName;
        }

        #endregion methods
    }
}
