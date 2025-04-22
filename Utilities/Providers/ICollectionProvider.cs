namespace Utilities.Providers
{
    public interface ICollectionProvider<T>
    {
        public IEnumerable<T> Get();
        public void Set(IEnumerable<T> values);
        public void Remove(T value);
    }
}