using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ShadowPet.Desktop.Converters
{
    public class RelativeTranslateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double size && parameter is string multiplierString)
            {
                if (double.TryParse(multiplierString, CultureInfo.InvariantCulture, out double multiplier))
                {
                    return size * multiplier;
                }
            }
            return 0.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
