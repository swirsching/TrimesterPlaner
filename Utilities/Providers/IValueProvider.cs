namespace Utilities.Providers
{
    public interface IValueProvider<T>
    {
        public T Get();
        public void Set(T? value);
    }
}