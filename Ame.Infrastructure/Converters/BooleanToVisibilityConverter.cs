using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ame.Infrastructure.Converters
{
    public class BooleanToVisiblityConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        public BooleanToVisiblityConverter()
        {
        }

        #endregion constructor


        #region properties

        public bool Collapse { get; set; }

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            Visibility visibility;
            if (booleanValue)
            {
                visibility = Visibility.Visible;
            }
            else if (Collapse)
            {
                visibility = Visibility.Collapsed;
            }
            else
            {
                visibility = Visibility.Hidden;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Visible;
        }

        #endregion methods
    }
}
