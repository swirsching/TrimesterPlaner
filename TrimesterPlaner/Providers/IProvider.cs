namespace TrimesterPlaner.Providers
{
    public interface IProvider<T>
    {
        public IEnumerable<T> GetAll();
        public void SetAll(IEnumerable<T> values);
        public void Remove(T value);
    }
}