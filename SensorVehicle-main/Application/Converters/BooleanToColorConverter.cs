using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Application.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        private static readonly Color DefaultTrueColor = Colors.LimeGreen;

        /// <summary>
        /// Converts bool to color
        /// </summary>
        /// <param name="value">Boolean value controlling color change</param>
        /// <param name="targetType">Color</param>
        /// <param name="parameter">Name of the color true should be mapped to (default is LimeGreen).</param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color colorIfFalse = Colors.Transparent;
            Color colorIfTrue = parameter == null ? DefaultTrueColor : ParseColor((string)parameter);

            return (value is bool && (bool)value) ? new SolidColorBrush(colorIfTrue) : new SolidColorBrush(colorIfFalse);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static Color ParseColor(string colorName)
        {
            System.Drawing.Color systemColor = System.Drawing.Color.FromName(colorName);
            return systemColor.IsKnownColor ? Color.FromArgb(systemColor.A, systemColor.R, systemColor.G, systemColor.B) : DefaultTrueColor;
        }
    }
}
