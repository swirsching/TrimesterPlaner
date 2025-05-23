﻿using Microsoft.Extensions.DependencyInjection;
using System.Windows.Markup;

namespace Utilities.Extensions
{
    public class Inject(Type type) : MarkupExtension
    {
        public static IServiceProvider? ServiceProvider { get; set; }
        private Type Type { get; } = type;
        public override object? ProvideValue(IServiceProvider serviceProvider) => ServiceProvider?.GetRequiredService(Type);

        public static T Require<T>() where T : notnull
        {
            return ServiceProvider!.GetRequiredService<T>();
        }
    }
}