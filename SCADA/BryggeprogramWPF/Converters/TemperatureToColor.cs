using System.Windows.Data;
using System.Windows.Media;


namespace BryggeprogramWPF.Converters
{
    public class TemperatureToColor : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double temp = (double)value;
            int r;
            r = System.Convert.ToInt32(temp);
            Color c = Color.FromRgb((byte)r, 0, 200);
            return c;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
