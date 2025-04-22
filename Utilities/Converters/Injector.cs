using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows.Data;
using Utilities.Extensions;

namespace Utilities.Converters
{
    public class Injector : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Type viewModelType)
            {
                if (Inject.ServiceProvider is null)
                {
                    return Activator.CreateInstance(viewModelType, value);
                }

                try
                {
                    return ActivatorUtilities.CreateInstance(Inject.ServiceProvider, viewModelType, value);
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