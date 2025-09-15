using System.Globalization;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;

namespace ArchiWindRevitAddIn.Views.Converters
{
    internal class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? parameter : Binding.DoNothing;
        }
    }
}
