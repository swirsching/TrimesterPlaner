using System.Globalization;
using System.Windows.Data;

namespace Utilities.Converters
{
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DayOfWeek dayOfWeek)
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return "M";
                    case DayOfWeek.Tuesday:
                        return "D";
                    case DayOfWeek.Wednesday:
                        return "M";
                    case DayOfWeek.Thursday:
                        return "D";
                    case DayOfWeek.Friday:
                        return "F";
                    case DayOfWeek.Saturday:
                        return "S";
                    case DayOfWeek.Sunday:
                        return "S";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}