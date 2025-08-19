using System.Globalization;
using System.Windows.Data;

namespace ArchiWindRevitAddIn.Views.Converters
{
    public class CoordinateValidationException : Exception
    {
        public CoordinateValidationException(string message) : base(message) { }
        public CoordinateValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CoordinateRangeException : CoordinateValidationException
    {
        public double Value { get; }
        public double MinValue { get; }
        public double MaxValue { get; }

        public CoordinateRangeException(double value, double minValue, double maxValue, string coordinateType)
            : base($"{coordinateType} value {value} is out of range. Must be between {minValue} and {maxValue}")
        {
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }

    public sealed class CoordinateValidationConverter : IValueConverter
    {
        public double MinValue { get; set; } = float.MinValue;
        public double MaxValue { get; set; } = float.MaxValue;
        public string CoordinateType { get; set; } = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                    return string.Empty;

                if (double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    if (result >= MinValue && result <= MaxValue)
                    {
                        return stringValue;
                    }
                    else
                    {
                        throw new CoordinateRangeException(result, MinValue, MaxValue, CoordinateType);
                    }
                }
                else
                {
                    throw new CoordinateValidationException($"Invalid {CoordinateType} format. Please enter a valid number.");
                }
            }

            return value;
        }
    }
}
