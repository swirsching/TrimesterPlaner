using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class Field(PropertyInfo propertyInfo, string title, object? value) : BindableBase
    {
        internal PropertyInfo PropertyInfo { get; } = propertyInfo;

        public string Title { get; } = title;

        private object? _Value = value;
        public object? Value
        {
            get => _Value;
            set => SetProperty(ref _Value, Convert.ChangeType(value, Nullable.GetUnderlyingType(PropertyInfo.PropertyType) ?? PropertyInfo.PropertyType));
        }
    }

    public class DialogViewModel : BindableBase
    {
        public DialogViewModel() 
        {
            Fields = [];
        }

        internal void Initialize<T>(T? initial)
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                var value = initial is not null ? propertyInfo.GetValue(initial) : propertyInfo.PropertyType.GetDefaultValue();
                Fields.Add(new Field(propertyInfo, propertyInfo.Name, value));
            }
        }

        public ObservableCollection<Field> Fields { get; }

        internal T? GetResult<T>()
        {
            var parameterlessConstructor = typeof(T).GetConstructor(Type.EmptyTypes);

            if (parameterlessConstructor is not null)
            {
                return GetResultWithParameterlessConstructor<T>();
            }
            return GetResultWithParameterConstructor<T>();
        }

        private T? GetResultWithParameterlessConstructor<T>()
        {
            var inst = (T?)Activator.CreateInstance(typeof(T));
            foreach (Field field in Fields)
            {
                field.PropertyInfo.SetValue(inst, field.Value);
            }
            return inst;
        }

        private T? GetResultWithParameterConstructor<T>()
        {
            List<object?> values = [];
            foreach (Field field in Fields)
            {
                values.Add(field.Value);
            }

            object?[]? args = [.. values];
            return (T?)Activator.CreateInstance(typeof(T), args);
        }
    }
}