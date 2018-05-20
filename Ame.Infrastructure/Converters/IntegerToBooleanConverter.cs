using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ame.Infrastructure.Converters
{
    public class IntegerToBooleanConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        public IntegerToBooleanConverter()
        {
            this.ComparisonValue = 0;
            this.FallbackValue = -1;
        }

        #endregion constructor


        #region properties

        public int ComparisonValue { get; set; }
        public int FallbackValue { get; set; }
        public bool IsInversed { get; set; }

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if (value.GetType() != typeof(int))
            {
                return false;
            }
            int integerValue = (int)value;
            return (this.ComparisonValue == integerValue) ^ IsInversed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if (targetType != typeof(bool))
            {
                return false;
            }
            bool booleanValue = ((bool)value) ^ IsInversed;
            return (booleanValue) ? this.ComparisonValue : this.FallbackValue;
        }

        #endregion methods
    }
}
