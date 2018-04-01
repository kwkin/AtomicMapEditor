using System;
using System.Windows.Data;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Converters
{
    public class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DockToolViewModelTemplate)
            {
                return value;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DockToolViewModelTemplate)
            {
                return value;
            }
            return Binding.DoNothing;
        }
    }
}
