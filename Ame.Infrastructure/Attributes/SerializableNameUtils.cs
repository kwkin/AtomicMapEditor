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
            var memberInfo = dataType.GetType().GetMember(dataType.ToString());
            var enumValueMemberInfo = memberInfo.FirstOrDefault(m => m.DeclaringType == dataType.GetType());
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SerializableNameAttribute), false);
            string metadataName = ((SerializableNameAttribute)valueAttributes[0]).Name;
            return metadataName;
        }

        #endregion methods
    }
}
