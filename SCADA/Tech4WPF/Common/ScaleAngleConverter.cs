using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Tech4WPF.Common
{
    /// <summary>
    /// This class provides scale angle (de)stretching.
    /// Stretch direction define string parameter. It can be 
    /// "StartAngle" or "EndAngle" or null.
    /// In parameter null case returns original value
    /// </summary>
    internal class ScaleAngleConverter : IValueConverter
    {
        #region Constants

        /// <summary>
        /// Addition for scale stretching to boundary major stamp corners
        /// </summary>
        private const double SCALE_STRETCH = 1.8;

        #endregion Constants

        #region Methods

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">
        /// The converter parameter to use. 
        /// It can be string repesentating"StartAngle" or "EndAngle" or null
        /// </param>
        /// <param name="culture">The culture has no effect in this converter.</param>
        /// <returns>
        /// A converted value. If parameter is null, returns value from binding source as is.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (parameter.ToString())
            {
                case "StartAngle":
                    return (double)value - SCALE_STRETCH;
                case "EndAngle":
                    return (double)value + SCALE_STRETCH;
                default:
                    return value;
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture has no effect in this converter.</param>
        /// <returns>
        /// A converted value. If parameter is null, returns value from binding target as is.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (parameter.ToString())
            {
                case "StartAngle":
                    return (double)value + SCALE_STRETCH;
                case "EndAngle":
                    return (double)value - SCALE_STRETCH;
                default:
                    return value;
            }
        }

        #endregion Mehods
    }
}
