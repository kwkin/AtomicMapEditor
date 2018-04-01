using System;
using System.ComponentModel;
using System.Reflection;

namespace Ame.Infrastructure.Converters
{
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public EnumDescriptionTypeConverter(Type type) : base(type)
        {
        }

        #endregion constructor & destructer


        #region properties

        #endregion properties


        #region methods

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }

                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion methods
    }
}
