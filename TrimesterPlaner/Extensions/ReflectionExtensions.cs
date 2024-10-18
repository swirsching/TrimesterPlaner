namespace TrimesterPlaner.Extensions
{
    public static class ReflectionExtensions
    {
        public static object? GetDefaultValue(this Type type)
        {
            if (type.IsPrimitive)
            {
                return Activator.CreateInstance(type);
            }

            if (type == typeof(string))
            {
                return "";
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Now;
            }

            return null;
        }
    }
}