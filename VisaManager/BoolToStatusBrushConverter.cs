using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VisaManager
{
    // BoolToStatusBrushConverter.cs
    public class BoolToStatusBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"))
                              : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}
