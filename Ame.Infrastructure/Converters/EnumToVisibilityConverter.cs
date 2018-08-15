using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ame.Infrastructure.Converters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        public EnumToVisibilityConverter()
        {
        }

        #endregion constructor


        #region properties

        public Visibility EqualVisibility { get; set; }
        public Visibility NotEqualVisibility { get; set; }

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object trueValue, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = this.NotEqualVisibility;
            if (value != null && value.GetType().IsEnum)
            {
                if (Enum.Equals(value, trueValue))
                {
                    visibility = this.EqualVisibility;
                }
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object trueValue, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion methods
    }
}
