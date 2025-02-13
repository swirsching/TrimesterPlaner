using Microsoft.Extensions.DependencyInjection;
using System.Windows.Markup;

namespace TrimesterPlaner.Extensions
{
    public class InjectExtension(Type type) : MarkupExtension
    {
        internal static IServiceProvider? ServiceProvider { get; set; }
        private Type Type { get; } = type;
        public override object? ProvideValue(IServiceProvider serviceProvider) => ServiceProvider?.GetRequiredService(Type);
    }
}