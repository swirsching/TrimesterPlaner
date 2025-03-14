namespace TrimesterPlaner.Providers
{
    public interface ICollectionProvider<T>
    {
        public IEnumerable<T> Get();
        public void Set(IEnumerable<T> values);
        public void Remove(T value);
    }

    public interface IValueProvider<T>
    {
        public T Get();
        public void Set(T value);
    }
}