namespace DotNetCore30Demo.Utility.MemoryCache
{
    public interface IMemoryCaching
    {
        T Get<T>(string cacheKey);

        void Set(string cacheKey, object cacheValue);

        void Set(string cacheKey, object cacheValue, int expireSeconds);
    }
}