using System;
using System.Windows;
using System.Windows.Data;

namespace Ame.Infrastructure.Converters
{
    public class BooleanToVisiblityConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public BooleanToVisiblityConverter()
        {
        }

        #endregion constructor & destructer


        #region properties

        public bool Collapse { get; set; }

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            if (booleanValue)
            {
                return Visibility.Visible;
            }
            else if (Collapse)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion methods
    }
}
