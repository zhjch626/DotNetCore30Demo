using System;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetCore30Demo.Utility.MemoryCache
{
    /// <summary>
    /// 实例化缓存接口IMemoryCaching
    /// </summary>
    public class MemoryCaching:IMemoryCaching
    {
        //引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        private readonly IMemoryCache _cache;

        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string cacheKey)
        {
            return _cache.Get<T>(cacheKey);
        }

        public void Set(string cacheKey, object cacheValue)
        {
            _cache.Set(cacheKey, cacheValue);
        }

        public void Set(string cacheKey, object cacheValue,int expireSeconds)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(expireSeconds));
        }


    }
}