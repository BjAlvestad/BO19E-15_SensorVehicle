using System;
using Windows.UI.Xaml.Data;

namespace Application.Converters
{
    //Inspiration https://stackoverflow.com/questions/34026332/string-format-using-uwp-and-xbind
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            if (parameter == null)
                return value;

            return string.Format((string) parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // XAML usage examples:
    /* E.g. to convert a TimeSpan the following code is added together with the x:Bind
       Converter={StaticResource StringFormatConverter}, ConverterParameter='{}{0:hh\\\\:mm\\\\:ss}'
       Note the use of quadruple slashes. This is due to a \ is required in the string for the : since custom TimeSpan format specifier do not include placeholders https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings
    */
}
