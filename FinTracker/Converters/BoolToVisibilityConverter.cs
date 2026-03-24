using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FinTracker.Converters
{
    /// <summary>
    /// Converts a boolean value to a <see cref="Visibility"/> value for use in WPF data bindings.
    /// When the boolean is true, the element is <see cref="Visibility.Visible"/>;
    /// when false, the element is <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a <see cref="Visibility"/> enumeration value.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The target type of the binding (expected to be <see cref="Visibility"/>).</param>
        /// <param name="parameter">An optional parameter. If set to "Invert", the logic is reversed.</param>
        /// <param name="culture">The culture information for the conversion.</param>
        /// <returns><see cref="Visibility.Visible"/> if true; <see cref="Visibility.Collapsed"/> if false (or vice versa if inverted).</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = value is bool boolVal && boolVal;

            bool shouldInvertLogic = parameter is string paramString
                && paramString.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            if (shouldInvertLogic)
            {
                booleanValue = !booleanValue;
            }

            return booleanValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a <see cref="Visibility"/> value back to a boolean.
        /// </summary>
        /// <param name="value">The <see cref="Visibility"/> value to convert back.</param>
        /// <param name="targetType">The target type of the binding (expected to be <see cref="bool"/>).</param>
        /// <param name="parameter">An optional parameter. If set to "Invert", the logic is reversed.</param>
        /// <param name="culture">The culture information for the conversion.</param>
        /// <returns>True if <see cref="Visibility.Visible"/>; false otherwise.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is Visibility visibility && visibility == Visibility.Visible;

            bool shouldInvertLogic = parameter is string paramString
                && paramString.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            if (shouldInvertLogic)
            {
                isVisible = !isVisible;
            }

            return isVisible;
        }
    }
}
