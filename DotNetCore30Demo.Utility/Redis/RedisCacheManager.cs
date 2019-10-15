using System;
using System.Threading.Tasks;
using CSRedis;
using DotNetCore30Demo.Utility.Helper;

namespace DotNetCore30Demo.Utility.Redis
{
    public class RedisCacheManager:IRedisCacheManager
    {
        private CSRedisClient client;
        public RedisCacheManager()
        {
            string redisConnectionString = AppSettings.App("AppSettings", "RedisCachingAOP", "ConnectionString");//获取连接字符串
            if (!string.IsNullOrWhiteSpace(redisConnectionString))
            {
                client = new CSRedisClient(redisConnectionString);
            }
            else
            {
                throw new ArgumentNullException(nameof(redisConnectionString));
            }
            RedisHelper.Initialization(client);
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return client.Get<T>(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return await client.GetAsync<T>(key);
        }

        public bool Set<T>(string key, T value, int expireSeconds = -1)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return RedisHelper.Set(key, value, expireSeconds);
        }

        public async Task<bool> SetAsync<T>(string key, T value, int expireSeconds = -1)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            
            return await RedisHelper.SetAsync(key, value, expireSeconds);
        }

        public bool Del(params string[] key)
        {
            if (key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            return RedisHelper.Del(key) > 0;
        }

        public async Task<bool> DelAsync(params string[] key)
        {
            if (key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            return await RedisHelper.DelAsync(key) > 0;
        }
    }
}