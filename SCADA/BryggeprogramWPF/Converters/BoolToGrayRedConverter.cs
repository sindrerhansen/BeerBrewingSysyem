using System;
using System.Windows.Data;

namespace BryggeprogramWPF.Converters
{
    class BoolToGrayRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ret;
            if ((bool)value)
            {
                ret = "Red";
            }
            else
            {
                ret = "Gray";
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
