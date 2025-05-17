using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VisaManager
{
    public class ExpiryStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 0 && values[0] is string dateStr && DateTime.TryParse(dateStr, out DateTime expireDate))
            {
                int daysLeft = (expireDate - DateTime.Now).Days;
                return daysLeft switch
                {
                    < 0 => new SolidColorBrush(Colors.Red),    // Expired
                    < 30 => new SolidColorBrush(Colors.Orange), // Urgent
                    < 60 => new SolidColorBrush(Colors.Yellow), // Warning
                    _ => new SolidColorBrush(Colors.Green)      // Valid
                };
            }
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
