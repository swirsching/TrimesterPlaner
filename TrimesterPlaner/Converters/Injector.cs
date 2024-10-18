using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows.Data;

namespace TrimesterPlaner.Converters
{
    public class Injector : IValueConverter
    {
        internal static IServiceProvider? ServiceProvider { private get; set; }
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Type viewModelType)
            {
                if (ServiceProvider is null)
                {
                    return Activator.CreateInstance(viewModelType, value);
                }

                try
                {
                    return ActivatorUtilities.CreateInstance(ServiceProvider, viewModelType, value);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}