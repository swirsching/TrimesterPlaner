using Microsoft.Extensions.DependencyInjection;
using System.Windows.Markup;
using TrimesterPlaner.Providers;

namespace TrimesterPlaner.Extensions
{
    public class Inject(Type type) : MarkupExtension
    {
        internal static IServiceProvider? ServiceProvider { get; set; }
        private Type Type { get; } = type;
        public override object? ProvideValue(IServiceProvider serviceProvider) => ServiceProvider?.GetRequiredService(Type);

        public static T Require<T>() where T : notnull
        {
            return ServiceProvider!.GetRequiredService<T>();
        }

        public static IEnumerable<T> GetCollection<T>() where T : notnull
        {
            return Require<ICollectionProvider<T>>().Get();
        }

        public static void SetCollection<T>(IEnumerable<T> collection) where T : notnull
        {
            Require<ICollectionProvider<T>>().Set(collection);
        }

        public static T GetValue<T>() where T : notnull
        {
            return Require<IValueProvider<T>>().Get();
        }

        public static void SetValue<T>(T value) where T : notnull
        {
            Require<IValueProvider<T>>().Set(value);
        }
    }
}