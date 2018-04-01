using System;
using System.Windows.Data;
using AtomicMapEditor.Infrastructure.BaseTypes;

namespace AtomicMapEditor.Infrastructure.Converters
{
    public class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DockViewModelTemplate)
            {
                return value;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DockViewModelTemplate)
            {
                return value;
            }
            return Binding.DoNothing;
        }
    }
}
