namespace TrimesterPlaner.Extensions
{
    public static class ReflectionExtensions
    {
        public static void Update<T>(this T instance, T value)
        {
            if (instance is null || value is null)
            {
                return;
            }

            Type type = instance.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.SetMethod is null)
                {
                    continue;
                }
                property.SetValue(instance, property.GetValue(value));
            }
        }
    }
}