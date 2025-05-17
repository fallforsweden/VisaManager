using System;
using System.Globalization;
using System.Windows.Data;

namespace VisaManager
{
    public class DaysLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateStr && DateTime.TryParse(dateStr, out DateTime expireDate))
            {
                int daysLeft = (expireDate - DateTime.Now).Days;
                return daysLeft switch
                {
                    < 0 => "🔴 Expired",
                    < 30 => $"🟠 {daysLeft} days (Urgent)",
                    < 60 => $"🟡 {daysLeft} days (Warning)",
                    _ => $"🟢 {daysLeft} days"
                };
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}